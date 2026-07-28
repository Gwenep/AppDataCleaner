using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading;
using System.Windows.Forms;

namespace AppDataCleaner
{
    public partial class Form1 : Form
    {
        // ---- 列索引常量 ----
        private const int COL_PATH = 0;
        private const int COL_SIZE = 1;
        private const int COL_DESC = 2;

        // ---- 状态 ----
        private readonly Dictionary<string, string> _notes = new();
        private readonly string _notesFilePath;

        // 缓存原始大小，用于排序
        private readonly Dictionary<string, long> _sizeCache = new();

        // 当前正在扫描的 CancellationTokenSource
        private CancellationTokenSource? _cts;

        // 数据源（跨排序/搜索共享）
        private BindingList<ScanResultItem> _items = new();
        private readonly BindingSource _bindingSource = new();

        // 搜索条件
        private string _searchFilter = "";

        // 是否正在扫描
        private bool _isScanning;

        public Form1()
        {
            // 备注文件路径，在 exe 所在目录（单文件发布时也正确）
            string? exeDir = Path.GetDirectoryName(Environment.ProcessPath);
            _notesFilePath = Path.Combine(exeDir ?? ".", "appname.json");

            InitializeComponent();
            SetupColumns();
            SetupDataBinding();
            LoadNotes();
            UpdateSelectAllButton();
            CheckAdminStatus();
        }

        // ================================================================
        //  手动配置列（不依赖 AutoGenerateColumns）
        // ================================================================

        private void SetupColumns()
        {
            dataGridView1.Columns.Clear();

            // 路径列
            var pathCol = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(ScanResultItem.Path),
                HeaderText = "路径",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 180,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.Programmatic
            };
            dataGridView1.Columns.Add(pathCol);

            // 大小列
            var sizeCol = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(ScanResultItem.SizeFormatted),
                HeaderText = "大小",
                Width = 100,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.Programmatic
            };
            dataGridView1.Columns.Add(sizeCol);

