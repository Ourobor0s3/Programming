using Calculator3;
using NLog;
using NLog.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

string? logRootDir = builder.Configuration["Logging:RootDir"] ?? "./logs";

GlobalDiagnosticsContext.Set("LogRootDir", logRootDir);

builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Logging.AddNLog();

builder.Services.AddTransient<Calculator>();

using var host = builder.Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Application starting...");

var calculator = host.Services.GetRequiredService<Calculator>();
calculator.Run();

logger.LogInformation("Application shutting down.");