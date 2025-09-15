using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace AppDataCleaner // 替换为你的命名空间
{
    public partial class Form1 : Form
    {
        private Dictionary<string, string> notes = new Dictionary<string, string>();
        private const string notesFilePath = "appname.json"; // 备注文件路径

        public Form1()
        {
            InitializeComponent();
            LoadNotes(); // 加载备注
            InitcheckedListBox1();
        }

        private void InitcheckedListBox1()//默认全选
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, true);
            }
        }

        private string FormatSize(long size)
        {
            if (size < 1024)
                return $"{size} B";
            else if (size < 1024 * 1024)
                return $"{size / 1024.0:F2} KB"; // 保留两位小数
            else if (size < 1024 * 1024 * 1024)
                return $"{size / (1024.0 * 1024):F2} MB"; // 保留两位小数
            else
                return $"{size / (1024.0 * 1024 * 1024):F2} GB"; // 保留两位小数
        }

        private void UpdateProgress(int value)
        {
            progressBar1.Value = value;
        }

        private void ScanFolders(IProgress<int> progress)
        {
            string userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string programDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData); // C:\ProgramData
            string[] targetFolders = {
        Path.Combine(userProfilePath, "AppData", "Local"),
        Path.Combine(userProfilePath, "AppData", "LocalLow"),
        Path.Combine(userProfilePath, "AppData", "Roaming"),
        programDataPath // 添加ProgramData目录
    };

            dataGridView1.Rows.Clear(); // 清除之前的结果
            fileSizeCache.Clear(); // 清除文件大小缓存

            int totalFiles = 0;
            int totalDirectories = 0;

            // 计算被选择文件夹的总文件和文件夹数量
            foreach (var item in checkedListBox1.CheckedItems)
            {
                string folderName = item.ToString();
                string folderPath;
                
                // 特殊处理ProgramData，因为它是完整路径
                if (folderName == "ProgramData")
                {
                    folderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                }
                else
                {
                    folderPath = Array.Find(targetFolders, f => f.EndsWith(folderName));
                }

                if (folderPath != null && Directory.Exists(folderPath))
                {
                    totalFiles += Directory.GetFiles(folderPath).Length;
                    totalDirectories += Directory.GetDirectories(folderPath).Length;
                }
            }

            int processedFiles = 0;
            int processedDirectories = 0;

            // 处理 ChecklistBox 的选项
            foreach (var item in checkedListBox1.CheckedItems)
            {
                string folderName = item.ToString();
                string folderPath;
                
                // 特殊处理ProgramData，因为它是完整路径
                if (folderName == "ProgramData")
                {
                    folderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                }
                else
                {
                    folderPath = Array.Find(targetFolders, f => f.EndsWith(folderName));
                }

                if (folderPath != null && Directory.Exists(folderPath))
                {
                    var files = Directory.GetFiles(folderPath);
                    var subDirectories = Directory.GetDirectories(folderPath);

                    // 使用并行处理文件
                    Parallel.ForEach(files, (file) =>
                    {
                        try
                        {
                            long fileSize = new FileInfo(file).Length; // 获取文件大小
                            Invoke((MethodInvoker)delegate {
                                AddRowWithSizeCache(file, FormatSize(fileSize), GetNoteDescription(Path.GetFileName(file)), fileSize);
                            });
                        }
                        catch (Exception ex)
                        {
                            // 忽略不可访问文件
                        }

                        Interlocked.Increment(ref processedFiles);
                        progress.Report((processedFiles + processedDirectories) * 100 / (totalFiles + totalDirectories));
                    });

                    // 使用并行处理子目录
                    Parallel.ForEach(subDirectories, (subDir) =>
                    {
                        try
                        {
                            long dirSize = GetDirectorySize(subDir); // 获取文件夹大小
                            Invoke((MethodInvoker)delegate {
                                AddRowWithSizeCache(subDir, FormatSize(dirSize), GetNoteDescription(Path.GetFileName(subDir)), dirSize);
                            });
                        }
                        catch (Exception ex)
                        {
                            // 忽略不可访问文件
                        }

                        Interlocked.Increment(ref processedDirectories);
                        progress.Report((processedFiles + processedDirectories) * 100 / (totalFiles + totalDirectories));
                    });
                }
                else
                {
                    MessageBox.Show($"找不到文件夹: {folderPath}");
                }
            }
        }



        // 计算文件夹的总大小
        private long GetDirectorySize(string dirPath)
        {
            long size = 0;

            try
            {
                // 计算文件大小
                var files = Directory.GetFiles(dirPath, "*.*", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    try
                    {
                        size += new FileInfo(file).Length;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // 忽略拒绝访问的文件
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // 忽略拒绝访问的文件夹
            }

            return size;
        }

        private async void ScanFoldersAsync()
        {
            await Task.Run(() => ScanFolders(new Progress<int>(UpdateProgress)));
            
            // 扫描完成后自动按文件大小排序
            SortDataGridViewByFileSize();
        }

        private void LoadNotes()
        {
            if (File.Exists(notesFilePath))
            {
                var json = File.ReadAllText(notesFilePath, System.Text.Encoding.UTF8);
                notes = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
        }

        private void SaveNotes()
        {
            var json = JsonConvert.SerializeObject(notes, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(notesFilePath, json, System.Text.Encoding.UTF8);
        }

        private void buttonScan_Click(object sender, EventArgs e)
        {
            ScanFoldersAsync();
        }


        private string GetNoteDescription(string name)
        {
            return notes.ContainsKey(name) ? notes[name] : "";
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // 获取路径
                string path = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();

                // 弹出确认对话框
                DialogResult result = MessageBox.Show("删除未知的文件可能导致程序配置丢失、游戏存档丢失等，确认要删除选定的项吗？",
                                                       "确认删除",
                                                       MessageBoxButtons.YesNo,
                                                       MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        if (Directory.Exists(path))
                        {
                            Directory.Delete(path, true);
                        }
                        else if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                        else
                        {
                            MessageBox.Show("文件或文件夹不存在。");
                        }

                        // 从缓存中移除该路径
                        if (fileSizeCache.ContainsKey(path))
                        {
                            fileSizeCache.Remove(path);
                        }
                        
                        // 从 DataGridView 中删除选定的行
                        dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        MessageBox.Show($"删除失败!访问被拒绝: {ex.Message}");
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show($"删除失败!IO错误: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"删除失败!: {ex.Message}");
                    }
                }
                // 如果用户选择"否"，什么都不做
            }
            else
            {
                MessageBox.Show("请先选择一个文件或文件夹。");
            }
        }



        private void OpenButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                string path = dataGridView1.SelectedRows[0].Cells[0].Value.ToString(); // 获取路径
                try
                {
                    System.Diagnostics.Process.Start("explorer.exe", path);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"打开失败: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("请先选择一个文件或文件夹。");
            }
        }

        private void NoteButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                string path = dataGridView1.SelectedRows[0].Cells[0].Value.ToString(); // 获取完整路径
                string name = Path.GetFileName(path); // 获取文件或文件夹名称
                string currentNote = notes.ContainsKey(name) ? notes[name] : "";

                string note = Microsoft.VisualBasic.Interaction.InputBox("请输入文件备注信息", "备注", currentNote);

                if (!string.IsNullOrWhiteSpace(note))
                {
                    notes[name] = note; // 只保存文件或文件夹的名称作为索引
                    SaveNotes(); // 保存备注到文件

                    // 更新 DataGridView 中的备注显示
                    UpdateDataGridViewItem(path, note); // 使用完整路径进行更新
                }
            }
            else
            {
                MessageBox.Show("请先选择一个文件或文件夹。");
            }
        }

        private void UpdateDataGridViewItem(string path, string note)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[0].Value.ToString() == path) // 使用完整路径进行匹配
                {
                    // 更新备注内容
                    row.Cells[2].Value = note;
                    break;
                }
            }
        }

        // 存储原始文件大小值的字典，用于排序
        private Dictionary<string, long> fileSizeCache = new Dictionary<string, long>();

        // 添加行到DataGridView并缓存原始大小值
        private void AddRowWithSizeCache(string path, string formattedSize, string description, long rawSize)
        {
            dataGridView1.Rows.Add(path, formattedSize, description);
            fileSizeCache[path] = rawSize;
        }

        // 处理DataGridView列标题点击事件
        private void DataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // 如果点击的是大小列（索引为1）
            if (e.ColumnIndex == 1)
            {
                // 按照数值大小排序，而不是字符串
                SortDataGridViewByFileSize();
            }
        }

        // 按文件大小排序DataGridView
        private void SortDataGridViewByFileSize()
        {
            // 创建一个临时列表来存储行数据
            List<DataGridViewRow> rows = new List<DataGridViewRow>();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                rows.Add(row);
            }

            // 按照缓存的原始大小值排序
            rows.Sort((a, b) => 
            {
                string pathA = a.Cells[0].Value.ToString();
                string pathB = b.Cells[0].Value.ToString();
                
                long sizeA = fileSizeCache.ContainsKey(pathA) ? fileSizeCache[pathA] : 0;
                long sizeB = fileSizeCache.ContainsKey(pathB) ? fileSizeCache[pathB] : 0;
                
                // 降序排列（从大到小）
                return sizeB.CompareTo(sizeA);
            });

            // 清空DataGridView并按排序后的顺序重新添加行
            dataGridView1.Rows.Clear();
            foreach (DataGridViewRow row in rows)
            {
                dataGridView1.Rows.Add(row);
            }
        }
    }
}
