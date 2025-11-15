using System.ComponentModel;

namespace TaskManager
{
    public partial class AddTaskForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TaskItem Task { get; private set; } = new();

        public AddTaskForm()
        {
            InitializeComponent();
            AcceptButton = btnOK;
            CancelButton = btnCancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            var title = txtTitle.Text.Trim();
            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Пожалуйста, введите название задачи.", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTitle.Focus();
                return;
            }

            Task.Title = title;
            Task.Description = txtDescription.Text.Trim();
            Task.IsDone = chkCompleted.Checked;

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}