using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AppDataCleaner
{
    /// <summary>
    /// 带圆角与可选边框的面板，用作卡片容器。
    /// </summary>
    public class RoundedPanel : Panel
    {
        private int _cornerRadius = 12;
        private Color _borderColor = Theme.Border;
        private int _borderThickness = 1;

        public RoundedPanel()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint,
                true);

            BackColor = Theme.Surface;
        }

        public int CornerRadius
        {
            get => _cornerRadius;
            set
            {
                int radius = Math.Max(0, value);
                if (_cornerRadius == radius) return;
                _cornerRadius = radius;
                UpdateRegion();
                Invalidate();
            }
        }

        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                if (_borderColor == value) return;
                _borderColor = value;
                Invalidate();
            }
        }

        public int BorderThickness
        {
            get => _borderThickness;
            set
            {
                int thickness = Math.Max(0, value);
                if (_borderThickness == thickness) return;
                _borderThickness = thickness;
                Invalidate();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateRegion();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_borderThickness <= 0 || _borderColor.A == 0)
                return;

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            if (rect.Width <= 0 || rect.Height <= 0)
                return;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var path = Theme.RoundedRect(rect, _cornerRadius);
            using var pen = new Pen(_borderColor, _borderThickness);
            e.Graphics.DrawPath(pen, path);
        }

        private void UpdateRegion()
        {
            if (Width <= 0 || Height <= 0)
                return;

            var oldRegion = Region;
            using (var path = Theme.RoundedRect(new Rectangle(0, 0, Width, Height), _cornerRadius))
            {
                Region = new Region(path);
            }
            oldRegion?.Dispose();
        }
    }
}
