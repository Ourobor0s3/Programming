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

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.label1 = new Label();
            this.txtTitle = new TextBox();
            this.label2 = new Label();
            this.txtDescription = new TextBox();
            this.chkCompleted = new CheckBox();
            this.btnOK = new Button();
            this.btnCancel = new Button();

            this.SuspendLayout();

            // label1
            this.label1.AutoSize = true;
            this.label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.label1.Location = new Point(20, 20);
            this.label1.Name = "label1";
            this.label1.Size = new Size(73, 15);
            this.label1.Text = "Название *";

            // txtTitle
            this.txtTitle.Location = new Point(23, 40);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new Size(354, 23);
            this.txtTitle.TabIndex = 0;

            // label2
            this.label2.AutoSize = true;
            this.label2.Location = new Point(20, 75);
            this.label2.Name = "label2";
            this.label2.Size = new Size(66, 15);
            this.label2.Text = "Описание";

            // txtDescription
            this.txtDescription.AcceptsReturn = true;
            this.txtDescription.Location = new Point(23, 95);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ScrollBars = ScrollBars.Vertical;
            this.txtDescription.Size = new Size(354, 80);
            this.txtDescription.TabIndex = 1;

            // chkCompleted
            this.chkCompleted.AutoSize = true;
            this.chkCompleted.Location = new Point(23, 185);
            this.chkCompleted.Name = "chkCompleted";
            this.chkCompleted.Size = new Size(149, 19);
            this.chkCompleted.TabIndex = 2;
            this.chkCompleted.Text = "Отметить как выполненное";
            this.chkCompleted.UseVisualStyleBackColor = true;

            // btnOK
            this.btnOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnOK.DialogResult = DialogResult.OK;
            this.btnOK.Font = new Font("Segoe UI", 9F);
            this.btnOK.Location = new Point(217, 220);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new Size(75, 30);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "Создать";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += btnOK_Click;

            // btnCancel
            this.btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnCancel.DialogResult = DialogResult.Cancel;
            this.btnCancel.Font = new Font("Segoe UI", 9F);
            this.btnCancel.Location = new Point(298, 220);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(75, 30);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;

            // AddTaskForm
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(400, 260);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.chkCompleted);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtTitle);
            this.Controls.Add(this.label1);
            this.Font = new Font("Segoe UI", 9F);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddTaskForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Новая задача";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}