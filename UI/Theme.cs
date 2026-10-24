using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AppDataCleaner
{
    /// <summary>
    /// 全局视觉主题：配色、字体与圆角绘制辅助方法。
    /// 统一放在这里，方便后续调整整体风格。
    /// </summary>
    internal static class Theme
    {
        // ---- 颜色 ----
        public static readonly Color Background = Color.FromArgb(244, 246, 250);
        public static readonly Color Surface = Color.White;
        public static readonly Color SurfaceAlt = Color.FromArgb(248, 250, 252);
        public static readonly Color Hover = Color.FromArgb(241, 245, 249);
        public static readonly Color Border = Color.FromArgb(226, 232, 240);
        public static readonly Color BorderStrong = Color.FromArgb(203, 213, 225);

        public static readonly Color TextPrimary = Color.FromArgb(15, 23, 42);
        public static readonly Color TextSecondary = Color.FromArgb(100, 116, 139);
        public static readonly Color TextMuted = Color.FromArgb(148, 163, 184);

        public static readonly Color Primary = Color.FromArgb(59, 130, 246);
        public static readonly Color PrimaryDark = Color.FromArgb(37, 99, 235);
        public static readonly Color PrimaryDarker = Color.FromArgb(29, 78, 216);
        public static readonly Color PrimarySoft = Color.FromArgb(239, 246, 255);
        public static readonly Color Accent = Color.FromArgb(99, 102, 241);
        public static readonly Color Selection = Color.FromArgb(219, 234, 254);
        public static readonly Color SelectionText = Color.FromArgb(30, 64, 175);

        public static readonly Color Danger = Color.FromArgb(239, 68, 68);
        public static readonly Color DangerDark = Color.FromArgb(220, 38, 38);
        public static readonly Color DangerDarker = Color.FromArgb(185, 28, 28);
        public static readonly Color Warning = Color.FromArgb(245, 158, 11);
        public static readonly Color WarningSoft = Color.FromArgb(255, 247, 237);
        public static readonly Color WarningBorder = Color.FromArgb(254, 215, 170);
        public static readonly Color WarningText = Color.FromArgb(194, 65, 12);
        public static readonly Color Success = Color.FromArgb(16, 185, 129);

        public static readonly Color DisabledBackground = Color.FromArgb(241, 245, 249);
        public static readonly Color DisabledText = Color.FromArgb(148, 163, 184);
        public static readonly Color Track = Color.FromArgb(226, 232, 240);

        public static readonly Color HeaderStart = Color.FromArgb(15, 23, 42);
        public static readonly Color HeaderEnd = Color.FromArgb(30, 58, 138);

        // ---- 字体 ----
        public static readonly Font BodyFont = new("Microsoft YaHei UI", 9F);
        public static readonly Font SmallFont = new("Microsoft YaHei UI", 8.5F);
        public static readonly Font CaptionFont = new("Microsoft YaHei UI", 8.5F);
        public static readonly Font ButtonFont = new("Microsoft YaHei UI", 9F);
        public static readonly Font SectionFont = new("Microsoft YaHei UI", 10.5F, FontStyle.Bold);
        public static readonly Font TitleFont = new("Microsoft YaHei UI", 18F, FontStyle.Bold);
        public static readonly Font SubtitleFont = new("Microsoft YaHei UI", 9F);
        public static readonly Font MetricFont = new("Microsoft YaHei UI", 18F, FontStyle.Bold);

        /// <summary>
        /// 创建一个圆角矩形路径。半径为 0 时返回普通矩形。
        /// </summary>
        public static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();

            if (bounds.Width <= 0 || bounds.Height <= 0)
                return path;

            if (radius <= 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            int diameter = Math.Min(Math.Max(1, radius * 2), Math.Min(bounds.Width, bounds.Height));
            var arc = new Rectangle(bounds.X, bounds.Y, diameter, diameter);

            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.X;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}
