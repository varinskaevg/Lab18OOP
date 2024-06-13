using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Lab18OOP
{
    public partial class Form1 : Form
    {
        // ����������� �����
        public Form1()
        {
            InitializeComponent();
            InitializeProcessGridView();
            InitializeContextMenu();
        }

        // ����������� DataGridView � ���������
        private void InitializeProcessGridView()
        {
            dataGridViewProcesses.Columns.Add("ID", "ID");
            dataGridViewProcesses.Columns.Add("Name", "�����");
            dataGridViewProcesses.Columns.Add("CPU", "CPU");
            dataGridViewProcesses.Columns.Add("Memory", "���'���");
            DisplayProcesses();
        }

        // ³���������� ���������� ��� ������� � DataGridView
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

        // ����������� ������������ ���� ��� DataGridView
        private void InitializeContextMenu()
        {
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();

            ToolStripMenuItem toolStripMenuItemInfo = new ToolStripMenuItem("���������� ��� ������");
            toolStripMenuItemInfo.Click += ToolStripMenuItemInfo_Click;
            contextMenuStrip.Items.Add(toolStripMenuItemInfo);

            ToolStripMenuItem toolStripMenuItemKill = new ToolStripMenuItem("��������� ������");
            toolStripMenuItemKill.Click += ToolStripMenuItemKill_Click;
            contextMenuStrip.Items.Add(toolStripMenuItemKill);

            ToolStripMenuItem toolStripMenuItemExport = new ToolStripMenuItem("������� ������ �������");
            toolStripMenuItemExport.Click += ToolStripMenuItemExport_Click;
            contextMenuStrip.Items.Add(toolStripMenuItemExport);

            ToolStripMenuItem toolStripMenuItemThreadsModules = new ToolStripMenuItem("�������� ������ � �����");
            toolStripMenuItemThreadsModules.Click += ToolStripMenuItemThreadsModules_Click;
            contextMenuStrip.Items.Add(toolStripMenuItemThreadsModules);

            // ����� ���� ��� ��������� ������ �������
            ToolStripMenuItem toolStripMenuItemRefresh = new ToolStripMenuItem("������� ������ �������");
            toolStripMenuItemRefresh.Click += ToolStripMenuItemRefresh_Click;
            contextMenuStrip.Items.Add(toolStripMenuItemRefresh);

            dataGridViewProcesses.ContextMenuStrip = contextMenuStrip;
        }

        // �������� ��䳿 ������ ������ ���� "���������� ��� ������"
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
                                         $"�����: {selectedProcess.ProcessName}\n" +
                                         $"��� CPU: {selectedProcess.TotalProcessorTime}\n" +
                                         $"������������ ���'��: {(selectedProcess.WorkingSet64 / 1024)} KB";
                    MessageBox.Show(processInfo, "���������� ��� ������", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"������� ��� �������� ���������� ��� ������: {ex.Message}", "�������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("�������� ������� ������ � ������.", "�����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // �������� ��䳿 ������ ������ ���� "��������� ������"
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
                    MessageBox.Show($"������ {selectedProcess.ProcessName} � ID {selectedProcess.Id} ���� ���������.",
                                    "������ ���������", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DisplayProcesses(); // ������� ������ ������� � DataGridView
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"�� ������� ��������� ������: {ex.Message}", "�������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // �������� ��䳿 ������ ������ ���� "������� ������ �������"
        private void ToolStripMenuItemExport_Click(object sender, EventArgs e)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter("�������������.txt"))
                {
                    foreach (DataGridViewRow row in dataGridViewProcesses.Rows)
                    {
                        string line = $"{row.Cells["ID"].Value}, {row.Cells["Name"].Value}, {row.Cells["CPU"].Value}, {row.Cells["Memory"].Value}";
                        sw.WriteLine(line);
                    }
                }
                MessageBox.Show("������ ������� ������������ � �������������.txt", "������� ���������", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"�� ������� ������������ ������ �������: {ex.Message}", "�������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // �������� ��䳿 ������ ������ ���� "�������� ������ � �����"
        private void ToolStripMenuItemThreadsModules_Click(object sender, EventArgs e)
        {
            if (dataGridViewProcesses.SelectedRows.Count > 0)
            {
                try
                {
                    DataGridViewRow selectedRow = dataGridViewProcesses.SelectedRows[0];
                    if (selectedRow.Cells["ID"].Value == null)
                    {
                        MessageBox.Show("�������: �� ������� �������� ������������� �������.", "�������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string selectedProcessId = selectedRow.Cells["ID"].Value.ToString();
                    int processId;
                    if (!int.TryParse(selectedProcessId, out processId))
                    {
                        MessageBox.Show("�������: ����������� ������ �������������� �������.", "�������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    Process selectedProcess = Process.GetProcessById(processId);

                    // ���� ���������� ��� ������
                    string threadsInfo = "������:\n";
                    foreach (ProcessThread thread in selectedProcess.Threads)
                    {
                        threadsInfo += $"ID ������: {thread.Id}, ��������: {thread.PriorityLevel}, ��� �������: {thread.StartTime}\n";
                    }

                    // ���� ���������� ��� �����
                    string modulesInfo = "\n�����:\n";
                    foreach (ProcessModule module in selectedProcess.Modules)
                    {
                        modulesInfo += $"����� ������: {module.ModuleName}, ��'� �����: {module.FileName}, ����� ���'��: {module.ModuleMemorySize}\n";
                    }

                    MessageBox.Show(threadsInfo + modulesInfo, "���������� ��� ������ � �����", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"������� ��� �������� ���������� ��� ������ � �����: {ex.Message}", "�������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("�������� ������� ������ � ������.", "�����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ��������� ������ ������� ��� ��������� ������ ���������
        private void ToolStripMenuItemRefresh_Click(object sender, EventArgs e)
        {
            DisplayProcesses();
        }
    }
}
