using TaskManager.Api.Services;
using TaskManager.Api.Services.Impl;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

builder.Services.AddSingleton<ITaskService, TaskService>();

var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Run("http://localhost:5065");