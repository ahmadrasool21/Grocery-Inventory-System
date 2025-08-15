using GroceryInventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GroceryInventory.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(b =>
        {
            b.HasKey(p => p.Id);
            b.Property(p => p.Name).HasMaxLength(200).IsRequired();
            b.Property(p => p.Sku).HasMaxLength(100).IsRequired();
            b.HasIndex(p => p.Sku).IsUnique();
        });

        modelBuilder.Entity<Category>(b =>
        {
            b.HasKey(c => c.Id);
            b.Property(c => c.Name).HasMaxLength(100).IsRequired();
            b.HasIndex(c => c.Name).IsUnique();
        });

        modelBuilder.Entity<Supplier>(b =>
        {
            b.HasKey(s => s.Id);
            b.Property(s => s.Name).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<StockMovement>(b =>
        {
            b.HasKey(m => m.Id);
            b.Property(m => m.Quantity).HasColumnType("decimal(18,3)");
            b.HasIndex(m => new { m.ProductId, m.OccurredAt });
        });

        // Seed data (stable IDs)
        var cGroceries = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var pBanana    = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var pMilk      = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var now = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<Category>().HasData(new Category { Id = cGroceries, Name = "Groceries" });
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = pBanana, Name = "Banana", Sku = "SKU-BAN-001", CategoryId = cGroceries, Unit = "kg",  CostPrice = 1.00m, SalePrice = 1.50m, TaxRate = 0.10m, ReorderLevel = 10, IsBatchTracked = false, IsActive = true, CreatedAt = now },
            new Product { Id = pMilk,   Name = "Milk 1L", Sku = "SKU-MLK-001", CategoryId = cGroceries, Unit = "pcs", CostPrice = 0.70m, SalePrice = 1.20m, TaxRate = 0.10m, ReorderLevel = 15, IsBatchTracked = true,  IsActive = true, CreatedAt = now }
        );

        modelBuilder.Entity<StockMovement>().HasData(
            new StockMovement { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), ProductId = pBanana, Type = StockMovementType.Purchase, Quantity = 10, OccurredAt = now },
            new StockMovement { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), ProductId = pMilk,   Type = StockMovementType.Purchase, Quantity = 10, OccurredAt = now }
        );
    }
}