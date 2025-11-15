using System.Net.Http.Json;

namespace TaskManager
{
    public partial class MainForm : Form
    {
        private readonly HttpClient _httpClient = new();
        private readonly string _apiBaseUrl;
        private bool _isLoading = false;

        public MainForm()
        {
            InitializeComponent();
            _apiBaseUrl = "http://localhost:5065/api/tasks";
            Task.Run(async () => await LoadTasksAsync()); // Асинхронная загрузка при старте
        }

        private async Task LoadTasksAsync()
        {
            if (_isLoading) return;
            _isLoading = true;
            statusLabel.Text = "Загрузка задач...";
            btnAdd.Enabled = false;
            btnRefresh.Enabled = false;

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
                btnAdd.Enabled = true;
                btnRefresh.Enabled = true;
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

            // Формат даты
            if (dataGridView1.Columns["DueDate"] is DataGridViewTextBoxColumn dateCol)
                dateCol.DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";

            // Чекбокс для IsCompleted
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
            }
        }

        private async void btnRefresh_Click(object sender, EventArgs e) => await LoadTasksAsync();
    }
}