using System.ComponentModel;

namespace TaskManager
{
    partial class AddTaskForm
    {
        private Button btnOK;
        private Button btnCancel;
        private TextBox txtTitle;
        private TextBox txtDescription;
        private CheckBox chkCompleted;
        private Label label1;
        private Label label2;
        private Label label3;
        private DateTimePicker dtpDueDate;
        private Panel panelCard;
        private Label lblHeader;

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelCard = new Panel();
            this.btnCancel = new Button();
            this.btnOK = new Button();
            this.chkCompleted = new CheckBox();
            this.dtpDueDate = new DateTimePicker();
            this.label3 = new Label();
            this.txtDescription = new TextBox();
            this.label2 = new Label();
            this.txtTitle = new TextBox();
            this.label1 = new Label();
            this.lblHeader = new Label();
            this.panelCard.SuspendLayout();
            this.SuspendLayout();

            // panelCard
            this.panelCard.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.panelCard.BackColor = Color.White;
            this.panelCard.Location = new Point(12, 12);
            this.panelCard.Name = "panelCard";
            this.panelCard.Padding = new Padding(20);
            this.panelCard.Size = new Size(416, 366);
            this.panelCard.TabIndex = 0;
            this.panelCard.Controls.Add(this.btnCancel);
            this.panelCard.Controls.Add(this.btnOK);
            this.panelCard.Controls.Add(this.chkCompleted);
            this.panelCard.Controls.Add(this.dtpDueDate);
            this.panelCard.Controls.Add(this.label3);
            this.panelCard.Controls.Add(this.txtDescription);
            this.panelCard.Controls.Add(this.label2);
            this.panelCard.Controls.Add(this.txtTitle);
            this.panelCard.Controls.Add(this.label1);
            this.panelCard.Controls.Add(this.lblHeader);

            // btnCancel
            this.btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnCancel.FlatStyle = FlatStyle.Flat;
            this.btnCancel.DialogResult = DialogResult.Cancel;
            this.btnCancel.Location = new Point(266, 320);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(110, 32);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;

            // btnOK
            this.btnOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnOK.BackColor = Color.FromArgb(55, 125, 255);
            this.btnOK.FlatStyle = FlatStyle.Flat;
            this.btnOK.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.btnOK.ForeColor = Color.White;
            this.btnOK.Location = new Point(150, 320);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new Size(110, 32);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "Создать";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += btnOK_Click;

            // chkCompleted
            this.chkCompleted.AutoSize = true;
            this.chkCompleted.Location = new Point(20, 285);
            this.chkCompleted.Name = "chkCompleted";
            this.chkCompleted.Size = new Size(209, 19);
            this.chkCompleted.TabIndex = 3;
            this.chkCompleted.Text = "Отметить как выполненную";
            this.chkCompleted.UseVisualStyleBackColor = true;

            // dtpDueDate
            this.dtpDueDate.CustomFormat = "dd MMMM yyyy HH:mm";
            this.dtpDueDate.Format = DateTimePickerFormat.Custom;
            this.dtpDueDate.Location = new Point(20, 252);
            this.dtpDueDate.Name = "dtpDueDate";
            this.dtpDueDate.Size = new Size(372, 23);
            this.dtpDueDate.TabIndex = 2;

            // label3
            this.label3.AutoSize = true;
            this.label3.Location = new Point(20, 232);
            this.label3.Name = "label3";
            this.label3.Size = new Size(96, 15);
            this.label3.Text = "Дата выполнения";

            // txtDescription
            this.txtDescription.AcceptsReturn = true;
            this.txtDescription.Location = new Point(20, 135);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ScrollBars = ScrollBars.Vertical;
            this.txtDescription.Size = new Size(372, 90);
            this.txtDescription.TabIndex = 1;

            // label2
            this.label2.AutoSize = true;
            this.label2.Location = new Point(20, 115);
            this.label2.Name = "label2";
            this.label2.Size = new Size(66, 15);
            this.label2.Text = "Описание";

            // txtTitle
            this.txtTitle.Location = new Point(20, 80);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new Size(372, 23);
            this.txtTitle.TabIndex = 0;

            // label1
            this.label1.AutoSize = true;
            this.label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.label1.Location = new Point(20, 60);
            this.label1.Name = "label1";
            this.label1.Size = new Size(77, 15);
            this.label1.Text = "Название *";

            // lblHeader
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold);
            this.lblHeader.ForeColor = Color.FromArgb(23, 43, 77);
            this.lblHeader.Location = new Point(20, 20);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new Size(203, 25);
            this.lblHeader.Text = "Создание новой задачи";

            // AddTaskForm
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.ClientSize = new Size(440, 390);
            this.Controls.Add(this.panelCard);
            this.Font = new Font("Segoe UI", 9F);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddTaskForm";
            this.Padding = new Padding(0, 0, 0, 10);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Новая задача";
            this.panelCard.ResumeLayout(false);
            this.panelCard.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}