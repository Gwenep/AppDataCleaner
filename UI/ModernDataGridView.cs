using System.Windows.Forms;

namespace AppDataCleaner
{
    /// <summary>
    /// 开启双缓冲的 DataGridView，减少大量行刷新时的闪烁。
    /// </summary>
    public class ModernDataGridView : DataGridView
    {
        public ModernDataGridView()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }
    }
}
