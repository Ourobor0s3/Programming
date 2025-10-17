using Microsoft.EntityFrameworkCore;

namespace Lab5_CostAccounting
{
    public class TransactionsContext : DbContext
    {
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Database=lab5-transaction;Port=5433;User ID=postgres;Password=admin;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).IsRequired();
                entity.Property(e => e.Date).IsRequired().HasColumnType("date");
                entity.Property(e => e.Category).HasColumnType("text");
                entity.Property(e => e.Amount).IsRequired().HasColumnType("int");
                entity.Property(e => e.Note).HasColumnType("text");
            });
        }
    }
}