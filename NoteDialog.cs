using System;
using System.Drawing;
using System.Windows.Forms;

namespace AppDataCleaner
{
    /// <summary>
    /// 编辑文件/文件夹备注的现代化对话框。
    /// </summary>
    internal sealed class NoteDialog : Form
    {
        private readonly TextBox _textBox;

        public NoteDialog(string itemName, string itemPath, string currentNote)
        {
            Icon = AppIcon.Load();

            Text = "编辑备注";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            ClientSize = new Size(480, 286);
            BackColor = Theme.Background;
            Font = Theme.BodyFont;
            Padding = new Padding(0);

            var title = new Label
            {
                Text = "编辑备注",
                Font = new Font("Microsoft YaHei UI", 15F, FontStyle.Bold),
                ForeColor = Theme.TextPrimary,
                AutoSize = true,
                Location = new Point(24, 20),
                BackColor = Color.Transparent
            };

            var nameLabel = new Label
            {
                Text = itemName,
                Font = new Font("Microsoft YaHei UI", 9.5F, FontStyle.Bold),
                ForeColor = Theme.TextPrimary,
                AutoEllipsis = true,
                Location = new Point(24, 56),
                Size = new Size(432, 20),
                BackColor = Color.Transparent
            };

            var pathLabel = new Label
            {
                Text = itemPath,
                Font = Theme.SmallFont,
                ForeColor = Theme.TextSecondary,
                AutoEllipsis = true,
                Location = new Point(24, 78),
                Size = new Size(432, 18),
                BackColor = Color.Transparent
            };

            var editor = new RoundedPanel
            {
                Location = new Point(24, 104),
                Size = new Size(432, 118),
                CornerRadius = 10,
                BorderColor = Theme.Border,
                BackColor = Theme.Surface,
                Padding = new Padding(12, 10, 12, 10)
            };

            _textBox = new TextBox
            {
                Multiline = true,
                BorderStyle = BorderStyle.None,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Microsoft YaHei UI", 9.5F),
                ForeColor = Theme.TextPrimary,
                BackColor = Theme.Surface,
                Dock = DockStyle.Fill,
                MaxLength = 500,
                Text = currentNote ?? ""
            };

            editor.Controls.Add(_textBox);

            var hint = new Label
            {
                Text = "留空保存可清除已有备注",
                Font = Theme.SmallFont,
                ForeColor = Theme.TextMuted,
                AutoSize = true,
                Location = new Point(24, 238),
                BackColor = Color.Transparent
            };

            var saveButton = new ModernButton
            {
                Text = "保存",
                Variant = ButtonVariant.Primary,
                Location = new Point(366, 234),
                Size = new Size(90, 38),
                DialogResult = DialogResult.OK
            };

            var cancelButton = new ModernButton
            {
                Text = "取消",
                Variant = ButtonVariant.Secondary,
                Location = new Point(266, 234),
                Size = new Size(90, 38),
                DialogResult = DialogResult.Cancel
            };

            AcceptButton = saveButton;
            CancelButton = cancelButton;

            Controls.Add(title);
            Controls.Add(nameLabel);
            Controls.Add(pathLabel);
            Controls.Add(editor);
            Controls.Add(hint);
            Controls.Add(saveButton);
            Controls.Add(cancelButton);

            Shown += (_, _) =>
            {
                _textBox.Focus();
                _textBox.SelectAll();
            };
        }

        /// <summary>用户输入的备注内容（已去除首尾空白）。</summary>
        public string Note => _textBox.Text.Trim();
    }
}
