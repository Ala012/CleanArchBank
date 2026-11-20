using Domain.Entities;

namespace Application.Abstractions.Persistence;

/// <summary>Contrat de persistance des clients (implémenté par l'Infrastructure).</summary>
public interface IClientRepository
{
    Task<IReadOnlyList<Client>> GetAllAsync(CancellationToken ct = default);

    Task<Client?> GetByIdAsync(int clientId, CancellationToken ct = default);

    Task<Client?> GetByEmailAsync(string email, CancellationToken ct = default);

    Task<Client> AddAsync(Client client, CancellationToken ct = default);
}
