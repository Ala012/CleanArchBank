using Domain.Entities;

namespace Application.Abstractions.Persistence;

/// <summary>
/// Contrat de persistance des comptes et opérations.
/// L'implémentation (Infrastructure) est injectée par inversion de dépendance.
/// </summary>
public interface ICompteRepository
{
    Task<IReadOnlyList<Compte>> GetAllAsync(CancellationToken ct = default);

    Task<Compte?> GetByIdAsync(int compteId, CancellationToken ct = default);

    Task<IReadOnlyList<Compte>> SearchByClientAsync(int clientId, CancellationToken ct = default);

    Task<Compte> AddAsync(Compte compte, CancellationToken ct = default);

    Task UpdateAsync(Compte compte, CancellationToken ct = default);

    Task<bool> DeleteAsync(int compteId, CancellationToken ct = default);

    Task<int> CountOperationsAsync(int compteId, CancellationToken ct = default);

    Task<int> CountOperationsByClientAsync(int clientId, CancellationToken ct = default);

    /// <summary>Ajoute une opération et met à jour le solde dans une transaction atomique.</summary>
    Task<Operation> AddOperationAsync(Operation operation, decimal nouveauSolde, CancellationToken ct = default);

    /// <summary>Effectue un virement : deux opérations + deux soldes, dans une transaction atomique.</summary>
    Task TransferAsync(
        Operation operationSource,
        Operation operationDestination,
        decimal nouveauSoldeSource,
        decimal nouveauSoldeDestination,
        CancellationToken ct = default);

    Task<IReadOnlyList<Operation>> GetStatementAsync(int compteId, DateTime from, CancellationToken ct = default);

    Task<decimal> GetTotalBalanceAsync(int clientId, CancellationToken ct = default);

    Task<decimal> GetTotalWithdrawalsAsync(int clientId, CancellationToken ct = default);
}
