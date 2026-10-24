using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AppDataCleaner
{
    /// <summary>
    /// 首页数据卡片：顶部说明文字 + 大号数值。
    /// </summary>
    public class StatCard : RoundedPanel
    {
        private string _caption = "";
        private string _value = "";
        private Color _accentColor = Theme.Primary;

        public StatCard()
        {
            CornerRadius = 14;
            BorderColor = Theme.Border;
            BackColor = Theme.Surface;
            Height = 96;
        }

        [DefaultValue("")]
        public string Caption
        {
            get => _caption;
            set
            {
                _caption = value ?? "";
                Invalidate();
            }
        }

        [DefaultValue("")]
        public string Value
        {
            get => _value;
            set
            {
                _value = value ?? "";
                Invalidate();
            }
        }

        public Color AccentColor
        {
            get => _accentColor;
            set
            {
                _accentColor = value;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var accentRect = new Rectangle(18, 24, 4, 14);
            using (var accentPath = Theme.RoundedRect(accentRect, 2))
            using (var accentBrush = new SolidBrush(_accentColor))
            {
                graphics.FillPath(accentBrush, accentPath);
            }

            var captionRect = new Rectangle(30, 20, Math.Max(10, Width - 48), 22);
            TextRenderer.DrawText(
                graphics,
                _caption,
                Theme.CaptionFont,
                captionRect,
                Theme.TextSecondary,
                TextFormatFlags.Left |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis |
                TextFormatFlags.NoPrefix);

            var valueRect = new Rectangle(18, 44, Math.Max(10, Width - 36), 38);
            TextRenderer.DrawText(
                graphics,
                _value,
                Theme.MetricFont,
                valueRect,
                Theme.TextPrimary,
                TextFormatFlags.Left |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis |
                TextFormatFlags.NoPrefix);
        }
    }
}