            // 描述列
            var descCol = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(ScanResultItem.Description),
                HeaderText = "描述",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 100,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.Programmatic
            };
            dataGridView1.Columns.Add(descCol);
        }

        // ================================================================
        //  数据绑定
        // ================================================================

        private void SetupDataBinding()
        {
            _bindingSource.DataSource = _items;
            dataGridView1.DataSource = _bindingSource;
        }

        // ================================================================
        //  备注文件操作（System.Text.Json）
        // ================================================================

        private void LoadNotes()
        {
            try
            {
                if (File.Exists(_notesFilePath))
                {
                    string json = File.ReadAllText(_notesFilePath, System.Text.Encoding.UTF8);
                    _notes.Clear();
                    var deserialized = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                    if (deserialized != null)
                    {
                        foreach (var kv in deserialized)
                            _notes[kv.Key] = kv.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error("加载备注文件失败", ex);
            }
        }

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        private void SaveNotes()
        {
            try
            {
                string json = JsonSerializer.Serialize(_notes, _jsonOptions);
                File.WriteAllText(_notesFilePath, json, System.Text.Encoding.UTF8);
            }
            catch (Exception ex)
            {
                AppLogger.Error("保存备注文件失败", ex);
                MessageBox.Show($"保存备注失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 扫描完成后，把扫描到的所有项都写入 appname.json，
        /// 已有备注的保留，没有的写入空字符串。
        /// 保存前会重新读取磁盘上的现有文件，确保不丢失任何条目。
        /// </summary>
        private void SaveAllItemsToNotes()
        {
            bool changed = false;

            // 重新读取磁盘上的现有文件，与内存中的 _notes 合并
            // 防止启动时加载失败导致已有条目丢失
            try
            {
                if (File.Exists(_notesFilePath))
                {
                    string json = File.ReadAllText(_notesFilePath, System.Text.Encoding.UTF8);
                    var existing = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                    if (existing != null)
                    {
                        foreach (var kv in existing)
                        {
                            if (!_notes.ContainsKey(kv.Key))
                            {
                                _notes[kv.Key] = kv.Value;
                                changed = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppLogger.Warn($"合并现有备注文件失败，继续使用内存数据: {ex.Message}");
            }

            foreach (var item in _items)
            {
                string name = Path.GetFileName(item.Path);
                if (!_notes.ContainsKey(name))
                {
                    _notes[name] = "";
                    changed = true;
                }
            }
            if (changed)
            {
                SaveNotes();
            }
        }

        // ================================================================
        //  管理员检测
        // ================================================================

        private void CheckAdminStatus()
        {
            if (!FileSystemUtils.IsAdministrator())
            {
                // 非管理员，显示提示但不阻止使用
                labelAdminWarning.Visible = true;
                AppLogger.Info("程序以非管理员身份运行，部分目录可能无法访问。");
            }
        }

        // ================================================================
        //  扫描
        // ================================================================

        private async void ScanFoldersAsync()
        {
            if (_isScanning) return;

            // 获取选中的目录
            var checkedNames = checkedListBox1.CheckedItems.Cast<string>().ToList();
            if (checkedNames.Count == 0)
            {
                MessageBox.Show("请至少勾选一个要扫描的目录。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _isScanning = true;
            UpdateButtonStates();

            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            // 清空之前的结果
            _items.Clear();
            _sizeCache.Clear();
            labelCount.Text = "0 项";

            // 进度
            var progress = new Progress<int>(v => progressBar1.Value = Math.Min(v, 100));
            var status = new Progress<string>(s => labelStatus.Text = s);

            var service = new ScanService(_notes);

            try
            {
                await Task.Run(() => service.Scan(
                    checkedNames,
                    progress,
                    status,
                    token,
                    batch =>
                    {
                        // 批量更新（UI 线程安全）
                        if (dataGridView1.IsHandleCreated)
                        {
                            // 使用 Invoke 而不是 BeginInvoke，确保批量更新按顺序执行
                            dataGridView1.Invoke((MethodInvoker)delegate
                            {
                                foreach (var item in batch)
                                {
                                    _items.Add(item);
                                    _sizeCache[item.Path] = item.SizeRaw;
                                }
                                labelCount.Text = $"{_items.Count} 项";
                                // 强制刷新 UI
                                Application.DoEvents();
                            });
                        }
                    },
                    batchSize: 50));

                // 扫描完成后按大小排序
                SortBySizeDescending();

                // 将所有扫描到的项写入备注文件（不匹配的也写入空描述）
                SaveAllItemsToNotes();
                labelStatus.Text = "扫描完成。";
            }
            catch (OperationCanceledException)
            {
                labelStatus.Text = "扫描已取消。";
                AppLogger.Info("用户取消了扫描。");
            }
            catch (Exception ex)
            {
                AppLogger.Error("扫描过程中发生错误", ex);
                MessageBox.Show($"扫描过程中发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                labelStatus.Text = "扫描出错。";
            }
            finally
            {
                _isScanning = false;
                _cts = null;
                UpdateButtonStates();
                progressBar1.Value = 100;
                labelCount.Text = $"{_items.Count} 项";
            }
        }

        // ================================================================
        //  排序
        // ================================================================

        private void SortBySizeDescending()
        {
            var sorted = _items.OrderByDescending(i => i.SizeRaw).ToList();
            _items.Clear();
            foreach (var item in sorted)
                _items.Add(item);
        }

        // ================================================================
        //  搜索 / 筛选
        // ================================================================

        private void ApplyFilter()
        {
            string filter = _searchFilter.Trim().ToLowerInvariant();

            if (string.IsNullOrEmpty(filter))
            {
                _bindingSource.DataSource = _items;
            }
            else
            {
                var filtered = _items.Where(i =>
                    i.Path.ToLowerInvariant().Contains(filter) ||
                    i.Description.ToLowerInvariant().Contains(filter)).ToList();
                _bindingSource.DataSource = new BindingList<ScanResultItem>(filtered);
            }

            labelCount.Text = $"{_bindingSource.Count} 项";
        }

        // ================================================================
        //  删除
        // ================================================================

        private void DeleteSelected(bool useRecycleBin)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("请先选择要删除的文件或文件夹。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 收集选中路径
            var pathsToDelete = new List<string>();
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                if (row.Cells[COL_PATH].Value is string path)
                    pathsToDelete.Add(path);
            }

            if (pathsToDelete.Count == 0) return;

            // 确认对话框
            string msg = useRecycleBin
                ? $"确认将以下 {pathsToDelete.Count} 项移动到回收站吗？\n\n删除后可能导致程序配置丢失、游戏存档丢失等。\n\n{string.Join("\n", pathsToDelete.Take(10))}{(pathsToDelete.Count > 10 ? $"\n...等共 {pathsToDelete.Count} 项" : "")}"
                : $"确认永久删除以下 {pathsToDelete.Count} 项吗？\n\n删除后无法恢复！\n\n{string.Join("\n", pathsToDelete.Take(10))}{(pathsToDelete.Count > 10 ? $"\n...等共 {pathsToDelete.Count} 项" : "")}";

            DialogResult result = MessageBox.Show(
                msg,
                useRecycleBin ? "确认移动到回收站" : "确认永久删除",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes) return;

            int successCount = 0;
            int failCount = 0;

            foreach (string path in pathsToDelete)
            {
                try
                {
                    bool ok;
                    if (useRecycleBin)
                        ok = FileSystemUtils.SendToRecycleBin(path);
                    else
                        ok = FileSystemUtils.ForceDelete(path);

                    if (ok)
                    {
                        successCount++;
                        // 从缓存中移除
                        _sizeCache.Remove(path);
                        // 从数据源中移除
                        var item = _items.FirstOrDefault(i => i.Path == path);
                        if (item != null)
                            _items.Remove(item);
                    }
                    else
                    {
                        failCount++;
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    failCount++;
                    MessageBox.Show($"访问被拒绝: {path}\n\n请尝试以管理员身份运行。", "权限不足", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    failCount++;
                    AppLogger.Error($"删除失败: {path}", ex);
                    MessageBox.Show($"删除失败: {path}\n{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            string summary = $"删除完成。成功: {successCount}, 失败: {failCount}";
            labelStatus.Text = summary;
            AppLogger.Info(summary);
            UpdateCountLabel();
        }

        // ================================================================
        //  选择所有/取消选择所有（CheckedListBox）
        // ================================================================

        private void ToggleSelectAll()
        {
            bool allChecked = checkedListBox1.Items.Cast<object>().All(i => checkedListBox1.CheckedItems.Contains(i));
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
                checkedListBox1.SetItemChecked(i, !allChecked);
            UpdateSelectAllButton();
        }

        private void UpdateSelectAllButton()
        {
            bool allChecked = checkedListBox1.Items.Count > 0 &&
                checkedListBox1.Items.Cast<object>().All(i => checkedListBox1.CheckedItems.Contains(i));
            btnSelectAll.Text = allChecked ? "全不选" : "全选";
        }

        // ================================================================
        //  UI 状态
        // ================================================================

        private void UpdateButtonStates()
        {
            btnScan.Enabled = !_isScanning;
            btnCancel.Enabled = _isScanning;
            btnDelete.Enabled = !_isScanning;
            btnRecycle.Enabled = !_isScanning;
            btnOpen.Enabled = !_isScanning;
            btnNote.Enabled = !_isScanning;
            checkedListBox1.Enabled = !_isScanning;
            textBoxSearch.Enabled = !_isScanning;
            btnSelectAll.Enabled = !_isScanning;
        }

        private void UpdateCountLabel()
        {
            labelCount.Text = $"{_bindingSource.Count} 项";
        }

        // ================================================================
        //  事件处理
        // ================================================================

        private void btnScan_Click(object sender, EventArgs e)
        {
            ScanFoldersAsync();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
                labelStatus.Text = "正在取消...";
                btnCancel.Enabled = false;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteSelected(useRecycleBin: false);
        }

        private void btnRecycle_Click(object sender, EventArgs e)
        {
            DeleteSelected(useRecycleBin: true);
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("请先选择一个文件或文件夹。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string? path = dataGridView1.SelectedRows[0].Cells[COL_PATH].Value?.ToString();
            if (path != null)
            {
                try
                {
                    System.Diagnostics.Process.Start("explorer.exe", path);
                }
                catch (Exception ex)
                {
                    AppLogger.Error($"打开路径失败: {path}", ex);
                    MessageBox.Show($"打开失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnNote_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("请先选择一个文件或文件夹。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string? path = dataGridView1.SelectedRows[0].Cells[COL_PATH].Value?.ToString();
            if (path == null) return;

            string name = Path.GetFileName(path);
            string currentNote = _notes.TryGetValue(name, out var n) ? n : "";

            string note = Microsoft.VisualBasic.Interaction.InputBox("请输入文件备注信息", "备注", currentNote);
            if (!string.IsNullOrWhiteSpace(note))
            {
                _notes[name] = note;
                SaveNotes();

                // 更新数据源中所有匹配路径的备注
                foreach (var item in _items.Where(i => Path.GetFileName(i.Path) == name))
                {
                    item.Description = note;
                }
                _bindingSource.ResetBindings(false);
            }
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            ToggleSelectAll();
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // 延迟更新按钮文字，等 ItemCheck 完成
            this.BeginInvoke(() => UpdateSelectAllButton());
        }

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            _searchFilter = textBoxSearch.Text;
            ApplyFilter();
        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == COL_SIZE)
            {
                SortBySizeDescending();
            }
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
            {
                dataGridView1.ClearSelection();
                dataGridView1.Rows[e.RowIndex].Selected = true;
                contextMenuStrip1.Show(dataGridView1, e.Location);
            }
        }

        // ================================================================
        //  右键菜单事件
        // ================================================================

        private void ctxOpen_Click(object sender, EventArgs e)
        {
            btnOpen_Click(sender, e);
        }

        private void ctxDelete_Click(object sender, EventArgs e)
        {
            DeleteSelected(useRecycleBin: false);
        }

        private void ctxRecycle_Click(object sender, EventArgs e)
        {
            DeleteSelected(useRecycleBin: true);
        }

        private void ctxNote_Click(object sender, EventArgs e)
        {
            btnNote_Click(sender, e);
        }
    }
}