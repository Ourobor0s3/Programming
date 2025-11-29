namespace TaskManager.Api.Models
{
    public class ExportOptions
    {
        public int IntervalSeconds { get; set; } = 60;
        public string ExportPath { get; set; } = "tasks_export.json";
    }
}