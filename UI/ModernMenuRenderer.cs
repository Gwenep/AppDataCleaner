using System.Drawing;
using System.Windows.Forms;

namespace AppDataCleaner
{
    /// <summary>
    /// 右键菜单的浅色现代化配色。
    /// </summary>
    public class ModernColorTable : ProfessionalColorTable
    {
        public override Color ToolStripDropDownBackground => Theme.Surface;
        public override Color ImageMarginGradientBegin => Theme.Surface;
        public override Color ImageMarginGradientMiddle => Theme.Surface;
        public override Color ImageMarginGradientEnd => Theme.Surface;
        public override Color MenuBorder => Theme.Border;
        public override Color MenuItemBorder => Theme.PrimarySoft;
        public override Color MenuItemSelected => Theme.Hover;
        public override Color MenuItemSelectedGradientBegin => Theme.Hover;
        public override Color MenuItemSelectedGradientEnd => Theme.Hover;
        public override Color MenuItemPressedGradientBegin => Theme.SurfaceAlt;
        public override Color MenuItemPressedGradientMiddle => Theme.SurfaceAlt;
        public override Color MenuItemPressedGradientEnd => Theme.SurfaceAlt;
        public override Color SeparatorDark => Theme.Border;
        public override Color SeparatorLight => Theme.Surface;
        public override Color ToolStripBorder => Theme.Border;
    }

    /// <summary>
    /// 右键菜单渲染器：统一文字颜色与圆角风格。
    /// </summary>
    public class ModernMenuRenderer : ToolStripProfessionalRenderer
    {
        public ModernMenuRenderer() : base(new ModernColorTable())
        {
            RoundedEdges = true;
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            if (!e.Item.Enabled)
                e.TextColor = Theme.DisabledText;
            else if (e.Item.ForeColor != Color.Empty && e.Item.ForeColor != SystemColors.ControlText)
                e.TextColor = e.Item.ForeColor;
            else
                e.TextColor = Theme.TextPrimary;

            base.OnRenderItemText(e);
        }
    }
}
