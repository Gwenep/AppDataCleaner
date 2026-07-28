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
    }

    /// <summary>
    /// 扫描服务，负责扫描 AppData 目录并返回结果。
    /// 支持取消、进度报告、批量数据收集。
    /// </summary>
    internal class ScanService
    {
        private readonly Dictionary<string, string> _notes;

        // 不带 ProgramData 的目录列表
        private static readonly string[] AppDataSubFolderNames = { "Local", "LocalLow", "Roaming" };

        public ScanService(Dictionary<string, string> notes)
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
                    string p = Path.Combine(appDataRoot, name);
                    if (Directory.Exists(p))
                        results.Add((name, p));
                }
            }
            return results;
        }

        /// <summary>
        /// 扫描，结果通过 batchCallback 分批返回。
        /// </summary>
        public void Scan(
            IEnumerable<string> checkedNames,
            IProgress<int> progress,
            IProgress<string> statusText,
            CancellationToken token,
            Action<List<ScanResultItem>> batchCallback,
            int batchSize = 50)
        {
            var targets = GetTargetPaths(checkedNames);
            if (targets.Count == 0)
            {
                statusText.Report("没有可扫描的目录。");
                return;
            }

            // 先统计总数用于进度
            int totalItems = 0;
            foreach (var (_, dir) in targets)
            {
                try
                {
                    totalItems += Directory.GetFiles(dir).Length;
                    totalItems += Directory.GetDirectories(dir).Length;
                }
                catch (UnauthorizedAccessException)
                {
                    // 忽略权限不足的目录
                }
            }

            if (totalItems == 0)
                totalItems = 1; // 避免除以零

            statusText.Report($"正在扫描，共 {totalItems} 项...");

            int processed = 0;
            var batch = new List<ScanResultItem>(batchSize);

            // 辅助方法：提交批次
            void FlushBatch()
            {
                if (batch.Count > 0)
                {
                    batchCallback(batch);
                    batch = new List<ScanResultItem>(batchSize);
                }
            }

            foreach (var (folderName, folderPath) in targets)
            {
                token.ThrowIfCancellationRequested();

                // 扫描文件
                IEnumerable<string> files;
                try
                {
                    files = Directory.EnumerateFiles(folderPath);
                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }

                foreach (var file in files)
                {
                    token.ThrowIfCancellationRequested();
                    try
                    {
                        var fi = new FileInfo(file);
                        long size = fi.Length;
                        string name = fi.Name;
                        string desc = GetNote(name);

                        batch.Add(new ScanResultItem
                        {
                            Path = file,
                            SizeRaw = size,
                            SizeFormatted = FormatSize(size),
                            Description = desc,
                            IsDirectory = false
                        });

                        if (batch.Count >= batchSize)
                            FlushBatch();
                    }
                    catch (Exception ex)
                    {
                        AppLogger.Warn($"读取文件大小失败: {file} | {ex.Message}");
                    }

                    Interlocked.Increment(ref processed);
                    progress.Report(processed * 100 / totalItems);
                }

                // 扫描子目录
                IEnumerable<string> subDirs;
                try
                {
                    subDirs = Directory.EnumerateDirectories(folderPath);
                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }

                foreach (var subDir in subDirs)
                {
                    token.ThrowIfCancellationRequested();
                    try
                    {
                        long size = GetDirectorySize(subDir, token);
                        string name = Path.GetFileName(subDir);
                        string desc = GetNote(name);

                        batch.Add(new ScanResultItem
                        {
                            Path = subDir,
                            SizeRaw = size,
                            SizeFormatted = FormatSize(size),
                            Description = desc,
                            IsDirectory = true
                        });

                        if (batch.Count >= batchSize)
                            FlushBatch();
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        AppLogger.Warn($"计算目录大小失败: {subDir} | {ex.Message}");
                    }

                    Interlocked.Increment(ref processed);
                    progress.Report(processed * 100 / totalItems);
                }
            }

            // 提交最后一批
            FlushBatch();

            statusText.Report("扫描完成。");
            progress.Report(100);
        }

        /// <summary>
        /// 计算目录总大小（流式枚举，不一次性加载所有路径）。
        /// </summary>
        private long GetDirectorySize(string dirPath, CancellationToken token)
        {
            long size = 0;
            try
            {
                foreach (var file in Directory.EnumerateFiles(dirPath, "*.*", SearchOption.AllDirectories))
                {
                    token.ThrowIfCancellationRequested();
                    try
                    {
                        size += new FileInfo(file).Length;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // 忽略拒绝访问的文件
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // 忽略拒绝访问的目录
            }
            return size;
        }

        private string GetNote(string name)
        {
            return _notes.TryGetValue(name, out var note) ? note : "";
        }

        private static string FormatSize(long size)
        {
            if (size < 1024)
                return $"{size} B";
            if (size < 1024 * 1024)
                return $"{size / 1024.0:F2} KB";
            if (size < 1024L * 1024 * 1024)
                return $"{size / (1024.0 * 1024):F2} MB";
            return $"{size / (1024.0 * 1024 * 1024):F2} GB";
        }
    }
}