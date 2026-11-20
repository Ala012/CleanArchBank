using Application.Common;
using Application.DTOs;

namespace Application.Abstractions.Services;

public interface IClientService
{
    Task<Result<IReadOnlyList<ClientDTO>>> GetAllAsync(CancellationToken ct = default);

    Task<Result<ClientDTO>> GetByIdAsync(int clientId, CancellationToken ct = default);

    Task<Result<ClientDTO>> CreateAsync(CreateClientDTO request, CancellationToken ct = default);
}
