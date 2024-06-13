using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Lab18OOP
{
    public partial class Form1 : Form
    {
        // Конструктор форми
        public Form1()
        {
            InitializeComponent();
            InitializeProcessGridView();
            InitializeContextMenu();
        }

        // Ініціалізація DataGridView з колонками
        private void InitializeProcessGridView()
        {
            dataGridViewProcesses.Columns.Add("ID", "ID");
            dataGridViewProcesses.Columns.Add("Name", "Назва");
            dataGridViewProcesses.Columns.Add("CPU", "CPU");
            dataGridViewProcesses.Columns.Add("Memory", "Пам'ять");
            DisplayProcesses();
        }

        // Відображення інформації про процеси у DataGridView
        private void DisplayProcesses()
        {
            dataGridViewProcesses.Rows.Clear();
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                try
                {
                    string id = process.Id.ToString();
                    string name = process.ProcessName;
                    string cpu = process.TotalProcessorTime.ToString();
                    string memory = (process.WorkingSet64 / 1024).ToString() + " KB";
                    dataGridViewProcesses.Rows.Add(id, name, cpu, memory);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception while reading process: {ex.Message}");
                }
            }
        }

        // Ініціалізація контекстного меню для DataGridView
        private void InitializeContextMenu()
        {
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();

            ToolStripMenuItem toolStripMenuItemInfo = new ToolStripMenuItem("Інформація про процес");
            toolStripMenuItemInfo.Click += ToolStripMenuItemInfo_Click;
            contextMenuStrip.Items.Add(toolStripMenuItemInfo);

            ToolStripMenuItem toolStripMenuItemKill = new ToolStripMenuItem("Завершити процес");
            toolStripMenuItemKill.Click += ToolStripMenuItemKill_Click;
            contextMenuStrip.Items.Add(toolStripMenuItemKill);

            ToolStripMenuItem toolStripMenuItemExport = new ToolStripMenuItem("Експорт списку процесів");
            toolStripMenuItemExport.Click += ToolStripMenuItemExport_Click;
            contextMenuStrip.Items.Add(toolStripMenuItemExport);

            ToolStripMenuItem toolStripMenuItemThreadsModules = new ToolStripMenuItem("Показати потоки і модулі");
            toolStripMenuItemThreadsModules.Click += ToolStripMenuItemThreadsModules_Click;
            contextMenuStrip.Items.Add(toolStripMenuItemThreadsModules);

            // Пункт меню для оновлення списку процесів
            ToolStripMenuItem toolStripMenuItemRefresh = new ToolStripMenuItem("Оновити список процесів");
            toolStripMenuItemRefresh.Click += ToolStripMenuItemRefresh_Click;
            contextMenuStrip.Items.Add(toolStripMenuItemRefresh);

            dataGridViewProcesses.ContextMenuStrip = contextMenuStrip;
        }

        // Обробник події вибору пункту меню "Інформація про процес"
        private void ToolStripMenuItemInfo_Click(object sender, EventArgs e)
        {
            if (dataGridViewProcesses.SelectedRows.Count > 0)
            {
                try
                {
                    DataGridViewRow selectedRow = dataGridViewProcesses.SelectedRows[0];
                    string selectedProcessId = selectedRow.Cells["ID"].Value.ToString();
                    int processId = int.Parse(selectedProcessId);
                    Process selectedProcess = Process.GetProcessById(processId);
                    string processInfo = $"ID: {selectedProcess.Id}\n" +
                                         $"Назва: {selectedProcess.ProcessName}\n" +
                                         $"Час CPU: {selectedProcess.TotalProcessorTime}\n" +
                                         $"Використання пам'яті: {(selectedProcess.WorkingSet64 / 1024)} KB";
                    MessageBox.Show(processInfo, "Інформація про процес", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка при отриманні інформації про процес: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Спочатку виберіть процес зі списку.", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Обробник події вибору пункту меню "Завершити процес"
        private void ToolStripMenuItemKill_Click(object sender, EventArgs e)
        {
            if (dataGridViewProcesses.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridViewProcesses.SelectedRows[0];
                string selectedProcessId = selectedRow.Cells["ID"].Value.ToString();
                int processId = int.Parse(selectedProcessId);
                Process selectedProcess = Process.GetProcessById(processId);
                try
                {
                    selectedProcess.Kill();
                    MessageBox.Show($"Процес {selectedProcess.ProcessName} з ID {selectedProcess.Id} було завершено.",
                                    "Процес завершено", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DisplayProcesses(); // Оновити список процесів у DataGridView
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не вдалося завершити процес: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Обробник події вибору пункту меню "Експорт списку процесів"
        private void ToolStripMenuItemExport_Click(object sender, EventArgs e)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter("СписокПроцесів.txt"))
                {
                    foreach (DataGridViewRow row in dataGridViewProcesses.Rows)
                    {
                        string line = $"{row.Cells["ID"].Value}, {row.Cells["Name"].Value}, {row.Cells["CPU"].Value}, {row.Cells["Memory"].Value}";
                        sw.WriteLine(line);
                    }
                }
                MessageBox.Show("Список процесів експортовано в СписокПроцесів.txt", "Експорт завершено", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не вдалося експортувати список процесів: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обробник події вибору пункту меню "Показати потоки і модулі"
        private void ToolStripMenuItemThreadsModules_Click(object sender, EventArgs e)
        {
            if (dataGridViewProcesses.SelectedRows.Count > 0)
            {
                try
                {
                    DataGridViewRow selectedRow = dataGridViewProcesses.SelectedRows[0];
                    if (selectedRow.Cells["ID"].Value == null)
                    {
                        MessageBox.Show("Помилка: не вдалося отримати ідентифікатор процесу.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string selectedProcessId = selectedRow.Cells["ID"].Value.ToString();
                    int processId;
                    if (!int.TryParse(selectedProcessId, out processId))
                    {
                        MessageBox.Show("Помилка: некоректний формат ідентифікатора процесу.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    Process selectedProcess = Process.GetProcessById(processId);

                    // Вивід інформації про потоки
                    string threadsInfo = "Потоки:\n";
                    foreach (ProcessThread thread in selectedProcess.Threads)
                    {
                        threadsInfo += $"ID потоку: {thread.Id}, Пріоритет: {thread.PriorityLevel}, Час початку: {thread.StartTime}\n";
                    }

                    // Вивід інформації про модулі
                    string modulesInfo = "\nМодулі:\n";
                    foreach (ProcessModule module in selectedProcess.Modules)
                    {
                        modulesInfo += $"Назва модуля: {module.ModuleName}, Ім'я файлу: {module.FileName}, Розмір пам'яті: {module.ModuleMemorySize}\n";
                    }

                    MessageBox.Show(threadsInfo + modulesInfo, "Інформація про потоки і модулі", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка при виведенні інформації про потоки і модулі: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Спочатку виберіть процес зі списку.", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Оновлення списку процесів при натисканні кнопки оновлення
        private void ToolStripMenuItemRefresh_Click(object sender, EventArgs e)
        {
            DisplayProcesses();
        }
    }
}
