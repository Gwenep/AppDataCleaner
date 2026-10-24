using System;
using System.IO;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace AppDataCleaner
{
    /// <summary>
    /// 文件系统工具类：回收站删除、管理员权限检测等。
    /// </summary>
    internal static class FileSystemUtils
    {
        /// <summary>
        /// 程序实际所在的目录（用户双击的 exe 所在位置）。
        /// 单文件发布时 AppContext.BaseDirectory 指向临时解压目录，
        /// 而 Assembly.Location 为空，因此必须优先使用 ProcessPath。
        /// </summary>
        public static string GetApplicationDirectory()
        {
            try
            {
                string exePath = Environment.ProcessPath ?? "";
                if (exePath.Length > 0)
                {
                    string? dir = Path.GetDirectoryName(exePath);
                    if (!string.IsNullOrEmpty(dir))
                        return dir;
                }
            }
            catch
            {
                // 忽略并回退
            }

            return AppContext.BaseDirectory;
        }

        /// <summary>
        /// 检测当前是否以管理员身份运行。
        /// </summary>
        public static bool IsAdministrator()
        {
            try
            {
                using var identity = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 将文件或文件夹移动到回收站。
        /// 返回 true 表示成功，false 表示用户取消或失败。
        /// </summary>
        public static bool SendToRecycleBin(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(
                        path,
                        Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,
                        Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
                    return true;
                }
                else if (File.Exists(path))
                {
                    Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(
                        path,
                        Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,
                        Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
                    return true;
                }
                return false;
            }
            catch (OperationCanceledException)
            {
                // 用户取消
                return false;
            }
            catch (Exception ex)
            {
                AppLogger.Error($"移动到回收站失败: {path}", ex);
                throw;
            }
        }

        /// <summary>
        /// 在专用 STA 线程上异步删除文件或文件夹，避免长时间操作阻塞界面。
        /// 回收站操作依赖 Windows Shell，需要 STA 线程。
        /// </summary>
        public static Task<bool> DeleteAsync(string path, bool useRecycleBin)
        {
            var completion = new TaskCompletionSource<bool>(
                TaskCreationOptions.RunContinuationsAsynchronously);

            var thread = new Thread(() =>
            {
                try
                {
                    bool deleted = useRecycleBin
                        ? SendToRecycleBin(path)
                        : ForceDelete(path);
                    completion.TrySetResult(deleted);
                }
                catch (Exception ex)
                {
                    completion.TrySetException(ex);
                }
            })
            {
                IsBackground = true,
                Name = "AppDataCleaner.Delete"
            };

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            return completion.Task;
        }

        /// <summary>
        /// 永久删除文件或文件夹。遇到只读文件时会先清除只读属性再重试。
        /// </summary>
        public static bool ForceDelete(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    try
                    {
                        Directory.Delete(path, true);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        ClearReadOnlyAttributes(path);
                        Directory.Delete(path, true);
                    }
                    return true;
                }

                if (File.Exists(path))
                {
                    try
                    {
                        File.Delete(path);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        var attributes = File.GetAttributes(path);
                        File.SetAttributes(path, attributes & ~FileAttributes.ReadOnly);
                        File.Delete(path);
                    }
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                AppLogger.Error($"永久删除失败: {path}", ex);
                throw;
            }
        }

        private static void ClearReadOnlyAttributes(string directory)
        {
            var options = new EnumerationOptions
            {
                IgnoreInaccessible = true,
                RecurseSubdirectories = true,
                // 跳过目录联接/符号链接，避免递归进入其他目录或形成循环。
                AttributesToSkip = FileAttributes.ReparsePoint,
                ReturnSpecialDirectories = false
            };

            try
            {
                var rootAttributes = File.GetAttributes(directory);
                if ((rootAttributes & FileAttributes.ReadOnly) != 0)
                    File.SetAttributes(directory, rootAttributes & ~FileAttributes.ReadOnly);
            }
            catch
            {
                // 根目录属性无法修改时继续尝试清理子项
            }

            try
            {
                foreach (string path in Directory.EnumerateFileSystemEntries(directory, "*", options))
                {
                    try
                    {
                        var attributes = File.GetAttributes(path);
                        if ((attributes & FileAttributes.ReadOnly) != 0)
                            File.SetAttributes(path, attributes & ~FileAttributes.ReadOnly);
                    }
                    catch
                    {
                        // 单个项目无法修改属性时忽略
                    }
                }
            }
            catch (Exception ex)
            {
                AppLogger.Warn($"清除只读属性失败: {directory} | {ex.Message}");
            }
        }
    }
}