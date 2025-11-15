using System.ComponentModel;

namespace TaskManager
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private Button btnAdd;
        private Button btnRefresh;
        private DataGridView dataGridView1;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel statusLabel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.dataGridView1 = new DataGridView();
            this.btnAdd = new Button();
            this.btnRefresh = new Button();
            this.statusStrip1 = new StatusStrip();
            this.statusLabel = new ToolStripStatusLabel();

            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();

            // dataGridView1
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new Point(12, 12);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new Size(776, 370);
            this.dataGridView1.TabIndex = 0;

            // btnAdd
            this.btnAdd.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.btnAdd.Font = new Font("Segoe UI", 9F);
            this.btnAdd.Location = new Point(610, 388);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new Size(100, 30);
            this.btnAdd.TabIndex = 1;
            this.btnAdd.Text = "➕ Добавить";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += btnAdd_Click;

            // btnRefresh
            this.btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.btnRefresh.Font = new Font("Segoe UI", 9F);
            this.btnRefresh.Location = new Point(716, 388);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new Size(74, 30);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "🔄";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += btnRefresh_Click;

            // statusStrip1
            this.statusStrip1.Items.AddRange(new ToolStripItem[] { this.statusLabel });
            this.statusStrip1.Location = new Point(0, 424);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new Size(800, 22);
            this.statusStrip1.TabIndex = 3;

            // statusLabel
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new Size(118, 17);
            this.statusLabel.Text = "Загрузка задач...";

            // MainForm
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(800, 446);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.dataGridView1);
            this.Font = new Font("Segoe UI", 9F);
            this.MinimumSize = new Size(600, 300);
            this.Name = "MainForm";
            this.Text = "Менеджер задач — TaskManager";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}