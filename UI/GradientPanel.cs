using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AppDataCleaner
{
    /// <summary>
    /// 顶部渐变横幅，带装饰光晕。
    /// </summary>
    public class GradientPanel : Panel
    {
        public GradientPanel()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint,
                true);

            Color1 = Theme.HeaderStart;
            Color2 = Theme.HeaderEnd;
        }

        public Color Color1 { get; set; }

        public Color Color2 { get; set; }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (ClientRectangle.Width <= 0 || ClientRectangle.Height <= 0)
                return;

            using var brush = new LinearGradientBrush(ClientRectangle, Color1, Color2, 0f);
            e.Graphics.FillRectangle(brush, ClientRectangle);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (var glow = new SolidBrush(Color.FromArgb(28, 96, 165, 250)))
                e.Graphics.FillEllipse(glow, Width - 330, -140, 460, 460);

            using (var glow = new SolidBrush(Color.FromArgb(20, 129, 140, 248)))
                e.Graphics.FillEllipse(glow, Width - 190, 10, 240, 240);

            base.OnPaint(e);
        }
    }
}
