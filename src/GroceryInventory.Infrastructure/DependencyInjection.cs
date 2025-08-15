using GroceryInventory.Application.Abstractions;
using GroceryInventory.Application.Services;
using GroceryInventory.Infrastructure.Persistence;
using GroceryInventory.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GroceryInventory.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string? connectionString, bool useSqlite = true)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Missing DB connection string");

        services.AddDbContext<AppDbContext>(options =>
        {
            if (useSqlite)
                options.UseSqlite(connectionString);
            else
                options.UseSqlServer(connectionString);
        });

        // EF repositories
        services.AddScoped<IProductRepository, EfProductRepository>();
        services.AddScoped<ICategoryRepository, EfCategoryRepository>();
        services.AddScoped<IStockMovementRepository, EfStockMovementRepository>();

        // Application services
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<CategoryService>();
        services.AddScoped<StockService>();

        return services;
    }
}