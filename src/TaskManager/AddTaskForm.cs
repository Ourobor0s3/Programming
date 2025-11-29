using System.ComponentModel;

namespace TaskManager
{
    public partial class AddTaskForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TaskItem Task { get; private set; } = new();

        private readonly bool _isEditMode;

        public AddTaskForm(TaskItem? taskToEdit = null)
        {
            InitializeComponent();
            AcceptButton = btnOK;
            CancelButton = btnCancel;

            if (taskToEdit != null)
            {
                _isEditMode = true;
                Task = new TaskItem
                {
                    Id = taskToEdit.Id,
                    Title = taskToEdit.Title,
                    Description = taskToEdit.Description,
                    DueDate = taskToEdit.DueDate,
                    IsDone = taskToEdit.IsDone
                };
            }

            Text = _isEditMode ? "Редактирование задачи" : "Новая задача";
            lblHeader.Text = _isEditMode ? "Обновление задачи" : "Создание новой задачи";
            btnOK.Text = _isEditMode ? "Сохранить" : "Создать";

            txtTitle.Text = Task.Title ?? string.Empty;
            txtDescription.Text = Task.Description ?? string.Empty;
            chkCompleted.Checked = Task.IsDone;
            dtpDueDate.Value = Task.DueDate == default ? DateTime.Now : Task.DueDate.ToLocalTime();
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
            Task.DueDate = DateTime.SpecifyKind(dtpDueDate.Value, DateTimeKind.Local);

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}