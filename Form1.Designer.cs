using System.Windows.Forms;

namespace AppDataCleaner
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            btnScan = new Button();
            btnDelete = new Button();
            btnOpen = new Button();
            btnNote = new Button();
            dataGridView1 = new DataGridView();
            progressBar1 = new ProgressBar();
            checkedListBox1 = new CheckedListBox();
            btnCancel = new Button();
            btnRecycle = new Button();
            btnSelectAll = new Button();
            textBoxSearch = new TextBox();
            labelStatus = new Label();
            labelCount = new Label();
            labelAdminWarning = new Label();
            contextMenuStrip1 = new ContextMenuStrip(components);
            ctxOpen = new ToolStripMenuItem();
            ctxRecycle = new ToolStripMenuItem();
            ctxDelete = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            ctxNote = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            //
            // btnScan
            //
            btnScan.Location = new Point(12, 12);
            btnScan.Name = "btnScan";
            btnScan.Size = new Size(90, 30);
            btnScan.TabIndex = 0;
            btnScan.Text = "开始扫描";
            btnScan.UseVisualStyleBackColor = true;
            btnScan.Click += btnScan_Click;
            //
            // btnCancel
            //
            btnCancel.Enabled = false;
            btnCancel.Location = new Point(108, 12);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 30);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "取消扫描";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            //
            // btnSelectAll
            //
            btnSelectAll.Location = new Point(12, 48);
            btnSelectAll.Name = "btnSelectAll";
            btnSelectAll.Size = new Size(104, 25);
            btnSelectAll.TabIndex = 2;
            btnSelectAll.Text = "全选";
            btnSelectAll.UseVisualStyleBackColor = true;
            btnSelectAll.Click += btnSelectAll_Click;
            //
            // btnDelete
            //
            btnDelete.Location = new Point(572, 12);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(75, 30);
            btnDelete.TabIndex = 3;
            btnDelete.Text = "永久删除";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            //
            // btnRecycle
            //
            btnRecycle.Location = new Point(653, 12);
            btnRecycle.Name = "btnRecycle";
            btnRecycle.Size = new Size(95, 30);
            btnRecycle.TabIndex = 4;
            btnRecycle.Text = "移至回收站";
            btnRecycle.UseVisualStyleBackColor = true;
            btnRecycle.Click += btnRecycle_Click;
            //
            // btnOpen
            //
            btnOpen.Location = new Point(754, 12);
            btnOpen.Name = "btnOpen";
            btnOpen.Size = new Size(75, 30);
            btnOpen.TabIndex = 5;
            btnOpen.Text = "打开位置";
            btnOpen.UseVisualStyleBackColor = true;
            btnOpen.Click += btnOpen_Click;
            //
            // btnNote
            //
            btnNote.Location = new Point(835, 12);
            btnNote.Name = "btnNote";
            btnNote.Size = new Size(75, 30);
            btnNote.TabIndex = 6;
            btnNote.Text = "备注";
            btnNote.UseVisualStyleBackColor = true;
            btnNote.Click += btnNote_Click;
            //
            // textBoxSearch
            //
            textBoxSearch.Location = new Point(692, 48);
            textBoxSearch.Name = "textBoxSearch";
            textBoxSearch.PlaceholderText = "搜索路径或备注...";
            textBoxSearch.Size = new Size(218, 25);
            textBoxSearch.TabIndex = 7;
            textBoxSearch.TextChanged += textBoxSearch_TextChanged;
            //
            // dataGridView1
            //
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.BackgroundColor = SystemColors.Window;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(132, 79);
            dataGridView1.MultiSelect = true;
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new Size(778, 400);
            dataGridView1.TabIndex = 8;
            dataGridView1.CellMouseClick += dataGridView1_CellMouseClick;
            dataGridView1.ColumnHeaderMouseClick += dataGridView1_ColumnHeaderMouseClick;
            //
            // checkedListBox1
            //
            checkedListBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            checkedListBox1.CheckOnClick = true;
            checkedListBox1.FormattingEnabled = true;
            checkedListBox1.Items.AddRange(new object[] { "Local", "LocalLow", "Roaming", "ProgramData" });
            checkedListBox1.Location = new Point(12, 79);
            checkedListBox1.Name = "checkedListBox1";
            checkedListBox1.Size = new Size(104, 400);
            checkedListBox1.TabIndex = 9;
            checkedListBox1.ItemCheck += checkedListBox1_ItemCheck;
            //
            // progressBar1
            //
            progressBar1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            progressBar1.Location = new Point(12, 485);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(898, 23);
            progressBar1.TabIndex = 10;
            //
            // labelStatus
            //
            labelStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            labelStatus.AutoSize = true;
            labelStatus.Location = new Point(12, 511);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new Size(56, 17);
            labelStatus.TabIndex = 11;
            labelStatus.Text = "就绪";
            //
            // labelCount
            //
            labelCount.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            labelCount.AutoSize = true;
            labelCount.Location = new Point(854, 511);
            labelCount.Name = "labelCount";
            labelCount.Size = new Size(56, 17);
            labelCount.TabIndex = 12;
            labelCount.Text = "0 项";
            labelCount.TextAlign = ContentAlignment.TopRight;
            //
            // labelAdminWarning
            //
            labelAdminWarning.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            labelAdminWarning.AutoSize = true;
            labelAdminWarning.ForeColor = SystemColors.GrayText;
            labelAdminWarning.Location = new Point(132, 511);
            labelAdminWarning.Name = "labelAdminWarning";
            labelAdminWarning.Size = new Size(200, 17);
            labelAdminWarning.TabIndex = 13;
            labelAdminWarning.Text = "⚠ 非管理员模式，部分目录受限";
            labelAdminWarning.Visible = false;
            //
            // contextMenuStrip1
            //
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] {
                ctxOpen,
                ctxRecycle,
                ctxDelete,
                toolStripSeparator1,
                ctxNote
            });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(149, 98);
            //
            // ctxOpen
            //
            ctxOpen.Name = "ctxOpen";
            ctxOpen.Size = new Size(148, 22);
            ctxOpen.Text = "打开位置";
            ctxOpen.Click += ctxOpen_Click;
            //
            // ctxRecycle
            //
            ctxRecycle.Name = "ctxRecycle";
            ctxRecycle.Size = new Size(148, 22);
            ctxRecycle.Text = "移至回收站";
            ctxRecycle.Click += ctxRecycle_Click;
            //
            // ctxDelete
            //
            ctxDelete.Name = "ctxDelete";
            ctxDelete.Size = new Size(148, 22);
            ctxDelete.Text = "永久删除";
            ctxDelete.Click += ctxDelete_Click;
            //
            // toolStripSeparator1
            //
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(145, 6);
            //
            // ctxNote
            //
            ctxNote.Name = "ctxNote";
            ctxNote.Size = new Size(148, 22);
            ctxNote.Text = "备注";
            ctxNote.Click += ctxNote_Click;
            //
            // Form1
            //
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(922, 537);
            Controls.Add(labelAdminWarning);
            Controls.Add(labelCount);
            Controls.Add(labelStatus);
            Controls.Add(progressBar1);
            Controls.Add(checkedListBox1);
            Controls.Add(dataGridView1);
            Controls.Add(textBoxSearch);
            Controls.Add(btnNote);
            Controls.Add(btnOpen);
            Controls.Add(btnRecycle);
            Controls.Add(btnDelete);
            Controls.Add(btnSelectAll);
            Controls.Add(btnCancel);
            Controls.Add(btnScan);
            MinimumSize = new Size(750, 450);
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "应用数据清理工具 V0.2.0";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnScan;
        private Button btnCancel;
        private Button btnSelectAll;
        private Button btnDelete;
        private Button btnRecycle;
        private Button btnOpen;
        private Button btnNote;
        private TextBox textBoxSearch;
        private DataGridView dataGridView1;
        private CheckedListBox checkedListBox1;
        private ProgressBar progressBar1;
        private Label labelStatus;
        private Label labelCount;
        private Label labelAdminWarning;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem ctxOpen;
        private ToolStripMenuItem ctxRecycle;
        private ToolStripMenuItem ctxDelete;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem ctxNote;
    }
}