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
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            dataGridView1 = new DataGridView();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
            progressBar1 = new ProgressBar();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(12, 21);
            button1.Name = "button1";
            button1.Size = new Size(177, 23);
            button1.TabIndex = 1;
            button1.Text = "开始扫描";
            button1.UseVisualStyleBackColor = true;
            button1.Click += buttonScan_Click;
            // 
            // button2
            // 
            button2.Location = new Point(478, 21);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 2;
            button2.Text = "删除";
            button2.UseVisualStyleBackColor = true;
            button2.Click += DeleteButton_Click;
            // 
            // button3
            // 
            button3.Location = new Point(559, 21);
            button3.Name = "button3";
            button3.Size = new Size(75, 23);
            button3.TabIndex = 3;
            button3.Text = "打开";
            button3.UseVisualStyleBackColor = true;
            button3.Click += OpenButton_Click;
            // 
            // button4
            // 
            button4.Location = new Point(643, 21);
            button4.Name = "button4";
            button4.Size = new Size(75, 23);
            button4.TabIndex = 4;
            button4.Text = "备注";
            button4.UseVisualStyleBackColor = true;
            button4.Click += NoteButton_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn2, dataGridViewTextBoxColumn3 });
            dataGridView1.Location = new Point(12, 60);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new Size(706, 343);
            dataGridView1.TabIndex = 5;
            dataGridViewTextBoxColumn1.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.HeaderText = "路径";
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewTextBoxColumn2.HeaderText = "大小 (字节)";
            dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            dataGridViewTextBoxColumn2.ReadOnly = true;
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewTextBoxColumn3.HeaderText = "描述";
            dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            dataGridViewTextBoxColumn3.ReadOnly = true;
            // progressBar1
            // 
            progressBar1.Location = new Point(12, 411);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(706, 23);
            progressBar1.TabIndex = 6;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(730, 442);
            Controls.Add(progressBar1);
            Controls.Add(dataGridView1);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "应用数据清理工具V0.1";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
        private DataGridView dataGridView1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private ProgressBar progressBar1;
    }
}
