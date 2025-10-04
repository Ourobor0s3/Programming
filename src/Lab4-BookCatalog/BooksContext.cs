using Lab4_BookCatalog;
using Microsoft.EntityFrameworkCore;

public class BooksContext : DbContext
{
    public DbSet<Book> Books { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Database=bookcatalog;Port=5433;User ID=postgres;Password=admin;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.ISBN);
            entity.Property(e => e.ISBN).IsRequired();
            entity.Property(e => e.Title).IsRequired().HasColumnType("text");
            entity.Property(e => e.Author).IsRequired().HasColumnType("text");
        });
    }
}