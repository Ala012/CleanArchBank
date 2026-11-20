using Application.Abstractions.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Implémentation EF Core du repository des comptes.
/// Les opérations sensibles (retrait/versement, virement) sont exécutées
/// dans une transaction SQL unique afin de garantir l'atomicité.
/// </summary>
public sealed class CompteRepository(BankDbContext db) : ICompteRepository
{
    public async Task<IReadOnlyList<Compte>> GetAllAsync(CancellationToken ct = default)
        => await db.Comptes
            .AsNoTracking()
            .Include(c => c.Client)
            .OrderBy(c => c.DateCreation)
            .ThenBy(c => c.Id)
            .ToListAsync(ct);

    public async Task<Compte?> GetByIdAsync(int compteId, CancellationToken ct = default)
        => await db.Comptes
            .Include(c => c.Client)
            .FirstOrDefaultAsync(c => c.Id == compteId, ct);

    public async Task<IReadOnlyList<Compte>> SearchByClientAsync(int clientId, CancellationToken ct = default)
        => await db.Comptes
            .AsNoTracking()
            .Include(c => c.Client)
            .Where(c => c.ClientId == clientId)
            .OrderBy(c => c.DateCreation)
            .ToListAsync(ct);

    public async Task<Compte> AddAsync(Compte compte, CancellationToken ct = default)
    {
        db.Comptes.Add(compte);
        await db.SaveChangesAsync(ct);
        return compte;
    }

    public async Task UpdateAsync(Compte compte, CancellationToken ct = default)
    {
        var entry = db.Entry(compte);
        if (entry.State == EntityState.Detached)
        {
            db.Comptes.Attach(compte);
        }

        entry.State = EntityState.Modified;
        await db.SaveChangesAsync(ct);
    }

    public async Task<bool> DeleteAsync(int compteId, CancellationToken ct = default)
    {
        var compte = await db.Comptes.FindAsync(new object[] { compteId }, ct);
        if (compte is null)
        {
            return false;
        }

        db.Comptes.Remove(compte);
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<int> CountOperationsAsync(int compteId, CancellationToken ct = default)
        => await db.Operations.CountAsync(o => o.CompteId == compteId, ct);

    public async Task<int> CountOperationsByClientAsync(int clientId, CancellationToken ct = default)
        => await db.Operations.CountAsync(o => o.Compte.ClientId == clientId, ct);

    public async Task<Operation> AddOperationAsync(
        Operation operation,
        decimal nouveauSolde,
        CancellationToken ct = default)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(ct);

        var compte = await db.Comptes.FirstOrDefaultAsync(c => c.Id == operation.CompteId, ct)
            ?? throw new InvalidOperationException($"Le compte {operation.CompteId} n'existe pas.");

        compte.AjusterSolde(nouveauSolde);
        db.Operations.Add(operation);

        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return operation;
    }

    public async Task TransferAsync(
        Operation operationSource,
        Operation operationDestination,
        decimal nouveauSoldeSource,
        decimal nouveauSoldeDestination,
        CancellationToken ct = default)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(ct);

        var source = await db.Comptes.FirstOrDefaultAsync(c => c.Id == operationSource.CompteId, ct)
            ?? throw new InvalidOperationException(
                $"Le compte source {operationSource.CompteId} n'existe pas.");
        var destination = await db.Comptes.FirstOrDefaultAsync(c => c.Id == operationDestination.CompteId, ct)
            ?? throw new InvalidOperationException(
                $"Le compte destination {operationDestination.CompteId} n'existe pas.");

        source.AjusterSolde(nouveauSoldeSource);
        destination.AjusterSolde(nouveauSoldeDestination);
        db.Operations.AddRange(operationSource, operationDestination);

        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
    }

    public async Task<IReadOnlyList<Operation>> GetStatementAsync(
        int compteId,
        DateTime from,
        CancellationToken ct = default)
        => await db.Operations
            .AsNoTracking()
            .Where(o => o.CompteId == compteId && o.DateOperation >= from)
            .OrderBy(o => o.DateOperation)
            .ThenBy(o => o.Id)
            .ToListAsync(ct);

    public async Task<decimal> GetTotalBalanceAsync(int clientId, CancellationToken ct = default)
    {
        // Somme côté client : évite les conversions de type décimal propres à chaque provider MySQL.
        var soldes = await db.Comptes
            .AsNoTracking()
            .Where(c => c.ClientId == clientId)
            .Select(c => c.Solde)
            .ToListAsync(ct);

        return soldes.Sum();
    }

    public async Task<decimal> GetTotalWithdrawalsAsync(int clientId, CancellationToken ct = default)
    {
        var montants = await db.Operations
            .AsNoTracking()
            .Where(o => o.Compte.ClientId == clientId && o.TypeOperation == OperationType.Retrait)
            .Select(o => o.Montant)
            .ToListAsync(ct);

        return montants.Sum();
    }
}
