using Lab6_InventoryManager.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lab6_InventoryManager
{
    public class DataContext : DbContext
    {
        public const string ConnectionString = @"Host=localhost;Database=data-study;Port=5433;User ID=postgres;Password=admin;";

        public DbSet<Product> Products { get; set; }

        public DbSet<StockMovement> StockMovements { get; set; }

        public DbSet<Warehouse> Warehouses { get; set; }

        public DbSet<PriceHistory> PriceHistory { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.ProductCode);
                entity.Property(p => p.ProductCode).IsRequired().HasMaxLength(50);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
                entity.Property(p => p.Price).HasColumnType("decimal(10,2)").IsRequired();
                entity.Property(p => p.Stock).IsRequired();
            });

            // Настройка Warehouse
            modelBuilder.Entity<Warehouse>(entity =>
            {
                entity.HasKey(w => w.Id);
                entity.Property(w => w.Name).HasMaxLength(100);
            });

            modelBuilder.Entity<PriceHistory>(entity =>
            {
                entity.HasKey(ph => ph.Id);
                entity.Property(ph => ph.ProductCode).IsRequired().HasMaxLength(50);
                entity.Property(ph => ph.OldPrice).HasColumnType("decimal(10,2)").IsRequired();
                entity.Property(ph => ph.NewPrice).HasColumnType("decimal(10,2)").IsRequired();
                entity.Property(ph => ph.ChangedAt).IsRequired();
                entity.Property(ph => ph.Reason).HasMaxLength(200);

                entity.HasIndex(ph => new { ph.ProductCode, ph.ChangedAt });
            });

            // Настройка StockMovement
            modelBuilder.Entity<StockMovement>(entity =>
            {
                entity.HasKey(sm => sm.Id);
                entity.Property(sm => sm.ProductCode).IsRequired().HasMaxLength(50);
                entity.Property(sm => sm.Quantity).IsRequired();
                entity.Property(sm => sm.When).IsRequired();

                // Внешние ключи
                entity.HasOne<Product>()
                    .WithMany()
                    .HasForeignKey(sm => sm.ProductCode)
                    .OnDelete(DeleteBehavior.Restrict); // чтобы нельзя было удалить продукт, если есть движения

                entity.HasOne<Warehouse>()
                    .WithMany()
                    .HasForeignKey(sm => sm.FromWarehouseId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);

                entity.HasOne<Warehouse>()
                    .WithMany()
                    .HasForeignKey(sm => sm.ToWarehouseId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);
            });
        }
    }
}