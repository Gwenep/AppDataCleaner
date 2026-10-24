using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace AppDataCleaner
{
    /// <summary>
    /// 统一加载应用图标：优先使用嵌入资源，其次使用程序目录下的 ico 文件。
    /// 这样无论通过 apphost、dotnet 还是单文件方式启动，窗口图标都一致。
    /// </summary>
    internal static class AppIcon
    {
        private const string ResourceName = "AppDataCleaner.AppDataCleaner.ico";

        public static Icon? Load()
        {
            try
            {
                using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName);
                if (stream is not null)
                    return new Icon(stream);
            }
            catch (Exception ex)
            {
                AppLogger.Warn($"加载内置图标失败: {ex.Message}");
            }

            try
            {
                string path = Path.Combine(FileSystemUtils.GetApplicationDirectory(), "AppDataCleaner.ico");
                if (File.Exists(path))
                    return new Icon(path);
            }
            catch (Exception ex)
            {
                AppLogger.Warn($"加载外部图标失败: {ex.Message}");
            }

            try
            {
                return Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            }
            catch (Exception ex)
            {
                AppLogger.Warn($"提取程序图标失败: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 加载用于界面绘制的位图版本图标（例如标题栏 Logo）。
        /// 会从 ico 中挑选最接近 <paramref name="size"/> 的帧，避免放大后模糊。
        /// </summary>
        public static Bitmap? LoadBitmap(int size = 256)
        {
            if (size <= 0)
                size = 256;

            try
            {
                using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName);
                if (stream is not null)
                {
                    using var icon = new Icon(stream, size, size);
                    return icon.ToBitmap();
                }
            }
            catch (Exception ex)
            {
                AppLogger.Warn($"加载内置图标位图失败: {ex.Message}");
            }

            try
            {
                string path = Path.Combine(FileSystemUtils.GetApplicationDirectory(), "AppDataCleaner.ico");
                if (File.Exists(path))
                {
                    using var icon = new Icon(path, size, size);
                    return icon.ToBitmap();
                }
            }
            catch (Exception ex)
            {
                AppLogger.Warn($"加载外部图标位图失败: {ex.Message}");
            }

            try
            {
                using var icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                return icon?.ToBitmap();
            }
            catch (Exception ex)
            {
                AppLogger.Warn($"提取程序图标位图失败: {ex.Message}");
                return null;
            }
        }
    }
}
