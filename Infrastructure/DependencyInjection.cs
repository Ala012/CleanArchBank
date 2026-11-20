using Application.Abstractions.Persistence;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySql.EntityFrameworkCore.Extensions;

namespace Infrastructure;

/// <summary>Point d'entrée de la couche Infrastructure (composition root).</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DbConnection")
            ?? throw new InvalidOperationException(
                "La chaîne de connexion 'DbConnection' est absente de appsettings.json.");

        services.AddDbContext<BankDbContext>(options => options.UseMySQL(connectionString));

        services.AddScoped<ICompteRepository, CompteRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();

        return services;
    }

    /// <summary>Applique les migrations puis insère le jeu de données de démonstration.</summary>
    public static async Task InitialiserBaseAsync(
        this IServiceProvider services,
        CancellationToken ct = default)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BankDbContext>();

        await db.Database.MigrateAsync(ct);
        await DatabaseSeeder.SeedAsync(db, ct);
    }
}
