using System.Text.Json;
using Microsoft.Extensions.Options;

namespace TaskManager.Api.Services;

public class ExportBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<ExportBackgroundService> _logger;
    private readonly ExportServiceOptions _options;
    private readonly IWebHostEnvironment _environment;

    public ExportBackgroundService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<ExportBackgroundService> logger,
        IOptions<ExportServiceOptions> options,
        IWebHostEnvironment environment)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _options = options.Value;
        _environment = environment;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ExportBackgroundService запущен. Интервал экспорта: {IntervalSeconds} секунд", 
            _options.ExportIntervalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ExportTasksAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при экспорте задач");
            }

            await Task.Delay(TimeSpan.FromSeconds(_options.ExportIntervalSeconds), stoppingToken);
        }

        _logger.LogInformation("ExportBackgroundService остановлен");
    }

    private async Task ExportTasksAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var taskService = scope.ServiceProvider.GetRequiredService<ITaskService>();

        var tasks = await taskService.GetAllAsync();
        var filePath = Path.Combine(_environment.ContentRootPath, _options.ExportFilePath);

        var json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });

        await File.WriteAllTextAsync(filePath, json, cancellationToken);

        _logger.LogInformation("Экспортировано {Count} задач в файл {FilePath}", tasks.Count, filePath);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Остановка ExportBackgroundService...");
        await base.StopAsync(cancellationToken);
        _logger.LogInformation("ExportBackgroundService остановлен");
    }
}

public class ExportServiceOptions
{
    public const string SectionName = "ExportService";
    
    public int ExportIntervalSeconds { get; set; } = 60;
    public string ExportFilePath { get; set; } = "tasks_export.json";
}