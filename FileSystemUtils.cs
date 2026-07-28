using System;
using System.IO;
using System.Security.Principal;

namespace AppDataCleaner
{
    /// <summary>
    /// 文件系统工具类：回收站删除、管理员权限检测等。
    /// </summary>
    internal static class FileSystemUtils
    {
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
        /// 永久删除文件或文件夹。
        /// </summary>
        public static bool ForceDelete(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                    return true;
                }
                else if (File.Exists(path))
                {
                    File.Delete(path);
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
    }
}