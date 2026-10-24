using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AppDataCleaner
{
    public enum ButtonVariant
    {
        Primary,
        Secondary,
        Danger,
        Warning,
        Ghost
    }

    /// <summary>
    /// 扁平圆角按钮，支持主色、次要、危险、警告等样式。
    /// </summary>
    public class ModernButton : Button
    {
        private bool _hovered;
        private bool _pressed;
        private ButtonVariant _variant = ButtonVariant.Secondary;
        private int _cornerRadius = 10;

        public ModernButton()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint,
                true);

            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            UseVisualStyleBackColor = false;
            Font = Theme.ButtonFont;
            Cursor = Cursors.Hand;
            Size = new Size(100, 38);
        }

        [DefaultValue(ButtonVariant.Secondary)]
        public ButtonVariant Variant
        {
            get => _variant;
            set
            {
                if (_variant == value) return;
                _variant = value;
                Invalidate();
            }
        }

        [DefaultValue(10)]
        public int CornerRadius
        {
            get => _cornerRadius;
            set
            {
                int radius = Math.Max(0, value);
                if (_cornerRadius == radius) return;
                _cornerRadius = radius;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var (backColor, foreColor, borderColor) = GetPalette();

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var backdrop = Parent?.BackColor ?? Theme.Surface;
            if (backdrop.A == 0)
                backdrop = Theme.Surface;

            e.Graphics.Clear(backdrop);

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            if (rect.Width <= 0 || rect.Height <= 0)
                return;

            using (var path = Theme.RoundedRect(rect, _cornerRadius))
            {
                using (var brush = new SolidBrush(backColor))
                    e.Graphics.FillPath(brush, path);

                if (borderColor.A > 0)
                {
                    using var pen = new Pen(borderColor, 1f);
                    e.Graphics.DrawPath(pen, path);
                }
            }

            TextRenderer.DrawText(
                e.Graphics,
                Text,
                Font,
                ClientRectangle,
                foreColor,
                TextFormatFlags.HorizontalCenter |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis |
                TextFormatFlags.NoPrefix);

            if (Focused && ShowFocusCues)
            {
                var focusRect = Rectangle.Inflate(ClientRectangle, -4, -4);
                ControlPaint.DrawFocusRectangle(e.Graphics, focusRect, foreColor, Color.Transparent);
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _hovered = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _hovered = false;
            _pressed = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            if (mevent.Button == MouseButtons.Left)
            {
                _pressed = true;
                Invalidate();
            }
            base.OnMouseDown(mevent);
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            _pressed = false;
            Invalidate();
            base.OnMouseUp(mevent);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            Invalidate();
            base.OnEnabledChanged(e);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            Invalidate();
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            Invalidate();
            base.OnLostFocus(e);
        }

        protected override void OnKeyDown(KeyEventArgs kevent)
        {
            if (kevent.KeyCode is Keys.Space or Keys.Enter)
            {
                _pressed = true;
                Invalidate();
            }
            base.OnKeyDown(kevent);
        }

        protected override void OnKeyUp(KeyEventArgs kevent)
        {
            if (kevent.KeyCode is Keys.Space or Keys.Enter)
            {
                _pressed = false;
                Invalidate();
            }
            base.OnKeyUp(kevent);
        }

        private (Color Back, Color Fore, Color Border) GetPalette()
        {
            if (!Enabled)
                return (Theme.DisabledBackground, Theme.DisabledText, Theme.Border);

            return _variant switch
            {
                ButtonVariant.Primary => (
                    _pressed ? Theme.PrimaryDarker : _hovered ? Theme.PrimaryDark : Theme.Primary,
                    Color.White,
                    Color.Transparent),

                ButtonVariant.Danger => (
                    _pressed ? Theme.DangerDarker : _hovered ? Theme.DangerDark : Theme.Danger,
                    Color.White,
                    Color.Transparent),

                ButtonVariant.Warning => (
                    _pressed ? Color.FromArgb(253, 186, 116) : _hovered ? Color.FromArgb(255, 237, 213) : Theme.WarningSoft,
                    Theme.WarningText,
                    Theme.WarningBorder),

                ButtonVariant.Ghost => (
                    _pressed ? Theme.Border : _hovered ? Theme.Hover : Theme.Surface,
                    Theme.TextSecondary,
                    Color.Transparent),

                _ => (
                    _pressed ? Theme.Hover : _hovered ? Theme.SurfaceAlt : Theme.Surface,
                    Theme.TextPrimary,
                    Theme.Border)
            };
        }
    }
}
