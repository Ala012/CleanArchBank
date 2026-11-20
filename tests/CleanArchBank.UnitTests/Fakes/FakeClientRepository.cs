using Application.Abstractions.Persistence;
using Domain.Entities;

namespace CleanArchBank.UnitTests.Fakes;

internal sealed class FakeClientRepository : IClientRepository
{
    public List<Client> Clients { get; } = [];

    public Task<IReadOnlyList<Client>> GetAllAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Client>>(Clients);

    public Task<Client?> GetByIdAsync(int clientId, CancellationToken ct = default)
        => Task.FromResult(Clients.FirstOrDefault(c => c.Id == clientId));

    public Task<Client?> GetByEmailAsync(string email, CancellationToken ct = default)
        => Task.FromResult(Clients.FirstOrDefault(c =>
            string.Equals(c.Email, email, StringComparison.OrdinalIgnoreCase)));

    public Task<Client> AddAsync(Client client, CancellationToken ct = default)
    {
        client.Id = Clients.Count == 0 ? 1 : Clients.Max(c => c.Id) + 1;
        Clients.Add(client);
        return Task.FromResult(client);
    }
}
