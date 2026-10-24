using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AppDataCleaner
{
    /// <summary>
    /// 扫描结果中的一项。
    /// </summary>
    public class ScanResultItem
    {
        public string Path { get; set; } = "";
        public long SizeRaw { get; set; }
        public string SizeFormatted { get; set; } = "";
        public string Description { get; set; } = "";

        /// <summary>true=目录, false=文件</summary>
        public bool IsDirectory { get; set; }

        /// <summary>文件或文件夹名称。</summary>
        public string Name => System.IO.Path.GetFileName(Path);
    }

    /// <summary>
    /// 扫描服务，负责扫描 AppData 目录并分批返回结果。
    /// 支持取消、进度报告与批量数据收集。
    /// </summary>
    internal sealed class ScanService
    {
        private readonly IReadOnlyDictionary<string, string> _notes;

        /// <summary>顶层枚举选项：忽略无权访问项，跳过目录联接/符号链接。</summary>
        private static readonly EnumerationOptions TopLevelOptions = new()
        {
            IgnoreInaccessible = true,
            RecurseSubdirectories = false,
            AttributesToSkip = FileAttributes.ReparsePoint,
            ReturnSpecialDirectories = false,
            MatchType = MatchType.Simple
        };

        /// <summary>递归统计大小时使用：忽略无权访问项，并跳过联接避免循环。</summary>
        private static readonly EnumerationOptions SizeOptions = new()
        {
            IgnoreInaccessible = true,
            RecurseSubdirectories = true,
            AttributesToSkip = FileAttributes.ReparsePoint,
            ReturnSpecialDirectories = false,
            MatchType = MatchType.Simple
        };

        public ScanService(IReadOnlyDictionary<string, string> notes)
        {
            _notes = notes;
        }

        /// <summary>
        /// 获取选中目录的完整路径列表。
        /// </summary>
        public static List<(string Name, string Path)> GetTargetPaths(IEnumerable<string> checkedNames)
        {
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string appDataRoot = Path.Combine(userProfile, "AppData");

            var results = new List<(string, string)>();
            foreach (var name in checkedNames)
            {
                if (name == "ProgramData")
                {
                    if (Directory.Exists(programData))
                        results.Add((name, programData));
                }
                else
                {
                    string path = Path.Combine(appDataRoot, name);
                    if (Directory.Exists(path))
                        results.Add((name, path));
                }
            }
            return results;
        }

        /// <summary>
        /// 扫描，结果通过 batchReporter 分批返回。
        /// </summary>
        public void Scan(
            IEnumerable<string> checkedNames,
            IProgress<int>? progress,
            IProgress<string>? statusText,
            CancellationToken token,
            IProgress<IReadOnlyList<ScanResultItem>> batchReporter,
            int batchSize = 50)
        {
            var targets = GetTargetPaths(checkedNames);
            if (targets.Count == 0)
            {
                statusText?.Report("没有可扫描的目录。");
                progress?.Report(0);
                return;
            }

            int totalItems = CountTopLevelItems(targets);
            int processed = 0;
            int lastPercent = -1;
            long lastStatusTicks = 0;
            var batch = new List<ScanResultItem>(batchSize);

            void ReportProgress()
            {
                int percent = totalItems <= 0
                    ? 0
                    : (int)Math.Min(100L, (long)processed * 100L / totalItems);

                if (percent != lastPercent)
                {
                    lastPercent = percent;
                    progress?.Report(percent);
                }
            }

            // 状态文字更新过于频繁会淹没 UI 消息队列，这里做轻量节流。
            void ReportStatus(string text, bool force = false)
            {
                long now = Environment.TickCount64;
                if (!force && now - lastStatusTicks < 120)
                    return;

                lastStatusTicks = now;
                statusText?.Report(text);
            }

            void FlushBatch(bool force)
            {
                if (batch.Count == 0 || (!force && batch.Count < batchSize))
                    return;

                batchReporter.Report(batch.ToArray());
                batch = new List<ScanResultItem>(batchSize);
            }

            ReportStatus($"正在扫描，共 {totalItems:N0} 项...", force: true);

            foreach (var (folderName, folderPath) in targets)
            {
                token.ThrowIfCancellationRequested();
                ReportStatus($"正在扫描 {folderName} ...", force: true);

                foreach (var file in EnumerateTopLevelFiles(folderPath))
                {
                    token.ThrowIfCancellationRequested();

                    try
                    {
                        var info = new FileInfo(file);
                        long size = info.Length;

                        batch.Add(new ScanResultItem
                        {
                            Path = file,
                            SizeRaw = size,
                            SizeFormatted = FormatSize(size),
                            Description = GetNote(info.Name),
                            IsDirectory = false
                        });

                        FlushBatch(force: false);
                    }
                    catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                    {
                        AppLogger.Warn($"读取文件大小失败: {file} | {ex.Message}");
                    }

                    processed++;
                    ReportProgress();
                }

                // 文件扫描完成后先提交已有结果，避免在统计大目录时界面长时间无数据。
                FlushBatch(force: true);

                foreach (var subDir in EnumerateTopLevelDirectories(folderPath))
                {
                    token.ThrowIfCancellationRequested();

                    try
                    {
                        ReportStatus($"正在统计 {Path.GetFileName(subDir)} 的大小...");
                        long size = GetDirectorySize(subDir, token);

                        batch.Add(new ScanResultItem
                        {
                            Path = subDir,
                            SizeRaw = size,
                            SizeFormatted = FormatSize(size),
                            Description = GetNote(Path.GetFileName(subDir)),
                            IsDirectory = true
                        });

                        FlushBatch(force: false);
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        AppLogger.Warn($"计算目录大小失败: {subDir} | {ex.Message}");
                    }

                    processed++;
                    ReportProgress();
                }
            }

            FlushBatch(force: true);

            progress?.Report(100);
        }

        /// <summary>
        /// 计算目录总大小。使用忽略无权访问项、跳过目录联接的递归枚举，
        /// 避免因某个子目录无权限而丢弃整个目录的统计结果。
        /// </summary>
        private static long GetDirectorySize(string dirPath, CancellationToken token)
        {
            long size = 0;

            try
            {
                foreach (var file in Directory.EnumerateFiles(dirPath, "*", SizeOptions))
                {
                    token.ThrowIfCancellationRequested();

                    try
                    {
                        size += new FileInfo(file).Length;
                    }
                    catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                    {
                        // 单个文件读不到时跳过，不影响目录其余部分
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                AppLogger.Warn($"遍历目录失败: {dirPath} | {ex.Message}");
            }

            return size;
        }

        private static int CountTopLevelItems(IEnumerable<(string Name, string Path)> targets)
        {
            int count = 0;
            foreach (var (_, path) in targets)
            {
                try
                {
                    count += Directory.EnumerateFileSystemEntries(path, "*", TopLevelOptions).Count();
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                {
                    AppLogger.Warn($"统计目录项目失败: {path} | {ex.Message}");
                }
            }
            return count;
        }

        private static IEnumerable<string> EnumerateTopLevelFiles(string folderPath)
        {
            try
            {
                return Directory.EnumerateFiles(folderPath, "*", TopLevelOptions).ToArray();
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                AppLogger.Warn($"枚举文件失败: {folderPath} | {ex.Message}");
                return Array.Empty<string>();
            }
        }

        private static IEnumerable<string> EnumerateTopLevelDirectories(string folderPath)
        {
            try
            {
                return Directory.EnumerateDirectories(folderPath, "*", TopLevelOptions).ToArray();
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                AppLogger.Warn($"枚举目录失败: {folderPath} | {ex.Message}");
                return Array.Empty<string>();
            }
        }

        private string GetNote(string name)
        {
            return _notes.TryGetValue(name, out var note) ? note ?? "" : "";
        }

        /// <summary>
        /// 将字节数格式化为易读的大小文本。
        /// </summary>
        public static string FormatSize(long size)
        {
            if (size < 0)
                size = 0;

            if (size < 1024)
                return $"{size} B";
            if (size < 1024L * 1024)
                return $"{size / 1024.0:F2} KB";
            if (size < 1024L * 1024 * 1024)
                return $"{size / (1024.0 * 1024):F2} MB";
            if (size < 1024L * 1024 * 1024 * 1024)
                return $"{size / (1024.0 * 1024 * 1024):F2} GB";

            return $"{size / (1024.0 * 1024 * 1024 * 1024):F2} TB";
        }
    }
}
