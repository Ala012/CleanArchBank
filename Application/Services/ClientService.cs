using Application.Abstractions.Persistence;
using Application.Abstractions.Services;
using Application.Common;
using Application.DTOs;
using Application.Mappings;
using Domain.Entities;

namespace Application.Services;

public sealed class ClientService(IClientRepository clients) : IClientService
{
    public async Task<Result<IReadOnlyList<ClientDTO>>> GetAllAsync(CancellationToken ct = default)
    {
        var liste = await clients.GetAllAsync(ct);
        return Result<IReadOnlyList<ClientDTO>>.Success(liste.Select(c => c.ToDto()).ToList());
    }

    public async Task<Result<ClientDTO>> GetByIdAsync(int clientId, CancellationToken ct = default)
    {
        var client = await clients.GetByIdAsync(clientId, ct);
        return client is null
            ? Result<ClientDTO>.Failure($"Le client {clientId} n'existe pas.", ErrorType.NotFound)
            : Result<ClientDTO>.Success(client.ToDto());
    }

    public async Task<Result<ClientDTO>> CreateAsync(CreateClientDTO request, CancellationToken ct = default)
    {
        var email = request.Email.Trim();
        var existant = await clients.GetByEmailAsync(email, ct);
        if (existant is not null)
        {
            return Result<ClientDTO>.Failure(
                $"Un client possède déjà l'adresse e-mail {email}.", ErrorType.Conflict);
        }

        var client = new Client
        {
            Nom = request.Nom.Trim(),
            Prenom = request.Prenom.Trim(),
            Email = email,
            Tel = request.Tel?.Trim(),
            Adresse = request.Adresse?.Trim(),
            Image = request.Image?.Trim(),
            DateInscription = DateTime.UtcNow
        };

        await clients.AddAsync(client, ct);
        return Result<ClientDTO>.Success(client.ToDto());
    }
}
