using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Models;

namespace TaskManager.Api.Data;

public class TaskDbContext : DbContext
{
    public TaskDbContext(DbContextOptions<TaskDbContext> options)
        : base(options)
    {
    }

    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var task = modelBuilder.Entity<TaskItem>();
        task.ToTable("tasks");

        task.HasKey(t => t.Id);

        task.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(100);

        task.Property(t => t.Description)
            .HasMaxLength(500);

        task.Property(t => t.IsDone)
            .HasDefaultValue(false);

        task.Property(t => t.DueDate)
            .HasColumnType("timestamp with time zone");

        task.Property<DateTime>("CreatedAt")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAdd();
    }
}