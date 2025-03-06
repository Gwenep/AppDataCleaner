using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace AppDataCleaner // �滻Ϊ��������ռ�
{
    public partial class Form1 : Form
    {
        private Dictionary<string, string> notes = new Dictionary<string, string>();
        private const string notesFilePath = "appname.json"; // �����ļ�·��

        public Form1()
        {
            InitializeComponent();
            LoadNotes(); // ���ر�ע
        }

        private string FormatSize(long size)
        {
            if (size < 1024)
                return $"{size} �ֽ�";
            else if (size < 1024 * 1024)
                return $"{size / 1024.0:F2} KB"; // ������λС��
            else if (size < 1024 * 1024 * 1024)
                return $"{size / (1024.0 * 1024):F2} MB"; // ������λС��
            else
                return $"{size / (1024.0 * 1024 * 1024):F2} GB"; // ������λС��
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

            dataGridView1.Rows.Clear(); // ���֮ǰ�Ľ��

            int totalFiles = 0;
            int totalDirectories = 0;

            // �ȼ������ļ����ļ�������
            foreach (var folder in targetFolders)
            {
                if (Directory.Exists(folder))
                {
                    totalFiles += Directory.GetFiles(folder).Length;
                    totalDirectories += Directory.GetDirectories(folder).Length;
                }
            }

            int processedFiles = 0;
            int processedDirectories = 0;

            foreach (var folder in targetFolders)
            {
                if (Directory.Exists(folder))
                {
                    var files = Directory.GetFiles(folder);
                    var subDirectories = Directory.GetDirectories(folder);

                    // ʹ�ò��д����ļ�
                    Parallel.ForEach(files, (file) =>
                    {
                        try
                        {
                            long fileSize = new FileInfo(file).Length; // ��ȡ�ļ���С
                            Invoke((MethodInvoker)delegate {
                                dataGridView1.Rows.Add(file, FormatSize(fileSize), GetNoteDescription(Path.GetFileName(file)));
                            });
                        }
                        catch (UnauthorizedAccessException)
                        {
                            // ���Ծܾ����ʵ��ļ�
                        }

                        Interlocked.Increment(ref processedFiles);
                        progress.Report((processedFiles + processedDirectories) * 100 / (totalFiles + totalDirectories));
                    });

                    // ʹ�ò��д����ļ���
                    Parallel.ForEach(subDirectories, (subDir) =>
                    {
                        try
                        {
                            long dirSize = GetDirectorySize(subDir); // ��ȡ�ļ��д�С
                            Invoke((MethodInvoker)delegate {
                                dataGridView1.Rows.Add(subDir, FormatSize(dirSize), GetNoteDescription(Path.GetFileName(subDir)));
                            });
                        }
                        catch (UnauthorizedAccessException)
                        {
                            // ���Ծܾ����ʵ��ļ���
                        }

                        Interlocked.Increment(ref processedDirectories);
                        progress.Report((processedFiles + processedDirectories) * 100 / (totalFiles + totalDirectories));
                    });
                }
                else
                {
                    MessageBox.Show($"Folder not found: {folder}");
                }
            }
        }

        // �����ļ��е��ܴ�С
        private long GetDirectorySize(string dirPath)
        {
            long size = 0;

            try
            {
                // �����ļ���С
                var files = Directory.GetFiles(dirPath, "*.*", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    try
                    {
                        size += new FileInfo(file).Length;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // ���Ծܾ����ʵ��ļ�
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // ���Ծܾ����ʵ��ļ���
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
                // ��ȡ·��
                string path = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();

                // ����ȷ�϶Ի���
                DialogResult result = MessageBox.Show("ɾ��δ֪���ļ����ܵ�����������á���Ϸ�浵��ʧ�ȣ���ȷ��Ҫɾ��ѡ��������",
                                                       "ȷ��ɾ��",
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
                            MessageBox.Show("�ļ����ļ��в����ڡ�");
                        }

                        // �� DataGridView ��ɾ��ѡ������
                        dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        MessageBox.Show($"���ʱ��ܾ�: {ex.Message}");
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show($"IO����: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"ɾ��ʧ��: {ex.Message}");
                    }
                }
                // ����û�ѡ�񡰷񡱣�ʲô������
            }
            else
            {
                MessageBox.Show("����ѡ��һ���ļ����ļ��С�");
            }
        }



        private void OpenButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                string path = dataGridView1.SelectedRows[0].Cells[0].Value.ToString(); // ��ȡ·��
                try
                {
                    System.Diagnostics.Process.Start("explorer.exe", path);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"��ʧ��: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("����ѡ��һ���ļ����ļ��С�");
            }
        }

        private void NoteButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                string path = dataGridView1.SelectedRows[0].Cells[0].Value.ToString(); // ��ȡ����·��
                string name = Path.GetFileName(path); // ��ȡ�ļ����ļ�������
                string currentNote = notes.ContainsKey(name) ? notes[name] : "";

                string note = Microsoft.VisualBasic.Interaction.InputBox("�������ļ�������", "����", currentNote);

                if (!string.IsNullOrWhiteSpace(note))
                {
                    notes[name] = note; // ֻ�����ļ����ļ��е�������Ϊ����
                    SaveNotes(); // �����������ļ�

                    // ���� DataGridView �еı�ע��ʾ
                    UpdateDataGridViewItem(path, note); // ʹ������·�����и���
                }
            }
            else
            {
                MessageBox.Show("����ѡ��һ���ļ����ļ��С�");
            }
        }

        private void UpdateDataGridViewItem(string path, string note)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[0].Value.ToString() == path) // ʹ������·������ƥ��
                {
                    // ������������
                    row.Cells[2].Value = note;
                    break;
                }
            }
        }
    }
}
