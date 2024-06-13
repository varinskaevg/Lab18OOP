namespace Lab18OOP
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
            dataGridViewProcesses = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)dataGridViewProcesses).BeginInit();
            SuspendLayout();
            // 
            // dataGridViewProcesses
            // 
            dataGridViewProcesses.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewProcesses.Location = new Point(12, 12);
            dataGridViewProcesses.Name = "dataGridViewProcesses";
            dataGridViewProcesses.RowHeadersWidth = 51;
            dataGridViewProcesses.Size = new Size(578, 528);
            dataGridViewProcesses.TabIndex = 0;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(595, 565);
            Controls.Add(dataGridViewProcesses);
            Name = "Form1";
            Text = "Процеси";
            ((System.ComponentModel.ISupportInitialize)dataGridViewProcesses).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dataGridViewProcesses;
    }
}
