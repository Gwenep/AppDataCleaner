using System.Drawing;
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

            rootLayout = new TableLayoutPanel();
            headerPanel = new GradientPanel();
            logoBadge = new LogoBadge();
            lblAppTitle = new Label();
            lblAppSubtitle = new Label();
            lblVersion = new Label();
            lblUpdate = new LinkLabel();
            toolbarPanel = new Panel();
            btnScan = new ModernButton();
            btnCancel = new ModernButton();
            btnOpen = new ModernButton();
            btnNote = new ModernButton();
            btnRecycle = new ModernButton();
            btnDelete = new ModernButton();
            mainLayout = new TableLayoutPanel();
            sidebarCard = new RoundedPanel();
            sidebarLayout = new TableLayoutPanel();
            lblScopeTitle = new Label();
            lblScopeHint = new Label();
            checkedListBox1 = new ModernCheckedListBox();
            btnSelectAll = new ModernButton();
            contentLayout = new TableLayoutPanel();
            statsLayout = new TableLayoutPanel();
            statItems = new StatCard();
            statSize = new StatCard();
            statSelected = new StatCard();
            gridCard = new RoundedPanel();
            gridHost = new Panel();
            dataGridView1 = new ModernDataGridView();
            lblEmpty = new Label();
            gridHeader = new Panel();
            lblResults = new Label();
            lblResultHint = new Label();
            searchBox = new SearchBox();
            footerPanel = new Panel();
            progressBar1 = new ModernProgressBar();
            labelStatus = new Label();
            labelCount = new Label();
            labelAdminWarning = new Label();
            contextMenuStrip1 = new ContextMenuStrip(components);
            ctxOpen = new ToolStripMenuItem();
            ctxCopyPath = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            ctxRecycle = new ToolStripMenuItem();
            ctxDelete = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            ctxNote = new ToolStripMenuItem();
            toolTip1 = new ToolTip(components);

            rootLayout.SuspendLayout();
            headerPanel.SuspendLayout();
            toolbarPanel.SuspendLayout();
            mainLayout.SuspendLayout();
            sidebarCard.SuspendLayout();
            sidebarLayout.SuspendLayout();
            contentLayout.SuspendLayout();
            statsLayout.SuspendLayout();
            gridCard.SuspendLayout();
            gridHost.SuspendLayout();
            gridHeader.SuspendLayout();
            footerPanel.SuspendLayout();
            contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();

            //
            // rootLayout
            //
            rootLayout.BackColor = Theme.Background;
            rootLayout.ColumnCount = 1;
            rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            rootLayout.Controls.Add(headerPanel, 0, 0);
            rootLayout.Controls.Add(toolbarPanel, 0, 1);
            rootLayout.Controls.Add(mainLayout, 0, 2);
            rootLayout.Controls.Add(footerPanel, 0, 3);
            rootLayout.Dock = DockStyle.Fill;
            rootLayout.Location = new Point(0, 0);
            rootLayout.Margin = new Padding(0);
            rootLayout.Name = "rootLayout";
            rootLayout.RowCount = 4;
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 96F));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 68F));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 72F));
            rootLayout.Size = new Size(1200, 780);
            rootLayout.TabIndex = 0;

            //
            // headerPanel
            //
            headerPanel.Controls.Add(logoBadge);
            headerPanel.Controls.Add(lblAppTitle);
            headerPanel.Controls.Add(lblAppSubtitle);
            headerPanel.Controls.Add(lblVersion);
            headerPanel.Controls.Add(lblUpdate);
            headerPanel.Dock = DockStyle.Fill;
            headerPanel.Location = new Point(0, 0);
            headerPanel.Margin = new Padding(0);
            headerPanel.Name = "headerPanel";
            headerPanel.Size = new Size(1200, 96);
            headerPanel.TabIndex = 0;

            //
            // logoBadge
            //
            logoBadge.Location = new Point(28, 24);
            logoBadge.Name = "logoBadge";
            logoBadge.Size = new Size(48, 48);
            logoBadge.TabIndex = 0;

            //
            // lblAppTitle
            //
            lblAppTitle.AutoSize = true;
            lblAppTitle.BackColor = Color.Transparent;
            lblAppTitle.Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Bold);
            lblAppTitle.ForeColor = Color.White;
            lblAppTitle.Location = new Point(92, 20);
            lblAppTitle.Name = "lblAppTitle";
            lblAppTitle.Size = new Size(210, 31);
            lblAppTitle.TabIndex = 1;
            lblAppTitle.Text = "应用数据清理工具";

            //
            // lblAppSubtitle
            //
            lblAppSubtitle.AutoSize = true;
            lblAppSubtitle.BackColor = Color.Transparent;
            lblAppSubtitle.Font = new Font("Microsoft YaHei UI", 9F);
            lblAppSubtitle.ForeColor = Color.FromArgb(203, 213, 225);
            lblAppSubtitle.Location = new Point(94, 58);
            lblAppSubtitle.Name = "lblAppSubtitle";
            lblAppSubtitle.Size = new Size(300, 17);
            lblAppSubtitle.TabIndex = 2;
            lblAppSubtitle.Text = "AppData Cleaner · 安全扫描并清理应用残留数据";

            //
            // lblVersion
            //
            lblVersion.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblVersion.AutoSize = true;
            lblVersion.BackColor = Color.Transparent;
            lblVersion.Font = new Font("Microsoft YaHei UI", 9F);
            lblVersion.ForeColor = Color.FromArgb(203, 213, 225);
            lblVersion.Location = new Point(1128, 32);
            lblVersion.Name = "lblVersion";
            lblVersion.Size = new Size(44, 17);
            lblVersion.TabIndex = 3;
            lblVersion.Text = "v0.3.0";
            lblVersion.TextAlign = ContentAlignment.TopRight;
            lblVersion.Cursor = Cursors.Hand;
            lblVersion.Click += lblVersion_Click;

            //
            // lblUpdate
            //
            lblUpdate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblUpdate.AutoSize = true;
            lblUpdate.BackColor = Color.Transparent;
            lblUpdate.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            lblUpdate.LinkBehavior = LinkBehavior.HoverUnderline;
            lblUpdate.LinkColor = Color.FromArgb(191, 219, 254);
            lblUpdate.ActiveLinkColor = Color.White;
            lblUpdate.VisitedLinkColor = Color.FromArgb(191, 219, 254);
            lblUpdate.Location = new Point(960, 32);
            lblUpdate.Name = "lblUpdate";
            lblUpdate.Size = new Size(0, 17);
            lblUpdate.TabIndex = 4;
            lblUpdate.Text = "";
            lblUpdate.Visible = false;
            lblUpdate.LinkClicked += lblUpdate_LinkClicked;

            //
            // toolbarPanel
            //
            toolbarPanel.BackColor = Theme.Background;
            toolbarPanel.Controls.Add(btnScan);
            toolbarPanel.Controls.Add(btnCancel);
            toolbarPanel.Controls.Add(btnOpen);
            toolbarPanel.Controls.Add(btnNote);
            toolbarPanel.Controls.Add(btnRecycle);
            toolbarPanel.Controls.Add(btnDelete);
            toolbarPanel.Dock = DockStyle.Fill;
            toolbarPanel.Location = new Point(0, 96);
            toolbarPanel.Margin = new Padding(0);
            toolbarPanel.Name = "toolbarPanel";
            toolbarPanel.Size = new Size(1200, 68);
            toolbarPanel.TabIndex = 1;

            //
            // btnScan
            //
            btnScan.Location = new Point(24, 14);
            btnScan.Name = "btnScan";
            btnScan.Size = new Size(124, 40);
            btnScan.TabIndex = 0;
            btnScan.Text = "开始扫描";
            btnScan.Variant = ButtonVariant.Primary;
            btnScan.Click += btnScan_Click;

            //
            // btnCancel
            //
            btnCancel.Enabled = false;
            btnCancel.Location = new Point(158, 14);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(112, 40);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "取消扫描";
            btnCancel.Click += btnCancel_Click;

            //
            // btnOpen
            //
            btnOpen.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOpen.Location = new Point(710, 14);
            btnOpen.Name = "btnOpen";
            btnOpen.Size = new Size(108, 40);
            btnOpen.TabIndex = 2;
            btnOpen.Text = "打开位置";
            btnOpen.Click += btnOpen_Click;

            //
            // btnNote
            //
            btnNote.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnNote.Location = new Point(826, 14);
            btnNote.Name = "btnNote";
            btnNote.Size = new Size(90, 40);
            btnNote.TabIndex = 3;
            btnNote.Text = "编辑备注";
            btnNote.Click += btnNote_Click;

            //
            // btnRecycle
            //
            btnRecycle.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRecycle.Location = new Point(924, 14);
            btnRecycle.Name = "btnRecycle";
            btnRecycle.Size = new Size(126, 40);
            btnRecycle.TabIndex = 4;
            btnRecycle.Text = "移至回收站";
            btnRecycle.Variant = ButtonVariant.Warning;
            btnRecycle.Click += btnRecycle_Click;

            //
            // btnDelete
            //
            btnDelete.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDelete.Location = new Point(1058, 14);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(118, 40);
            btnDelete.TabIndex = 5;
            btnDelete.Text = "永久删除";
            btnDelete.Variant = ButtonVariant.Danger;
            btnDelete.Click += btnDelete_Click;

            //
            // mainLayout
            //
            mainLayout.BackColor = Theme.Background;
            mainLayout.ColumnCount = 2;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 280F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainLayout.Controls.Add(sidebarCard, 0, 0);
            mainLayout.Controls.Add(contentLayout, 1, 0);
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.Location = new Point(0, 164);
            mainLayout.Margin = new Padding(0);
            mainLayout.Name = "mainLayout";
            mainLayout.Padding = new Padding(24, 4, 24, 4);
            mainLayout.RowCount = 1;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainLayout.Size = new Size(1200, 544);
            mainLayout.TabIndex = 2;

            //
            // sidebarCard
            //
            sidebarCard.Controls.Add(sidebarLayout);
            sidebarCard.CornerRadius = 14;
            sidebarCard.Dock = DockStyle.Fill;
            sidebarCard.Location = new Point(24, 4);
            sidebarCard.Margin = new Padding(0, 0, 16, 0);
            sidebarCard.Name = "sidebarCard";
            sidebarCard.Padding = new Padding(20, 18, 20, 18);
            sidebarCard.Size = new Size(264, 536);
            sidebarCard.TabIndex = 0;

            //
            // sidebarLayout
            //
            sidebarLayout.BackColor = Theme.Surface;
            sidebarLayout.ColumnCount = 1;
            sidebarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            sidebarLayout.Controls.Add(lblScopeTitle, 0, 0);
            sidebarLayout.Controls.Add(lblScopeHint, 0, 1);
            sidebarLayout.Controls.Add(checkedListBox1, 0, 2);
            sidebarLayout.Controls.Add(btnSelectAll, 0, 3);
            sidebarLayout.Dock = DockStyle.Fill;
            sidebarLayout.Location = new Point(20, 18);
            sidebarLayout.Margin = new Padding(0);
            sidebarLayout.Name = "sidebarLayout";
            sidebarLayout.RowCount = 4;
            sidebarLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            sidebarLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            sidebarLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            sidebarLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            sidebarLayout.Size = new Size(224, 500);
            sidebarLayout.TabIndex = 0;

            //
            // lblScopeTitle
            //
            lblScopeTitle.AutoSize = true;
            lblScopeTitle.Dock = DockStyle.Fill;
            lblScopeTitle.Font = new Font("Microsoft YaHei UI", 10.5F, FontStyle.Bold);
            lblScopeTitle.ForeColor = Theme.TextPrimary;
            lblScopeTitle.Margin = new Padding(0, 0, 0, 2);
            lblScopeTitle.Name = "lblScopeTitle";
            lblScopeTitle.Size = new Size(224, 19);
            lblScopeTitle.TabIndex = 0;
            lblScopeTitle.Text = "扫描范围";

            //
            // lblScopeHint
            //
            lblScopeHint.AutoSize = true;
            lblScopeHint.Dock = DockStyle.Fill;
            lblScopeHint.Font = new Font("Microsoft YaHei UI", 8.5F);
            lblScopeHint.ForeColor = Theme.TextSecondary;
            lblScopeHint.Margin = new Padding(0, 0, 0, 10);
            lblScopeHint.Name = "lblScopeHint";
            lblScopeHint.Size = new Size(224, 17);
            lblScopeHint.TabIndex = 1;
            lblScopeHint.Text = "勾选需要扫描的 AppData 目录";

            //
            // checkedListBox1
            //
            checkedListBox1.Dock = DockStyle.Fill;
            checkedListBox1.Items.AddRange(new object[] { "Local", "LocalLow", "Roaming", "ProgramData" });
            checkedListBox1.Location = new Point(0, 48);
            checkedListBox1.Margin = new Padding(0);
            checkedListBox1.Name = "checkedListBox1";
            checkedListBox1.Size = new Size(224, 414);
            checkedListBox1.TabIndex = 2;
            checkedListBox1.ItemCheck += checkedListBox1_ItemCheck;

            //
            // btnSelectAll
            //
            btnSelectAll.Dock = DockStyle.Fill;
            btnSelectAll.Location = new Point(0, 462);
            btnSelectAll.Margin = new Padding(0, 12, 0, 0);
            btnSelectAll.Name = "btnSelectAll";
            btnSelectAll.Size = new Size(224, 38);
            btnSelectAll.TabIndex = 3;
            btnSelectAll.Text = "全选";
            btnSelectAll.Click += btnSelectAll_Click;

            //
            // contentLayout
            //
            contentLayout.BackColor = Theme.Background;
            contentLayout.ColumnCount = 1;
            contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            contentLayout.Controls.Add(statsLayout, 0, 0);
            contentLayout.Controls.Add(gridCard, 0, 1);
            contentLayout.Dock = DockStyle.Fill;
            contentLayout.Location = new Point(304, 4);
            contentLayout.Margin = new Padding(0);
            contentLayout.Name = "contentLayout";
            contentLayout.RowCount = 2;
            contentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 104F));
            contentLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            contentLayout.Size = new Size(872, 536);
            contentLayout.TabIndex = 1;

            //
            // statsLayout
            //
            statsLayout.BackColor = Theme.Background;
            statsLayout.ColumnCount = 3;
            statsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            statsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            statsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));
            statsLayout.Controls.Add(statItems, 0, 0);
            statsLayout.Controls.Add(statSize, 1, 0);
            statsLayout.Controls.Add(statSelected, 2, 0);
            statsLayout.Dock = DockStyle.Fill;
            statsLayout.Location = new Point(0, 0);
            statsLayout.Margin = new Padding(0);
            statsLayout.Name = "statsLayout";
            statsLayout.RowCount = 1;
            statsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            statsLayout.Size = new Size(872, 104);
            statsLayout.TabIndex = 0;

            //
            // statItems
            //
            statItems.AccentColor = Theme.Primary;
            statItems.Caption = "扫描项目";
            statItems.Dock = DockStyle.Fill;
            statItems.Location = new Point(0, 0);
            statItems.Margin = new Padding(0, 0, 12, 0);
            statItems.Name = "statItems";
            statItems.Size = new Size(279, 104);
            statItems.TabIndex = 0;
            statItems.Value = "0 项";

            //
            // statSize
            //
            statSize.AccentColor = Theme.Accent;
            statSize.Caption = "总占用空间";
            statSize.Dock = DockStyle.Fill;
            statSize.Location = new Point(291, 0);
            statSize.Margin = new Padding(0, 0, 12, 0);
            statSize.Name = "statSize";
            statSize.Size = new Size(279, 104);
            statSize.TabIndex = 1;
            statSize.Value = "0 B";

            //
            // statSelected
            //
            statSelected.AccentColor = Theme.Success;
            statSelected.Caption = "当前选择";
            statSelected.Dock = DockStyle.Fill;
            statSelected.Location = new Point(582, 0);
            statSelected.Margin = new Padding(0);
            statSelected.Name = "statSelected";
            statSelected.Size = new Size(290, 104);
            statSelected.TabIndex = 2;
            statSelected.Value = "未选择";

            //
            // gridCard
            //
            gridCard.Controls.Add(gridHost);
            gridCard.Controls.Add(gridHeader);
            gridCard.CornerRadius = 14;
            gridCard.Dock = DockStyle.Fill;
            gridCard.Location = new Point(0, 116);
            gridCard.Margin = new Padding(0, 12, 0, 0);
            gridCard.Name = "gridCard";
            gridCard.Size = new Size(872, 420);
            gridCard.TabIndex = 1;

            //
            // gridHost
            //
            gridHost.BackColor = Theme.Surface;
            gridHost.Controls.Add(dataGridView1);
            gridHost.Controls.Add(lblEmpty);
            gridHost.Dock = DockStyle.Fill;
            gridHost.Location = new Point(0, 58);
            gridHost.Margin = new Padding(0);
            gridHost.Name = "gridHost";
            gridHost.Padding = new Padding(12, 0, 12, 12);
            gridHost.Size = new Size(872, 362);
            gridHost.TabIndex = 1;

            //
            // dataGridView1
            //
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Theme.SurfaceAlt
            };
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGridView1.BackgroundColor = Theme.Surface;
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridView1.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                BackColor = Theme.SurfaceAlt,
                ForeColor = Theme.TextSecondary,
                SelectionBackColor = Theme.SurfaceAlt,
                SelectionForeColor = Theme.TextSecondary,
                Padding = new Padding(12, 0, 12, 0),
                Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold),
                WrapMode = DataGridViewTriState.False
            };
            dataGridView1.ColumnHeadersHeight = 42;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView1.DefaultCellStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                BackColor = Theme.Surface,
                ForeColor = Theme.TextPrimary,
                SelectionBackColor = Theme.Selection,
                SelectionForeColor = Theme.SelectionText,
                Padding = new Padding(12, 0, 12, 0),
                Font = new Font("Microsoft YaHei UI", 9F),
                WrapMode = DataGridViewTriState.False
            };
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.GridColor = Theme.Border;
            dataGridView1.Location = new Point(12, 0);
            dataGridView1.Margin = new Padding(0);
            dataGridView1.MultiSelect = true;
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.RowTemplate.Height = 38;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ShowCellToolTips = true;
            dataGridView1.Size = new Size(848, 350);
            dataGridView1.TabIndex = 0;
            dataGridView1.CellDoubleClick += dataGridView1_CellDoubleClick;
            dataGridView1.CellMouseClick += dataGridView1_CellMouseClick;
            dataGridView1.CellToolTipTextNeeded += dataGridView1_CellToolTipTextNeeded;
            dataGridView1.ColumnHeaderMouseClick += dataGridView1_ColumnHeaderMouseClick;
            dataGridView1.KeyDown += dataGridView1_KeyDown;
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;

            //
            // lblEmpty
            //
            lblEmpty.BackColor = Theme.Surface;
            lblEmpty.Dock = DockStyle.Fill;
            lblEmpty.Font = new Font("Microsoft YaHei UI", 10F);
            lblEmpty.ForeColor = Theme.TextMuted;
            lblEmpty.Location = new Point(12, 0);
            lblEmpty.Name = "lblEmpty";
            lblEmpty.Size = new Size(848, 350);
            lblEmpty.TabIndex = 1;
            lblEmpty.Text = "尚未扫描任何内容\r\n\r\n点击「开始扫描」查看 AppData 目录占用情况";
            lblEmpty.TextAlign = ContentAlignment.MiddleCenter;

            //
            // gridHeader
            //
            gridHeader.BackColor = Theme.Surface;
            gridHeader.Controls.Add(lblResults);
            gridHeader.Controls.Add(lblResultHint);
            gridHeader.Controls.Add(searchBox);
            gridHeader.Dock = DockStyle.Top;
            gridHeader.Location = new Point(0, 0);
            gridHeader.Margin = new Padding(0);
            gridHeader.Name = "gridHeader";
            gridHeader.Size = new Size(872, 58);
            gridHeader.TabIndex = 0;

            //
            // lblResults
            //
            lblResults.AutoSize = true;
            lblResults.Font = new Font("Microsoft YaHei UI", 10.5F, FontStyle.Bold);
            lblResults.ForeColor = Theme.TextPrimary;
            lblResults.Location = new Point(18, 12);
            lblResults.Name = "lblResults";
            lblResults.Size = new Size(72, 19);
            lblResults.TabIndex = 0;
            lblResults.Text = "扫描结果";

            //
            // lblResultHint
            //
            lblResultHint.AutoSize = true;
            lblResultHint.Font = new Font("Microsoft YaHei UI", 8.5F);
            lblResultHint.ForeColor = Theme.TextMuted;
            lblResultHint.Location = new Point(18, 35);
            lblResultHint.Name = "lblResultHint";
            lblResultHint.Size = new Size(170, 17);
            lblResultHint.TabIndex = 1;
            lblResultHint.Text = "双击行可直接打开所在位置";

            //
            // searchBox
            //
            searchBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            searchBox.Location = new Point(594, 10);
            searchBox.Name = "searchBox";
            searchBox.PlaceholderText = "搜索路径或备注...";
            searchBox.Size = new Size(260, 38);
            searchBox.TabIndex = 2;
            searchBox.TextChanged += searchBox_TextChanged;

            //
            // footerPanel
            //
            footerPanel.BackColor = Theme.Background;
            footerPanel.Controls.Add(progressBar1);
            footerPanel.Controls.Add(labelStatus);
            footerPanel.Controls.Add(labelCount);
            footerPanel.Controls.Add(labelAdminWarning);
            footerPanel.Dock = DockStyle.Fill;
            footerPanel.Location = new Point(0, 708);
            footerPanel.Margin = new Padding(0);
            footerPanel.Name = "footerPanel";
            footerPanel.Size = new Size(1200, 72);
            footerPanel.TabIndex = 3;

            //
            // progressBar1
            //
            progressBar1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            progressBar1.Location = new Point(24, 10);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(1152, 8);
            progressBar1.TabIndex = 0;

            //
            // labelStatus
            //
            labelStatus.AutoSize = true;
            labelStatus.Font = new Font("Microsoft YaHei UI", 9F);
            labelStatus.ForeColor = Theme.TextSecondary;
            labelStatus.Location = new Point(24, 28);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new Size(35, 17);
            labelStatus.TabIndex = 1;
            labelStatus.Text = "就绪";

            //
            // labelCount
            //
            labelCount.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelCount.AutoSize = true;
            labelCount.Font = new Font("Microsoft YaHei UI", 9F);
            labelCount.ForeColor = Theme.TextSecondary;
            labelCount.Location = new Point(1110, 28);
            labelCount.Name = "labelCount";
            labelCount.Size = new Size(35, 17);
            labelCount.TabIndex = 2;
            labelCount.Text = "0 项";
            labelCount.TextAlign = ContentAlignment.TopRight;

            //
            // labelAdminWarning
            //
            labelAdminWarning.AutoSize = true;
            labelAdminWarning.Font = new Font("Microsoft YaHei UI", 8.5F);
            labelAdminWarning.ForeColor = Theme.WarningText;
            labelAdminWarning.Location = new Point(24, 48);
            labelAdminWarning.Name = "labelAdminWarning";
            labelAdminWarning.Size = new Size(230, 17);
            labelAdminWarning.TabIndex = 3;
            labelAdminWarning.Text = "⚠ 非管理员模式，部分目录可能无法访问";
            labelAdminWarning.Visible = false;

            //
            // contextMenuStrip1
            //
            contextMenuStrip1.Font = new Font("Microsoft YaHei UI", 9F);
            contextMenuStrip1.Items.AddRange(new ToolStripItem[]
            {
                ctxOpen,
                ctxCopyPath,
                toolStripSeparator1,
                ctxRecycle,
                ctxDelete,
                toolStripSeparator2,
                ctxNote
            });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.ShowImageMargin = false;
            contextMenuStrip1.Size = new Size(180, 148);
            contextMenuStrip1.Opening += contextMenuStrip1_Opening;

            //
            // ctxOpen
            //
            ctxOpen.Name = "ctxOpen";
            ctxOpen.Padding = new Padding(8, 4, 8, 4);
            ctxOpen.Size = new Size(179, 26);
            ctxOpen.Text = "打开位置";
            ctxOpen.Click += ctxOpen_Click;

            //
            // ctxCopyPath
            //
            ctxCopyPath.Name = "ctxCopyPath";
            ctxCopyPath.Padding = new Padding(8, 4, 8, 4);
            ctxCopyPath.Size = new Size(179, 26);
            ctxCopyPath.Text = "复制路径";
            ctxCopyPath.Click += ctxCopyPath_Click;

            //
            // toolStripSeparator1
            //
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(176, 6);

            //
            // ctxRecycle
            //
            ctxRecycle.Name = "ctxRecycle";
            ctxRecycle.Padding = new Padding(8, 4, 8, 4);
            ctxRecycle.Size = new Size(179, 26);
            ctxRecycle.Text = "移至回收站";
            ctxRecycle.Click += ctxRecycle_Click;

            //
            // ctxDelete
            //
            ctxDelete.ForeColor = Theme.Danger;
            ctxDelete.Name = "ctxDelete";
            ctxDelete.Padding = new Padding(8, 4, 8, 4);
            ctxDelete.Size = new Size(179, 26);
            ctxDelete.Text = "永久删除";
            ctxDelete.Click += ctxDelete_Click;

            //
            // toolStripSeparator2
            //
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(176, 6);

            //
            // ctxNote
            //
            ctxNote.Name = "ctxNote";
            ctxNote.Padding = new Padding(8, 4, 8, 4);
            ctxNote.Size = new Size(179, 26);
            ctxNote.Text = "编辑备注";
            ctxNote.Click += ctxNote_Click;

            //
            // toolTip1
            //
            toolTip1.AutoPopDelay = 8000;
            toolTip1.InitialDelay = 350;
            toolTip1.ReshowDelay = 100;
            toolTip1.SetToolTip(btnScan, "扫描所选目录 (F5)");
            toolTip1.SetToolTip(btnCancel, "停止当前扫描 (Esc)");
            toolTip1.SetToolTip(btnOpen, "在资源管理器中打开 (Ctrl+O)");
            toolTip1.SetToolTip(btnNote, "为选中项添加或修改备注");
            toolTip1.SetToolTip(btnRecycle, "移动到回收站，之后仍可恢复");
            toolTip1.SetToolTip(btnDelete, "永久删除，无法恢复");
            toolTip1.SetToolTip(searchBox, "按路径或备注筛选结果 (Ctrl+F)");
            toolTip1.SetToolTip(checkedListBox1, "选择需要扫描的 AppData 子目录");
            toolTip1.SetToolTip(lblVersion, "点击检查更新");

            //
            // Form1
            //
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Theme.Background;
            ClientSize = new Size(1200, 780);
            Controls.Add(rootLayout);
            Font = new Font("Microsoft YaHei UI", 9F);
            KeyPreview = true;
            MinimumSize = new Size(980, 640);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "应用数据清理工具";
            FormClosing += Form1_FormClosing;
            KeyDown += Form1_KeyDown;
            Shown += Form1_Shown;

            rootLayout.ResumeLayout(false);
            headerPanel.ResumeLayout(false);
            headerPanel.PerformLayout();
            toolbarPanel.ResumeLayout(false);
            mainLayout.ResumeLayout(false);
            sidebarCard.ResumeLayout(false);
            sidebarLayout.ResumeLayout(false);
            sidebarLayout.PerformLayout();
            contentLayout.ResumeLayout(false);
            statsLayout.ResumeLayout(false);
            gridCard.ResumeLayout(false);
            gridHost.ResumeLayout(false);
            gridHeader.ResumeLayout(false);
            gridHeader.PerformLayout();
            footerPanel.ResumeLayout(false);
            footerPanel.PerformLayout();
            contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel rootLayout;
        private GradientPanel headerPanel;
        private LogoBadge logoBadge;
        private Label lblAppTitle;
        private Label lblAppSubtitle;
        private Label lblVersion;
        private LinkLabel lblUpdate;
        private Panel toolbarPanel;
        private ModernButton btnScan;
        private ModernButton btnCancel;
        private ModernButton btnOpen;
        private ModernButton btnNote;
        private ModernButton btnRecycle;
        private ModernButton btnDelete;
        private TableLayoutPanel mainLayout;
        private RoundedPanel sidebarCard;
        private TableLayoutPanel sidebarLayout;
        private Label lblScopeTitle;
        private Label lblScopeHint;
        private ModernCheckedListBox checkedListBox1;
        private ModernButton btnSelectAll;
        private TableLayoutPanel contentLayout;
        private TableLayoutPanel statsLayout;
        private StatCard statItems;
        private StatCard statSize;
        private StatCard statSelected;
        private RoundedPanel gridCard;
        private Panel gridHost;
        private ModernDataGridView dataGridView1;
        private Label lblEmpty;
        private Panel gridHeader;
        private Label lblResults;
        private Label lblResultHint;
        private SearchBox searchBox;
        private Panel footerPanel;
        private ModernProgressBar progressBar1;
        private Label labelStatus;
        private Label labelCount;
        private Label labelAdminWarning;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem ctxOpen;
        private ToolStripMenuItem ctxCopyPath;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem ctxRecycle;
        private ToolStripMenuItem ctxDelete;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem ctxNote;
        private ToolTip toolTip1;
    }
}
