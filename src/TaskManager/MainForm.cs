using System.Net.Http.Json;

namespace TaskManager
{
    public partial class MainForm : Form
    {
        private readonly HttpClient _httpClient = new();
        private readonly string _apiBaseUrl;
        private bool _isLoading;

        public MainForm()
        {
            InitializeComponent();
            _apiBaseUrl = "http://localhost:5233/api/tasks";
            Shown += async (_, _) => await LoadTasksAsync();
            UpdateActionButtonsState();
        }

        private async Task LoadTasksAsync()
        {
            if (_isLoading) return;
            _isLoading = true;
            statusLabel.Text = "Загрузка задач...";
            SetActionButtonsEnabled(false);

            try
            {
                var tasks = await _httpClient.GetFromJsonAsync<List<TaskItem>>(_apiBaseUrl);
                dataGridView1.DataSource = tasks?.ToList() ?? new List<TaskItem>();
                FormatDataGridView();
                statusLabel.Text = $"Задач: {tasks?.Count ?? 0}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Не удалось загрузить задачи:\n{ex.Message}",
                    "Ошибка подключения",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                statusLabel.Text = "Ошибка загрузки";
            }
            finally
            {
                _isLoading = false;
                UpdateActionButtonsState();
            }
        }

        private void FormatDataGridView()
        {
            if (dataGridView1.Columns.Count == 0) return;

            dataGridView1.Columns["Id"].Visible = false;
            dataGridView1.Columns["Title"].HeaderText = "Название";
            dataGridView1.Columns["Description"].HeaderText = "Описание";
            dataGridView1.Columns["IsDone"].HeaderText = "Выполнено";
            dataGridView1.Columns["DueDate"].HeaderText = "Создано";

            if (dataGridView1.Columns["DueDate"] is DataGridViewTextBoxColumn dateCol)
                dateCol.DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";

            if (dataGridView1.Columns["IsDone"] is DataGridViewCheckBoxColumn checkCol)
                checkCol.FlatStyle = FlatStyle.Standard;
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            var form = new AddTaskForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    statusLabel.Text = "Создание задачи...";
                    SetActionButtonsEnabled(false);

                    var response = await _httpClient.PostAsJsonAsync(_apiBaseUrl, form.Task);
                    if (response.IsSuccessStatusCode)
                    {
                        await LoadTasksAsync();
                        MessageBox.Show("Задача успешно добавлена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        MessageBox.Show(
                            $"Сервер вернул ошибку {response.StatusCode}:\n{error}",
                            "Ошибка API",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении:\n{ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    UpdateActionButtonsState();
                }
            }
        }

        private async void btnRefresh_Click(object sender, EventArgs e) => await LoadTasksAsync();

        private async void btnEdit_Click(object sender, EventArgs e) => await EditSelectedTaskAsync();

        private async void btnDelete_Click(object sender, EventArgs e) => await DeleteSelectedTaskAsync();

        private async void ctxEditItem_Click(object? sender, EventArgs e) => await EditSelectedTaskAsync();

        private async void ctxDeleteItem_Click(object? sender, EventArgs e) => await DeleteSelectedTaskAsync();

        private async void dataGridView1_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                await EditSelectedTaskAsync();
        }

        private void dataGridView1_SelectionChanged(object? sender, EventArgs e)
        {
            if (!_isLoading)
                UpdateActionButtonsState();
        }

        private TaskItem? GetSelectedTask() =>
            dataGridView1.CurrentRow?.DataBoundItem as TaskItem;

        private async Task EditSelectedTaskAsync()
        {
            var selected = GetSelectedTask();
            if (selected == null)
            {
                MessageBox.Show("Выберите задачу для редактирования.", "Нет выбранной задачи",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var form = new AddTaskForm(selected);
            if (form.ShowDialog() != DialogResult.OK) return;

            var updatedTask = form.Task;
            updatedTask.Id = selected.Id;

            try
            {
                statusLabel.Text = "Сохранение изменений...";
                SetActionButtonsEnabled(false);

                var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/{selected.Id}", updatedTask);
                if (response.IsSuccessStatusCode)
                {
                    await LoadTasksAsync();
                    statusLabel.Text = "Изменения сохранены";
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Не удалось обновить задачу:\n{error}", "Ошибка API",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении:\n{ex.Message}", "Ошибка", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                UpdateActionButtonsState();
            }
        }

        private async Task DeleteSelectedTaskAsync()
        {
            var selected = GetSelectedTask();
            if (selected == null)
            {
                MessageBox.Show("Выберите задачу для удаления.", "Нет выбранной задачи",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirm = MessageBox.Show(
                $"Удалить задачу \"{selected.Title}\"?",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            try
            {
                statusLabel.Text = "Удаление задачи...";
                SetActionButtonsEnabled(false);

                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/{selected.Id}");
                if (response.IsSuccessStatusCode)
                {
                    await LoadTasksAsync();
                    statusLabel.Text = "Задача удалена";
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Не удалось удалить задачу:\n{error}", "Ошибка API",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении:\n{ex.Message}", "Ошибка", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                UpdateActionButtonsState();
            }
        }

        private void UpdateActionButtonsState()
        {
            var hasSelection = GetSelectedTask() != null;
            btnAdd.Enabled = true;
            btnRefresh.Enabled = true;
            btnEdit.Enabled = hasSelection;
            btnDelete.Enabled = hasSelection;
        }

        private void SetActionButtonsEnabled(bool enabled)
        {
            btnAdd.Enabled = enabled;
            btnRefresh.Enabled = enabled;
            btnEdit.Enabled = enabled && GetSelectedTask() != null;
            btnDelete.Enabled = enabled && GetSelectedTask() != null;
        }
    }
}