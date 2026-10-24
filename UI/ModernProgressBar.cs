using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AppDataCleaner
{
    /// <summary>
    /// 圆角胶囊样式的进度条。
    /// </summary>
    public class ModernProgressBar : Control
    {
        private int _value;
        private int _maximum = 100;

        public ModernProgressBar()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint,
                true);

            Height = 8;
        }

        public int Value
        {
            get => _value;
            set
            {
                int clamped = Math.Clamp(value, 0, Maximum);
                if (_value == clamped) return;
                _value = clamped;
                Invalidate();
            }
        }

        public int Maximum
        {
            get => _maximum;
            set
            {
                int maximum = Math.Max(1, value);
                if (_maximum == maximum) return;
                _maximum = maximum;
                _value = Math.Min(_value, _maximum);
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 0 || Height <= 0)
                return;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            int radius = Height / 2;
            var trackRect = new Rectangle(0, 0, Width, Height);
            using (var trackPath = Theme.RoundedRect(trackRect, radius))
            using (var trackBrush = new SolidBrush(Theme.Track))
            {
                e.Graphics.FillPath(trackBrush, trackPath);
            }

            if (_maximum <= 0 || _value <= 0)
                return;

            int fillWidth = (int)Math.Round(Width * (_value / (double)_maximum));
            if (fillWidth <= 0)
                return;

            fillWidth = Math.Max(fillWidth, Height);
            fillWidth = Math.Min(fillWidth, Width);

            var fillRect = new Rectangle(0, 0, fillWidth, Height);
            using var fillPath = Theme.RoundedRect(fillRect, radius);
            using var gradient = new LinearGradientBrush(
                new Rectangle(0, 0, Math.Max(1, fillWidth), Height),
                Theme.Primary,
                Theme.Accent,
                LinearGradientMode.Horizontal);

            e.Graphics.FillPath(gradient, fillPath);
        }
    }
}
