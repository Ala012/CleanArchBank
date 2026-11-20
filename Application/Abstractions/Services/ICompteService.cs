using Application.Common;
using Application.DTOs;

namespace Application.Abstractions.Services;

/// <summary>Cas d'utilisation liés aux comptes bancaires.</summary>
public interface ICompteService
{
    Task<Result<IReadOnlyList<CompteDTO>>> GetAllAsync(CancellationToken ct = default);

    Task<Result<CompteDTO>> GetByIdAsync(int compteId, CancellationToken ct = default);

    Task<Result<IReadOnlyList<CompteDTO>>> SearchByClientAsync(int clientId, CancellationToken ct = default);

    Task<Result<CompteDTO>> CreateAsync(CreateCompteDTO request, CancellationToken ct = default);

    Task<Result<CompteDTO>> UpdateAsync(int compteId, UpdateCompteDTO request, CancellationToken ct = default);

    Task<Result> DeleteAsync(int compteId, CancellationToken ct = default);

    Task<Result<OperationDTO>> CreateOperationAsync(
        int compteId,
        CreateOperationDTO request,
        CancellationToken ct = default);

    Task<Result<int>> CountOperationsByClientAsync(int clientId, CancellationToken ct = default);

    Task<Result<int>> CountOperationsByAccountAsync(int compteId, CancellationToken ct = default);

    Task<Result<IReadOnlyList<OperationDTO>>> GetStatementAsync(
        int compteId,
        DateTime? from,
        CancellationToken ct = default);

    Task<Result<decimal>> GetTotalBalanceAsync(int clientId, CancellationToken ct = default);

    Task<Result> TransferAsync(VirementDTO request, CancellationToken ct = default);

    Task<Result<decimal>> GetTotalWithdrawalsAsync(int clientId, CancellationToken ct = default);
}
