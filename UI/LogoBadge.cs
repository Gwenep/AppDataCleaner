using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AppDataCleaner
{
    /// <summary>
    /// 标题栏左侧的圆角 Logo 徽章，直接绘制应用图标（AppDataCleaner.ico）。
    /// </summary>
    public class LogoBadge : Control
    {
        private Bitmap? _logo;

        public LogoBadge()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint |
                ControlStyles.SupportsTransparentBackColor,
                true);

            BackColor = Color.Transparent;
            Size = new Size(48, 48);

            _logo = AppIcon.LoadBitmap(256);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            if (rect.Width <= 0 || rect.Height <= 0)
                return;

            // 浅色圆角底板，让深色图标在深蓝标题栏上依然清晰。
            using (var path = Theme.RoundedRect(rect, 14))
            using (var brush = new LinearGradientBrush(
                       rect,
                       Color.FromArgb(255, 255, 255),
                       Color.FromArgb(226, 232, 240),
                       45f))
            {
                e.Graphics.FillPath(brush, path);
            }

            if (_logo is not null)
            {
                int pad = Math.Max(3, Width / 8);
                var target = Rectangle.Inflate(rect, -pad, -pad);
                e.Graphics.DrawImage(_logo, target);
                return;
            }

            // 图标加载失败时的兜底文字。
            using var font = new Font("Segoe UI", 16f, FontStyle.Bold, GraphicsUnit.Pixel);
            TextRenderer.DrawText(
                e.Graphics,
                "AC",
                font,
                new Rectangle(0, 0, Width, Height),
                Theme.PrimaryDark,
                TextFormatFlags.HorizontalCenter |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.NoPadding);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _logo?.Dispose();
                _logo = null;
            }

            base.Dispose(disposing);
        }
    }
}
