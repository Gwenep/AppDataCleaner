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
        private const string notesFilePath = "appname.json"; // 配置文件路径

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
                return $"{size} 字节";
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
            string[] targetFolders = {
        Path.Combine(userProfilePath, "AppData", "Local"),
        Path.Combine(userProfilePath, "AppData", "LocalLow"),
        Path.Combine(userProfilePath, "AppData", "Roaming")
    };

            dataGridView1.Rows.Clear(); // 清空之前的结果

            int totalFiles = 0;
            int totalDirectories = 0;

            // 计算被选中文件夹的总文件和文件夹数量
            foreach (var item in checkedListBox1.CheckedItems)
            {
                string folderName = item.ToString();
                string folderPath = Array.Find(targetFolders, f => f.EndsWith(folderName));

                if (folderPath != null && Directory.Exists(folderPath))
                {
                    totalFiles += Directory.GetFiles(folderPath).Length;
                    totalDirectories += Directory.GetDirectories(folderPath).Length;
                }
            }

            int processedFiles = 0;
            int processedDirectories = 0;

            // 遍历 ChecklistBox 的选项
            foreach (var item in checkedListBox1.CheckedItems)
            {
                string folderName = item.ToString();
                string folderPath = Array.Find(targetFolders, f => f.EndsWith(folderName));

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
                                dataGridView1.Rows.Add(file, FormatSize(fileSize), GetNoteDescription(Path.GetFileName(file)));
                            });
                        }
                        catch (Exception ex)
                        {
                            // 忽略报错的文件
                        }

                        Interlocked.Increment(ref processedFiles);
                        progress.Report((processedFiles + processedDirectories) * 100 / (totalFiles + totalDirectories));
                    });

                    // 使用并行处理文件夹
                    Parallel.ForEach(subDirectories, (subDir) =>
                    {
                        try
                        {
                            long dirSize = GetDirectorySize(subDir); // 获取文件夹大小
                            Invoke((MethodInvoker)delegate {
                                dataGridView1.Rows.Add(subDir, FormatSize(dirSize), GetNoteDescription(Path.GetFileName(subDir)));
                            });
                        }
                        catch (Exception ex)
                        {
                            // 忽略报错的文件
                        }

                        Interlocked.Increment(ref processedDirectories);
                        progress.Report((processedFiles + processedDirectories) * 100 / (totalFiles + totalDirectories));
                    });
                }
                else
                {
                    MessageBox.Show($"Folder not found: {folderPath}");
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
        }

        private void LoadNotes()
        {
            if (File.Exists(notesFilePath))
            {
                var json = File.ReadAllText(notesFilePath);
                notes = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
        }

        private void SaveNotes()
        {
            var json = JsonConvert.SerializeObject(notes, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(notesFilePath, json);
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
                // 提取路径
                string path = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();

                // 弹出确认对话框
                DialogResult result = MessageBox.Show("删除未知的文件可能导致您软件配置、游戏存档丢失等，您确定要删除选定的项吗？",
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
                // 如果用户选择“否”，什么都不做
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
                string path = dataGridView1.SelectedRows[0].Cells[0].Value.ToString(); // 提取路径
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
                string path = dataGridView1.SelectedRows[0].Cells[0].Value.ToString(); // 提取完整路径
                string name = Path.GetFileName(path); // 获取文件或文件夹名称
                string currentNote = notes.ContainsKey(name) ? notes[name] : "";

                string note = Microsoft.VisualBasic.Interaction.InputBox("请输入文件描述：", "描述", currentNote);

                if (!string.IsNullOrWhiteSpace(note))
                {
                    notes[name] = note; // 只保存文件或文件夹的名称作为描述
                    SaveNotes(); // 保存描述到文件

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
                    // 更新描述部分
                    row.Cells[2].Value = note;
                    break;
                }
            }
        }
    }
}
