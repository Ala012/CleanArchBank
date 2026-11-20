using Application.Abstractions.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public sealed class ClientRepository(BankDbContext db) : IClientRepository
{
    public async Task<IReadOnlyList<Client>> GetAllAsync(CancellationToken ct = default)
        => await db.Clients
            .AsNoTracking()
            .Include(c => c.Comptes)
            .OrderBy(c => c.Nom)
            .ThenBy(c => c.Prenom)
            .ToListAsync(ct);

    public async Task<Client?> GetByIdAsync(int clientId, CancellationToken ct = default)
        => await db.Clients
            .AsNoTracking()
            .Include(c => c.Comptes)
            .FirstOrDefaultAsync(c => c.Id == clientId, ct);

    public async Task<Client?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await db.Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Email == email, ct);

    public async Task<Client> AddAsync(Client client, CancellationToken ct = default)
    {
        db.Clients.Add(client);
        await db.SaveChangesAsync(ct);
        return client;
    }
}
