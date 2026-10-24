using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AppDataCleaner
{
    /// <summary>
    /// 带放大镜图标的圆角搜索框。
    /// </summary>
    public class SearchBox : RoundedPanel
    {
        private readonly TextBox _inner;
        private bool _focused;

        public SearchBox()
        {
            CornerRadius = 10;
            BorderColor = Theme.Border;
            BackColor = Theme.Surface;
            Height = 38;
            Padding = new Padding(0);

            _inner = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Font = Theme.BodyFont,
                ForeColor = Theme.TextPrimary,
                BackColor = Theme.Surface,
                PlaceholderText = "搜索路径或备注..."
            };

            _inner.TextChanged += (_, _) => OnTextChanged(EventArgs.Empty);
            _inner.Enter += (_, _) =>
            {
                _focused = true;
                BorderColor = Theme.Primary;
                Invalidate();
            };
            _inner.Leave += (_, _) =>
            {
                _focused = false;
                BorderColor = Theme.Border;
                Invalidate();
            };

            Controls.Add(_inner);
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [AllowNull]
        public override string Text
        {
            get => _inner.Text;
            set => _inner.Text = value ?? "";
        }

        [Browsable(true)]
        [DefaultValue("搜索路径或备注...")]
        public string PlaceholderText
        {
            get => _inner.PlaceholderText;
            set => _inner.PlaceholderText = value;
        }

        public void FocusInput()
        {
            _inner.Focus();
            _inner.SelectAll();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            LayoutInner();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            int centerY = Height / 2;
            var color = _focused ? Theme.Primary : Theme.TextMuted;

            using var pen = new Pen(color, 1.8f)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round
            };

            e.Graphics.DrawEllipse(pen, 16, centerY - 9, 10, 10);
            e.Graphics.DrawLine(pen, 24, centerY + 1, 28, centerY + 5);
        }

        private void LayoutInner()
        {
            if (_inner is null)
                return;

            int preferredHeight = _inner.PreferredHeight;
            int y = Math.Max(0, (Height - preferredHeight) / 2);
            int width = Math.Max(10, Width - 56);
            _inner.SetBounds(40, y, width, preferredHeight);
        }
    }
}
