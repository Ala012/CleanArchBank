using Application.Abstractions.Persistence;
using Domain.Entities;

namespace CleanArchBank.UnitTests.Fakes;

internal sealed class FakeCompteRepository : ICompteRepository
{
    public List<Compte> Comptes { get; } = [];

    public List<Operation> Operations { get; } = [];

    public decimal? DernierSoldeEnregistre { get; private set; }

    public bool DeleteAppele { get; private set; }

    public bool AddOperationAppele { get; private set; }

    public bool TransferAppele { get; private set; }

    public Task<IReadOnlyList<Compte>> GetAllAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Compte>>(Comptes.OrderBy(c => c.DateCreation).ToList());

    public Task<Compte?> GetByIdAsync(int compteId, CancellationToken ct = default)
        => Task.FromResult(Comptes.FirstOrDefault(c => c.Id == compteId));

    public Task<IReadOnlyList<Compte>> SearchByClientAsync(int clientId, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Compte>>(
            Comptes.Where(c => c.ClientId == clientId).ToList());

    public Task<Compte> AddAsync(Compte compte, CancellationToken ct = default)
    {
        compte.Id = Comptes.Count == 0 ? 1 : Comptes.Max(c => c.Id) + 1;
        Comptes.Add(compte);
        return Task.FromResult(compte);
    }

    public Task UpdateAsync(Compte compte, CancellationToken ct = default) => Task.CompletedTask;

    public Task<bool> DeleteAsync(int compteId, CancellationToken ct = default)
    {
        DeleteAppele = true;
        var compte = Comptes.FirstOrDefault(c => c.Id == compteId);
        if (compte is null)
        {
            return Task.FromResult(false);
        }

        Comptes.Remove(compte);
        return Task.FromResult(true);
    }

    public Task<int> CountOperationsAsync(int compteId, CancellationToken ct = default)
        => Task.FromResult(Operations.Count(o => o.CompteId == compteId));

    public Task<int> CountOperationsByClientAsync(int clientId, CancellationToken ct = default)
        => Task.FromResult(Operations.Count(o =>
            Comptes.Any(c => c.Id == o.CompteId && c.ClientId == clientId)));

    public Task<Operation> AddOperationAsync(
        Operation operation,
        decimal nouveauSolde,
        CancellationToken ct = default)
    {
        AddOperationAppele = true;
        DernierSoldeEnregistre = nouveauSolde;
        operation.Id = Operations.Count + 1;
        Operations.Add(operation);
        return Task.FromResult(operation);
    }

    public Task TransferAsync(
        Operation operationSource,
        Operation operationDestination,
        decimal nouveauSoldeSource,
        decimal nouveauSoldeDestination,
        CancellationToken ct = default)
    {
        TransferAppele = true;
        DernierSoldeEnregistre = nouveauSoldeSource;
        Operations.Add(operationSource);
        Operations.Add(operationDestination);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Operation>> GetStatementAsync(
        int compteId,
        DateTime from,
        CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Operation>>(
            Operations.Where(o => o.CompteId == compteId && o.DateOperation >= from).ToList());

    public Task<decimal> GetTotalBalanceAsync(int clientId, CancellationToken ct = default)
        => Task.FromResult(Comptes.Where(c => c.ClientId == clientId).Sum(c => c.Solde));

    public Task<decimal> GetTotalWithdrawalsAsync(int clientId, CancellationToken ct = default)
        => Task.FromResult(Operations
            .Where(o => o.TypeOperation == OperationType.Retrait &&
                        Comptes.Any(c => c.Id == o.CompteId && c.ClientId == clientId))
            .Sum(o => o.Montant));
}
