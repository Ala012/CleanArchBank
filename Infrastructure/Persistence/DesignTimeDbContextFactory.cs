using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MySql.EntityFrameworkCore.Extensions;

namespace Infrastructure.Persistence;

/// <summary>
/// Fabrique utilisée par les outils EF Core (dotnet ef migrations ...)
/// afin de ne pas démarrer l'API lors de la génération des migrations.
/// </summary>
public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<BankDbContext>
{
    public BankDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("CLEANARCHBANK_CONNECTION")
            ?? "server=127.0.0.1;port=3307;database=cleanarchbank_db;user=root;password=root;CharSet=utf8mb4;SslMode=Disabled";

        var options = new DbContextOptionsBuilder<BankDbContext>()
            .UseMySQL(connectionString)
            .Options;

        return new BankDbContext(options);
    }
}
