using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppDataCleaner
{
    public partial class Form1 : Form
    {
        // ---- 列索引常量 ----
        private const int COL_PATH = 0;
        private const int COL_SIZE = 1;
        private const int COL_DESC = 2;

        private enum SortField
        {
            Path,
            Size,
            Description
        }

        // ---- 状态 ----
        private readonly Dictionary<string, string> _notes = new(StringComparer.OrdinalIgnoreCase);
        private readonly string _notesFilePath;

        // 完整扫描结果（不受搜索和排序影响）
        private readonly List<ScanResultItem> _items = new();

        // 当前显示在表格中的数据
        private readonly BindingList<ScanResultItem> _view = new();
        private readonly BindingSource _bindingSource = new();

        private CancellationTokenSource? _cts;
        private bool _isScanning;
        private bool _isDeleting;
        private bool _suppressSelectionUpdate;

        private string _searchFilter = "";
        private long _totalSize;

        private UpdateInfo? _updateInfo;
        private bool _isCheckingUpdate;

        private SortField _sortField = SortField.Size;
        private bool _sortAscending;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public Form1()
        {
            // 备注文件路径：优先使用 exe 同目录，单文件发布时也不会落到临时解压目录
            _notesFilePath = Path.Combine(FileSystemUtils.GetApplicationDirectory(), "appname.json");

            InitializeComponent();

            // 版本号以程序集为准，避免界面显示与实际版本不一致。
            lblVersion.Text = $"v{UpdateService.CurrentVersion}";

            var appIcon = AppIcon.Load();
            if (appIcon is not null)
                Icon = appIcon;

            DoubleBuffered = true;
            contextMenuStrip1.Renderer = new ModernMenuRenderer();

            SetupColumns();
            SetupDataBinding();
            LoadNotes();
            SelectDefaultScope();
            UpdateSelectAllButton();
            CheckAdminStatus();
            UpdateSortGlyphs();
            UpdateButtonStates();
            RefreshView();
        }

        // ================================================================
        //  表格列与数据绑定
        // ================================================================

        private void SetupColumns()
        {
            dataGridView1.Columns.Clear();

            var pathColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(ScanResultItem.Path),
                HeaderText = "路径",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 170,
                MinimumWidth = 240,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.Programmatic
            };

            var sizeColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(ScanResultItem.SizeFormatted),
                HeaderText = "大小",
                Width = 120,
                MinimumWidth = 100,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.Programmatic,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Padding = new Padding(12, 0, 16, 0)
                }
            };
            sizeColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            sizeColumn.HeaderCell.Style.Padding = new Padding(0, 0, 22, 0);

            var descriptionColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(ScanResultItem.Description),
                HeaderText = "描述 / 备注",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 130,
                MinimumWidth = 160,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.Programmatic
            };

            dataGridView1.Columns.Add(pathColumn);
            dataGridView1.Columns.Add(sizeColumn);
            dataGridView1.Columns.Add(descriptionColumn);
        }

        private void SetupDataBinding()
        {
            _bindingSource.DataSource = _view;
            dataGridView1.DataSource = _bindingSource;
        }

        // ================================================================
        //  备注文件操作
        // ================================================================

        private void LoadNotes()
        {
            try
            {
                if (!File.Exists(_notesFilePath))
                {
                    AppLogger.Warn($"未找到备注文件: {_notesFilePath}");
                    return;
                }

                string json = File.ReadAllText(_notesFilePath);
                var deserialized = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                if (deserialized is null)
                    return;

                _notes.Clear();
                foreach (var pair in deserialized)
                    _notes[pair.Key] = pair.Value ?? "";

                AppLogger.Info($"已加载备注文件: {_notesFilePath}（{_notes.Count} 条）");
            }
            catch (Exception ex)
            {
                AppLogger.Error("加载备注文件失败", ex);
            }
        }

        private void SaveNotes()
        {
            string tempPath = _notesFilePath + ".tmp";
            try
            {
                string json = JsonSerializer.Serialize(_notes, JsonOptions);
                File.WriteAllText(tempPath, json, new UTF8Encoding(false));
                File.Move(tempPath, _notesFilePath, overwrite: true);
            }
            catch (Exception ex)
            {
                AppLogger.Error("保存备注文件失败", ex);
                try
                {
                    if (File.Exists(tempPath))
                        File.Delete(tempPath);
                }
                catch
                {
                    // 清理临时文件失败时忽略
                }

                MessageBox.Show($"保存备注失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 扫描完成后，把扫描到的所有项写入 appname.json；
        /// 已有备注保留，没有的写入空字符串。保存前会重新读取磁盘文件，避免丢失条目。
        /// </summary>
        private void SaveAllItemsToNotes()
        {
            bool changed = false;

            try
            {
                if (File.Exists(_notesFilePath))
                {
                    string json = File.ReadAllText(_notesFilePath);
                    var existing = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                    if (existing is not null)
                    {
                        foreach (var pair in existing)
                        {
                            if (!_notes.ContainsKey(pair.Key))
                            {
                                _notes[pair.Key] = pair.Value ?? "";
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
                string name = item.Name;
                if (name.Length > 0 && !_notes.ContainsKey(name))
                {
                    _notes[name] = "";
                    changed = true;
                }
            }

            if (changed)
                SaveNotes();
        }

        // ================================================================
        //  视图：搜索、排序、统计
        // ================================================================

        private bool MatchesFilter(ScanResultItem item)
        {
            if (_searchFilter.Length == 0)
                return true;

            return item.Path.Contains(_searchFilter, StringComparison.OrdinalIgnoreCase)
                || item.Description.Contains(_searchFilter, StringComparison.OrdinalIgnoreCase)
                || item.SizeFormatted.Contains(_searchFilter, StringComparison.OrdinalIgnoreCase);
        }

        private IEnumerable<ScanResultItem> ApplySort(IEnumerable<ScanResultItem> source)
        {
            return _sortField switch
            {
                SortField.Path => _sortAscending
                    ? source.OrderBy(item => item.Path, StringComparer.OrdinalIgnoreCase)
                    : source.OrderByDescending(item => item.Path, StringComparer.OrdinalIgnoreCase),

                SortField.Description => _sortAscending
                    ? source.OrderBy(item => item.Description, StringComparer.OrdinalIgnoreCase)
                    : source.OrderByDescending(item => item.Description, StringComparer.OrdinalIgnoreCase),

                _ => _sortAscending
                    ? source.OrderBy(item => item.SizeRaw)
                    : source.OrderByDescending(item => item.SizeRaw)
            };
        }

        private void RefreshView()
        {
            var selectedPaths = GetSelectedItems()
                .Select(item => item.Path)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            _suppressSelectionUpdate = true;
            try
            {
                _view.RaiseListChangedEvents = false;
                try
                {
                    _view.Clear();
                    foreach (var item in ApplySort(_items.Where(MatchesFilter)))
                        _view.Add(item);
                }
                finally
                {
                    _view.RaiseListChangedEvents = true;
                }
                _view.ResetBindings();
            }
            finally
            {
                _suppressSelectionUpdate = false;
            }

            RestoreSelection(selectedPaths);
            UpdateSummary();
            UpdateEmptyState();
            UpdateButtonStates();
            UpdateSortGlyphs();
        }

        private void RestoreSelection(HashSet<string> paths)
        {
            if (paths.Count == 0)
                return;

            _suppressSelectionUpdate = true;
            try
            {
                dataGridView1.ClearSelection();
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.DataBoundItem is ScanResultItem item && paths.Contains(item.Path))
                        row.Selected = true;
                }
            }
            finally
            {
                _suppressSelectionUpdate = false;
            }
        }

        private void AppendBatch(IReadOnlyList<ScanResultItem> batch)
        {
            if (batch.Count == 0)
                return;

            var selectedPaths = GetSelectedItems()
                .Select(item => item.Path)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            _items.AddRange(batch);

            _view.RaiseListChangedEvents = false;
            try
            {
                foreach (var item in batch)
                {
                    _totalSize += item.SizeRaw;
                    if (MatchesFilter(item))
                        _view.Add(item);
                }
            }
            finally
            {
                _view.RaiseListChangedEvents = true;
            }

            _view.ResetBindings();

            if (selectedPaths.Count > 0)
                RestoreSelection(selectedPaths);

            UpdateSummary();
            UpdateEmptyState();
        }

        private void UpdateSummary()
        {
            statItems.Value = $"{_items.Count:N0} 项";
            statSize.Value = ScanService.FormatSize(_totalSize);

            var selected = GetSelectedItems();
            statSelected.Value = selected.Count == 0
                ? "未选择"
                : $"{selected.Count:N0} 项 · {ScanService.FormatSize(selected.Sum(item => item.SizeRaw))}";

            labelCount.Text = _searchFilter.Length == 0
                ? $"{_items.Count:N0} 项"
                : $"{_view.Count:N0} / {_items.Count:N0} 项";
        }

        private void UpdateEmptyState()
        {
            bool empty = _view.Count == 0;
            lblEmpty.Visible = empty;

            if (!empty)
                return;

            if (_isScanning)
            {
                lblEmpty.Text = "正在扫描，请稍候...";
            }
            else if (_items.Count == 0)
            {
                lblEmpty.Text = "尚未扫描任何内容\r\n\r\n点击「开始扫描」查看 AppData 目录占用情况";
            }
            else
            {
                lblEmpty.Text = "没有匹配的结果\r\n\r\n尝试更换搜索关键词";
            }

            lblEmpty.BringToFront();
        }

        private void UpdateSortGlyphs()
        {
            foreach (DataGridViewColumn column in dataGridView1.Columns)
                column.HeaderCell.SortGlyphDirection = SortOrder.None;

            int index = _sortField switch
            {
                SortField.Path => COL_PATH,
                SortField.Description => COL_DESC,
                _ => COL_SIZE
            };

            if (index >= 0 && index < dataGridView1.Columns.Count)
            {
                dataGridView1.Columns[index].HeaderCell.SortGlyphDirection =
                    _sortAscending ? SortOrder.Ascending : SortOrder.Descending;
            }
        }

        // ================================================================
        //  管理员检测
        // ================================================================

        private void CheckAdminStatus()
        {
            if (FileSystemUtils.IsAdministrator())
                return;

            labelAdminWarning.Visible = true;
            toolTip1.SetToolTip(labelAdminWarning, "以管理员身份重新启动程序，可扫描和清理更多受保护目录。");
            AppLogger.Info("程序以非管理员身份运行，部分目录可能无法访问。");
        }

        // ================================================================
        //  新版本检测
        // ================================================================

        private async void Form1_Shown(object? sender, EventArgs e)
        {
            await CheckForUpdatesAsync(silent: true);
        }

        private async void lblVersion_Click(object? sender, EventArgs e)
        {
            await CheckForUpdatesAsync(silent: false);
        }

        private async Task CheckForUpdatesAsync(bool silent)
        {
            if (_isCheckingUpdate)
                return;

            _isCheckingUpdate = true;
            if (!silent)
                labelStatus.Text = "正在检查更新...";

            try
            {
                var info = await UpdateService.CheckAsync();
                if (IsDisposed || Disposing)
                    return;

                if (info is null)
                {
                    _updateInfo = null;
                    lblUpdate.Visible = false;
                    if (!silent)
                        labelStatus.Text = $"已是最新版本 v{UpdateService.CurrentVersion}。";
                    return;
                }

                _updateInfo = info;
                ShowUpdateLink(info);
                labelStatus.Text = $"发现新版本 v{info.Version}，点击右上角链接查看。";
                AppLogger.Info($"发现新版本 {info.TagName}（当前 v{UpdateService.CurrentVersion}）。");
            }
            catch (Exception ex)
            {
                AppLogger.Warn($"检查更新失败: {ex.Message}");
                if (!silent && !IsDisposed)
                    labelStatus.Text = "检查更新失败，请稍后重试。";
            }
            finally
            {
                _isCheckingUpdate = false;
            }
        }

        private void ShowUpdateLink(UpdateInfo info)
        {
            lblUpdate.Text = $"发现新版本 v{info.Version}";
            lblUpdate.LinkArea = new LinkArea(0, lblUpdate.Text.Length);
            lblUpdate.Visible = true;
            lblUpdate.Top = lblVersion.Top;

            int width = TextRenderer.MeasureText(lblUpdate.Text, lblUpdate.Font).Width;
            lblUpdate.Left = Math.Max(lblAppTitle.Right + 24, lblVersion.Left - width - 16);

            toolTip1.SetToolTip(
                lblUpdate,
                string.IsNullOrWhiteSpace(info.Notes)
                    ? $"点击下载 v{info.Version}"
                    : TrimForTooltip(info.Notes, 400));
        }

        private static string TrimForTooltip(string text, int maxLength)
        {
            string trimmed = text.Trim();
            return trimmed.Length <= maxLength ? trimmed : trimmed[..maxLength] + "…";
        }

        private void lblUpdate_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = _updateInfo?.ReleaseUrl ?? UpdateService.ReleasesUrl;
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                AppLogger.Info($"已打开更新页面: {url}");
            }
            catch (Exception ex)
            {
                AppLogger.Warn($"打开更新页面失败: {ex.Message}");
                MessageBox.Show(
                    $"无法打开浏览器，请手动访问：\n{url}",
                    "提示",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        // ================================================================
        //  扫描
        // ================================================================

        private async void btnScan_Click(object sender, EventArgs e)
        {
            await ScanFoldersAsync();
        }

        private async Task ScanFoldersAsync()
        {
            if (_isScanning || _isDeleting)
                return;

            var checkedNames = checkedListBox1.CheckedItems.Cast<string>().ToList();
            if (checkedNames.Count == 0)
            {
                MessageBox.Show("请至少勾选一个要扫描的目录。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _isScanning = true;
            _searchFilter = searchBox.Text.Trim();

            _items.Clear();
            _view.Clear();
            _view.ResetBindings();
            _totalSize = 0;

            progressBar1.Value = 0;
            labelStatus.Text = "正在准备扫描...";
            UpdateButtonStates();
            dataGridView1.Focus();
            UpdateSummary();
            UpdateEmptyState();

            using var cts = new CancellationTokenSource();
            _cts = cts;
            var token = cts.Token;

            var service = new ScanService(_notes);
            var progress = new Progress<int>(value => progressBar1.Value = value);
            var status = new Progress<string>(text => labelStatus.Text = text);
            var batchProgress = new Progress<IReadOnlyList<ScanResultItem>>(AppendBatch);

            try
            {
                await Task.Run(
                    () => service.Scan(checkedNames, progress, status, token, batchProgress, 50),
                    token);

                SortBySizeDescending();
                dataGridView1.ClearSelection();
                SaveAllItemsToNotes();

                labelStatus.Text = $"扫描完成，共 {_items.Count:N0} 项。";
                AppLogger.Info($"扫描完成，共 {_items.Count} 项，总大小 {ScanService.FormatSize(_totalSize)}。");
            }
            catch (OperationCanceledException)
            {
                labelStatus.Text = "扫描已取消。";
                AppLogger.Info("用户取消了扫描。");

                SortBySizeDescending();
                dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                AppLogger.Error("扫描过程中发生错误", ex);
                labelStatus.Text = "扫描出错。";
                MessageBox.Show($"扫描过程中发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

                RefreshView();
            }
            finally
            {
                _isScanning = false;
                _cts = null;

                UpdateButtonStates();
                UpdateSummary();
                UpdateEmptyState();
            }
        }

        private void SortBySizeDescending()
        {
            _sortField = SortField.Size;
            _sortAscending = false;
            RefreshView();
        }

        private void CancelScan()
        {
            if (_cts is { IsCancellationRequested: false } cts)
            {
                cts.Cancel();
                labelStatus.Text = "正在取消...";
                btnCancel.Enabled = false;
            }
        }

        // ================================================================
        //  删除 / 打开 / 备注
        // ================================================================

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            await DeleteSelectedAsync(useRecycleBin: false);
        }

        private async void btnRecycle_Click(object sender, EventArgs e)
        {
            await DeleteSelectedAsync(useRecycleBin: true);
        }

        private async Task DeleteSelectedAsync(bool useRecycleBin)
        {
            if (_isScanning || _isDeleting)
                return;

            var selectedItems = GetSelectedItems();
            if (selectedItems.Count == 0)
            {
                MessageBox.Show("请先选择要删除的文件或文件夹。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string action = useRecycleBin ? "移动到回收站" : "永久删除";
            string warning = useRecycleBin
                ? "删除后仍可从回收站恢复，但部分程序的配置或游戏存档可能因此丢失。"
                : "此操作无法恢复，请确认不再需要这些文件。";

            var preview = string.Join(
                Environment.NewLine,
                selectedItems.Take(10).Select(item => "  • " + item.Path));

            if (selectedItems.Count > 10)
                preview += $"{Environment.NewLine}  … 等共 {selectedItems.Count:N0} 项";

            var result = MessageBox.Show(
                $"确认{action}以下 {selectedItems.Count:N0} 项吗？{Environment.NewLine}{Environment.NewLine}" +
                $"{warning}{Environment.NewLine}{Environment.NewLine}{preview}",
                useRecycleBin ? "确认移至回收站" : "确认永久删除",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (result != DialogResult.Yes)
                return;

            _isDeleting = true;
            UpdateButtonStates();
            UseWaitCursor = true;

            int successCount = 0;
            var failures = new List<string>();
            var deletedPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                labelStatus.Text = $"正在{action}...";

                foreach (var item in selectedItems)
                {
                    try
                    {
                        bool ok = await FileSystemUtils.DeleteAsync(item.Path, useRecycleBin);

                        if (ok)
                        {
                            successCount++;
                            deletedPaths.Add(item.Path);
                        }
                        else
                        {
                            failures.Add($"{item.Path}（操作未完成）");
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        failures.Add($"{item.Path}（拒绝访问，请尝试以管理员身份运行）");
                    }
                    catch (Exception ex)
                    {
                        AppLogger.Error($"删除失败: {item.Path}", ex);
                        failures.Add($"{item.Path}（{ex.Message}）");
                    }

                    labelStatus.Text = $"正在{action} {successCount + failures.Count}/{selectedItems.Count}...";
                    await Task.Yield();
                }

                if (deletedPaths.Count > 0)
                {
                    _totalSize -= selectedItems
                        .Where(item => deletedPaths.Contains(item.Path))
                        .Sum(item => item.SizeRaw);

                    _items.RemoveAll(item => deletedPaths.Contains(item.Path));
                }

                RefreshView();

                string summary = $"处理完成：成功 {successCount} 项，失败 {failures.Count} 项。";
                labelStatus.Text = summary;
                AppLogger.Info($"{action}: {summary}");

                if (failures.Count > 0)
                {
                    string detail = string.Join(Environment.NewLine, failures.Take(8));
                    if (failures.Count > 8)
                        detail += $"{Environment.NewLine}… 等共 {failures.Count} 项失败";

                    MessageBox.Show(
                        $"部分项目未能删除：{Environment.NewLine}{Environment.NewLine}{detail}",
                        "删除结果",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            finally
            {
                _isDeleting = false;
                UseWaitCursor = false;
                UpdateButtonStates();
                UpdateSummary();
                UpdateEmptyState();
            }
        }

        private void OpenSelectedItem()
        {
            var selectedItems = GetSelectedItems();
            if (selectedItems.Count == 0)
            {
                MessageBox.Show("请先选择一个文件或文件夹。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var item = selectedItems[0];
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = item.IsDirectory
                        ? $"\"{item.Path}\""
                        : $"/select,\"{item.Path}\"",
                    UseShellExecute = true
                };

                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                AppLogger.Error($"打开路径失败: {item.Path}", ex);
                MessageBox.Show($"打开失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditSelectedNote()
        {
            if (_isScanning || _isDeleting)
                return;

            var selectedItems = GetSelectedItems();
            if (selectedItems.Count == 0)
            {
                MessageBox.Show("请先选择一个文件或文件夹。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var item = selectedItems[0];
            string name = item.Name;
            _notes.TryGetValue(name, out var currentNote);

            using var dialog = new NoteDialog(name, item.Path, currentNote ?? "");
            if (dialog.ShowDialog(this) != DialogResult.OK)
                return;

            string newNote = dialog.Note;
            if (newNote.Length == 0)
                _notes.Remove(name);
            else
                _notes[name] = newNote;

            SaveNotes();

            foreach (var match in _items.Where(i => string.Equals(i.Name, name, StringComparison.OrdinalIgnoreCase)))
                match.Description = newNote;

            RefreshView();

            labelStatus.Text = newNote.Length == 0
                ? $"已清除「{name}」的备注。"
                : $"已更新「{name}」的备注。";
        }

        private void CopySelectedPaths()
        {
            var selectedItems = GetSelectedItems();
            if (selectedItems.Count == 0)
                return;

            try
            {
                string text = string.Join(Environment.NewLine, selectedItems.Select(item => item.Path));
                Clipboard.SetText(text);
                labelStatus.Text = $"已复制 {selectedItems.Count:N0} 条路径。";
            }
            catch (Exception ex)
            {
                AppLogger.Error("复制路径失败", ex);
                MessageBox.Show($"复制失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<ScanResultItem> GetSelectedItems()
        {
            var result = new List<ScanResultItem>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                if (row.DataBoundItem is ScanResultItem item)
                {
                    if (seen.Add(item.Path))
                        result.Add(item);
                }
                else if (row.Cells.Count > COL_PATH && row.Cells[COL_PATH].Value is string path)
                {
                    var matched = _items.FirstOrDefault(
                        i => string.Equals(i.Path, path, StringComparison.OrdinalIgnoreCase));

                    if (matched is not null && seen.Add(matched.Path))
                        result.Add(matched);
                }
            }

            return result;
        }

        // ================================================================
        //  选择所有 / 取消选择所有
        // ================================================================

        private void SelectDefaultScope()
        {
            // 首次启动默认勾选全部目录，用户可直接开始扫描。
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
                checkedListBox1.SetItemChecked(i, true);
        }

        private void ToggleSelectAll()
        {
            bool allChecked = checkedListBox1.Items.Count > 0
                && checkedListBox1.CheckedIndices.Count == checkedListBox1.Items.Count;

            for (int i = 0; i < checkedListBox1.Items.Count; i++)
                checkedListBox1.SetItemChecked(i, !allChecked);

            UpdateSelectAllButton();
        }

        private void UpdateSelectAllButton()
        {
            bool allChecked = checkedListBox1.Items.Count > 0
                && checkedListBox1.CheckedIndices.Count == checkedListBox1.Items.Count;

            btnSelectAll.Text = allChecked ? "全不选" : "全选";
        }

        // ================================================================
        //  UI 状态
        // ================================================================

        private void UpdateButtonStates()
        {
            bool busy = _isScanning || _isDeleting;
            bool hasSelection = dataGridView1.SelectedRows.Count > 0;

            btnScan.Enabled = !busy;
            btnCancel.Enabled = _isScanning;

            btnDelete.Enabled = !busy && hasSelection;
            btnRecycle.Enabled = !busy && hasSelection;
            btnOpen.Enabled = !busy && hasSelection;
            btnNote.Enabled = !busy && hasSelection;

            checkedListBox1.Enabled = !busy;
            btnSelectAll.Enabled = !busy;
            searchBox.Enabled = !_isDeleting;
        }

        private void UpdateContextMenuState()
        {
            bool busy = _isScanning || _isDeleting;
            bool hasSelection = dataGridView1.SelectedRows.Count > 0;

            ctxOpen.Enabled = !busy && hasSelection;
            ctxCopyPath.Enabled = hasSelection;
            ctxRecycle.Enabled = !busy && hasSelection;
            ctxDelete.Enabled = !busy && hasSelection;
            ctxNote.Enabled = !busy && hasSelection;
        }

        // ================================================================
        //  事件处理
        // ================================================================

        private void btnCancel_Click(object sender, EventArgs e)
        {
            CancelScan();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenSelectedItem();
        }

        private void btnNote_Click(object sender, EventArgs e)
        {
            EditSelectedNote();
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            ToggleSelectAll();
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (IsHandleCreated)
                BeginInvoke(new Action(UpdateSelectAllButton));
        }

        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            _searchFilter = searchBox.Text.Trim();
            RefreshView();
        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || e.ColumnIndex < 0)
                return;

            SortField field;
            switch (e.ColumnIndex)
            {
                case COL_PATH:
                    field = SortField.Path;
                    break;
                case COL_SIZE:
                    field = SortField.Size;
                    break;
                case COL_DESC:
                    field = SortField.Description;
                    break;
                default:
                    return;
            }

            if (_sortField == field)
                _sortAscending = !_sortAscending;
            else
            {
                _sortField = field;
                _sortAscending = field != SortField.Size;
            }

            UpdateSortGlyphs();
            RefreshView();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (_suppressSelectionUpdate)
                return;

            UpdateButtonStates();
            UpdateSummary();
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right || e.RowIndex < 0)
                return;

            var row = dataGridView1.Rows[e.RowIndex];

            // 右键点击未选中的行时切换到该行；点击已选中的行时保留多选。
            if (!row.Selected)
            {
                dataGridView1.ClearSelection();
                row.Selected = true;
            }

            if (e.ColumnIndex >= 0 && e.ColumnIndex < row.Cells.Count)
                dataGridView1.CurrentCell = row.Cells[e.ColumnIndex];

            UpdateContextMenuState();
            contextMenuStrip1.Show(Cursor.Position);
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                OpenSelectedItem();
        }

        private void dataGridView1_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridView1.Rows[e.RowIndex].DataBoundItem is ScanResultItem item)
                e.ToolTipText = item.Path;
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                e.SuppressKeyPress = true;
                CopySelectedPaths();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                OpenSelectedItem();
            }
            else if (e.KeyCode == Keys.Delete)
            {
                e.SuppressKeyPress = true;
                _ = DeleteSelectedAsync(useRecycleBin: !e.Shift);
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            UpdateContextMenuState();
        }

        private void ctxOpen_Click(object sender, EventArgs e)
        {
            OpenSelectedItem();
        }

        private void ctxCopyPath_Click(object sender, EventArgs e)
        {
            CopySelectedPaths();
        }

        private void ctxRecycle_Click(object sender, EventArgs e)
        {
            _ = DeleteSelectedAsync(useRecycleBin: true);
        }

        private void ctxDelete_Click(object sender, EventArgs e)
        {
            _ = DeleteSelectedAsync(useRecycleBin: false);
        }

        private void ctxNote_Click(object sender, EventArgs e)
        {
            EditSelectedNote();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5 && !_isScanning && !_isDeleting)
            {
                e.SuppressKeyPress = true;
                _ = ScanFoldersAsync();
            }
            else if (e.KeyCode == Keys.Escape && _isScanning)
            {
                e.SuppressKeyPress = true;
                CancelScan();
            }
            else if (e.Control && e.KeyCode == Keys.F)
            {
                e.SuppressKeyPress = true;
                searchBox.FocusInput();
            }
            else if (e.Control && e.KeyCode == Keys.O)
            {
                e.SuppressKeyPress = true;
                OpenSelectedItem();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_isDeleting)
            {
                MessageBox.Show("正在删除文件，请等待当前操作完成后再退出。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                e.Cancel = true;
                return;
            }

            if (!_isScanning)
                return;

            var result = MessageBox.Show(
                "扫描仍在进行，确定要退出吗？",
                "退出",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (result != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }

            _cts?.Cancel();
        }
    }
}
