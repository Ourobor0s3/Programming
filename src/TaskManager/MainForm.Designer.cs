namespace TaskManager
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private Button btnAdd;
        private Button btnRefresh;
        private Button btnEdit;
        private Button btnDelete;
        private DataGridView dataGridView1;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel statusLabel;
        private Panel headerPanel;
        private Label lblTitle;
        private Label lblSubtitle;
        private FlowLayoutPanel actionsPanel;
        private ContextMenuStrip gridContextMenu;
        private ToolStripMenuItem ctxEditItem;
        private ToolStripMenuItem ctxDeleteItem;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            dataGridView1 = new DataGridView();
            gridContextMenu = new ContextMenuStrip(components);
            ctxEditItem = new ToolStripMenuItem();
            ctxDeleteItem = new ToolStripMenuItem();
            btnAdd = new Button();
            btnRefresh = new Button();
            btnEdit = new Button();
            btnDelete = new Button();
            statusStrip1 = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            headerPanel = new Panel();
            lblTitle = new Label();
            lblSubtitle = new Label();
            actionsPanel = new FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            gridContextMenu.SuspendLayout();
            statusStrip1.SuspendLayout();
            headerPanel.SuspendLayout();
            actionsPanel.SuspendLayout();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(242, 245, 250);
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle1.ForeColor = Color.FromArgb(23, 43, 77);
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.ContextMenuStrip = gridContextMenu;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = SystemColors.Window;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = Color.FromArgb(232, 242, 254);
            dataGridViewCellStyle2.SelectionForeColor = Color.Black;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.Location = new Point(12, 150);
            dataGridView1.MultiSelect = false;
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.RowTemplate.Height = 36;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new Size(776, 302);
            dataGridView1.TabIndex = 0;
            dataGridView1.CellDoubleClick += dataGridView1_CellDoubleClick;
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
            // 
            // gridContextMenu
            // 
            gridContextMenu.Items.AddRange(new ToolStripItem[] { ctxEditItem, ctxDeleteItem });
            gridContextMenu.Name = "gridContextMenu";
            gridContextMenu.Size = new Size(129, 48);
            // 
            // ctxEditItem
            // 
            ctxEditItem.Name = "ctxEditItem";
            ctxEditItem.Size = new Size(128, 22);
            ctxEditItem.Text = "Изменить";
            ctxEditItem.Click += ctxEditItem_Click;
            // 
            // ctxDeleteItem
            // 
            ctxDeleteItem.Name = "ctxDeleteItem";
            ctxDeleteItem.Size = new Size(128, 22);
            ctxDeleteItem.Text = "Удалить";
            ctxDeleteItem.Click += ctxDeleteItem_Click;
            // 
            // btnAdd
            // 
            btnAdd.AutoSize = true;
            btnAdd.BackColor = Color.FromArgb(55, 125, 255);
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            btnAdd.ForeColor = Color.White;
            btnAdd.Location = new Point(377, 3);
            btnAdd.Name = "btnAdd";
            btnAdd.Padding = new Padding(12, 6, 12, 6);
            btnAdd.Size = new Size(97, 39);
            btnAdd.TabIndex = 1;
            btnAdd.Text = "Добавить";
            btnAdd.UseVisualStyleBackColor = false;
            btnAdd.Click += btnAdd_Click;
            // 
            // btnRefresh
            // 
            btnRefresh.AutoSize = true;
            btnRefresh.BackColor = Color.White;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.Font = new Font("Segoe UI", 9F);
            btnRefresh.ForeColor = Color.FromArgb(55, 125, 255);
            btnRefresh.Location = new Point(676, 3);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Padding = new Padding(12, 6, 12, 6);
            btnRefresh.Size = new Size(97, 39);
            btnRefresh.TabIndex = 2;
            btnRefresh.Text = "Обновить";
            btnRefresh.UseVisualStyleBackColor = false;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // btnEdit
            // 
            btnEdit.AutoSize = true;
            btnEdit.BackColor = Color.White;
            btnEdit.Enabled = false;
            btnEdit.FlatStyle = FlatStyle.Flat;
            btnEdit.Font = new Font("Segoe UI", 9F);
            btnEdit.ForeColor = Color.FromArgb(23, 43, 77);
            btnEdit.Location = new Point(573, 3);
            btnEdit.Name = "btnEdit";
            btnEdit.Padding = new Padding(12, 6, 12, 6);
            btnEdit.Size = new Size(97, 39);
            btnEdit.TabIndex = 4;
            btnEdit.Text = "Изменить";
            btnEdit.UseVisualStyleBackColor = false;
            btnEdit.Click += btnEdit_Click;
            // 
            // btnDelete
            // 
            btnDelete.AutoSize = true;
            btnDelete.BackColor = Color.FromArgb(255, 92, 87);
            btnDelete.Enabled = false;
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.Font = new Font("Segoe UI", 9F);
            btnDelete.ForeColor = Color.White;
            btnDelete.Location = new Point(480, 3);
            btnDelete.Name = "btnDelete";
            btnDelete.Padding = new Padding(12, 6, 12, 6);
            btnDelete.Size = new Size(87, 39);
            btnDelete.TabIndex = 5;
            btnDelete.Text = "Удалить";
            btnDelete.UseVisualStyleBackColor = false;
            btnDelete.Click += btnDelete_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { statusLabel });
            statusStrip1.Location = new Point(0, 466);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(800, 22);
            statusStrip1.TabIndex = 3;
            // 
            // statusLabel
            // 
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(97, 17);
            statusLabel.Text = "Загрузка задач...";
            // 
            // headerPanel
            // 
            headerPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            headerPanel.BackColor = Color.FromArgb(19, 57, 100);
            headerPanel.Controls.Add(lblTitle);
            headerPanel.Controls.Add(lblSubtitle);
            headerPanel.Location = new Point(12, 12);
            headerPanel.Name = "headerPanel";
            headerPanel.Padding = new Padding(16);
            headerPanel.Size = new Size(776, 80);
            headerPanel.TabIndex = 6;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 14);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(208, 32);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Менеджер задач";
            // 
            // lblSubtitle
            // 
            lblSubtitle.AutoSize = true;
            lblSubtitle.Font = new Font("Segoe UI", 10F);
            lblSubtitle.ForeColor = Color.FromArgb(224, 233, 244);
            lblSubtitle.Location = new Point(22, 48);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(299, 19);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Добавляйте, редактируйте и удаляйте задачи";
            // 
            // actionsPanel
            // 
            actionsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            actionsPanel.BackColor = Color.Transparent;
            actionsPanel.Controls.Add(btnRefresh);
            actionsPanel.Controls.Add(btnEdit);
            actionsPanel.Controls.Add(btnDelete);
            actionsPanel.Controls.Add(btnAdd);
            actionsPanel.FlowDirection = FlowDirection.RightToLeft;
            actionsPanel.Location = new Point(12, 98);
            actionsPanel.Name = "actionsPanel";
            actionsPanel.Padding = new Padding(0, 0, 0, 6);
            actionsPanel.Size = new Size(776, 46);
            actionsPanel.TabIndex = 7;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 247, 250);
            ClientSize = new Size(800, 488);
            Controls.Add(actionsPanel);
            Controls.Add(headerPanel);
            Controls.Add(statusStrip1);
            Controls.Add(dataGridView1);
            Font = new Font("Segoe UI", 9F);
            MinimumSize = new Size(600, 300);
            Name = "MainForm";
            Text = "Менеджер задач — TaskManager";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            gridContextMenu.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            headerPanel.ResumeLayout(false);
            headerPanel.PerformLayout();
            actionsPanel.ResumeLayout(false);
            actionsPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}