using Application.Abstractions.Persistence;
using Application.Abstractions.Services;
using Application.Common;
using Application.DTOs;
using Application.Mappings;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.Services;

/// <summary>
/// Implémentation des cas d'utilisation. Les règles métier (solde suffisant,
/// interdiction de supprimer un compte mouvementé, etc.) sont appliquées ici
/// et dans les entités du domaine, jamais dans le contrôleur ni dans EF Core.
/// </summary>
public sealed class CompteService(
    ICompteRepository comptes,
    IClientRepository clients) : ICompteService
{
    private static readonly DateTime DatePlancher = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public async Task<Result<IReadOnlyList<CompteDTO>>> GetAllAsync(CancellationToken ct = default)
    {
        var liste = await comptes.GetAllAsync(ct);
        return Result<IReadOnlyList<CompteDTO>>.Success(liste.Select(c => c.ToDto()).ToList());
    }

    public async Task<Result<CompteDTO>> GetByIdAsync(int compteId, CancellationToken ct = default)
    {
        var compte = await comptes.GetByIdAsync(compteId, ct);
        return compte is null
            ? Result<CompteDTO>.Failure($"Le compte {compteId} n'existe pas.", ErrorType.NotFound)
            : Result<CompteDTO>.Success(compte.ToDto());
    }

    public async Task<Result<IReadOnlyList<CompteDTO>>> SearchByClientAsync(
        int clientId,
        CancellationToken ct = default)
    {
        var client = await clients.GetByIdAsync(clientId, ct);
        if (client is null)
        {
            return Result<IReadOnlyList<CompteDTO>>.Failure(
                $"Le client {clientId} n'existe pas.", ErrorType.NotFound);
        }

        var liste = await comptes.SearchByClientAsync(clientId, ct);
        return Result<IReadOnlyList<CompteDTO>>.Success(liste.Select(c => c.ToDto()).ToList());
    }

    public async Task<Result<CompteDTO>> CreateAsync(CreateCompteDTO request, CancellationToken ct = default)
    {
        var client = await clients.GetByIdAsync(request.ClientId, ct);
        if (client is null)
        {
            return Result<CompteDTO>.Failure(
                $"Le client {request.ClientId} n'existe pas : impossible de créer le compte.",
                ErrorType.Validation);
        }

        try
        {
            var compte = new Compte(
                request.ClientId,
                request.NumeroCompte,
                request.TypeCompte,
                request.Solde);

            await comptes.AddAsync(compte, ct);
            compte.Client = client;
            return Result<CompteDTO>.Success(compte.ToDto());
        }
        catch (DomainException ex)
        {
            return Result<CompteDTO>.Failure(ex.Message, ErrorType.Validation);
        }
    }

    public async Task<Result<CompteDTO>> UpdateAsync(
        int compteId,
        UpdateCompteDTO request,
        CancellationToken ct = default)
    {
        var compte = await comptes.GetByIdAsync(compteId, ct);
        if (compte is null)
        {
            return Result<CompteDTO>.Failure($"Le compte {compteId} n'existe pas.", ErrorType.NotFound);
        }

        var client = await clients.GetByIdAsync(request.ClientId, ct);
        if (client is null)
        {
            return Result<CompteDTO>.Failure(
                $"Le client {request.ClientId} n'existe pas.", ErrorType.Validation);
        }

        try
        {
            compte.NumeroCompte = request.NumeroCompte;
            compte.TypeCompte = request.TypeCompte.Trim();
            compte.ClientId = request.ClientId;

            await comptes.UpdateAsync(compte, ct);
            compte.Client = client;
            return Result<CompteDTO>.Success(compte.ToDto());
        }
        catch (DomainException ex)
        {
            return Result<CompteDTO>.Failure(ex.Message, ErrorType.Validation);
        }
    }

    public async Task<Result> DeleteAsync(int compteId, CancellationToken ct = default)
    {
        var compte = await comptes.GetByIdAsync(compteId, ct);
        if (compte is null)
        {
            return Result.Failure($"Le compte {compteId} n'existe pas.", ErrorType.NotFound);
        }

        var nombreOperations = await comptes.CountOperationsAsync(compteId, ct);
        if (nombreOperations > 0)
        {
            return Result.Failure(
                $"Suppression interdite : le compte {compteId} possède {nombreOperations} opération(s).",
                ErrorType.Conflict);
        }

        await comptes.DeleteAsync(compteId, ct);
        return Result.Success();
    }

    public async Task<Result<OperationDTO>> CreateOperationAsync(
        int compteId,
        CreateOperationDTO request,
        CancellationToken ct = default)
    {
        if (request.TypeOperation == OperationType.Virement)
        {
            return Result<OperationDTO>.Failure(
                "Un virement doit être effectué via l'endpoint POST /api/comptes/virement.",
                ErrorType.Validation);
        }

        var compte = await comptes.GetByIdAsync(compteId, ct);
        if (compte is null)
        {
            return Result<OperationDTO>.Failure($"Le compte {compteId} n'existe pas.", ErrorType.NotFound);
        }

        try
        {
            switch (request.TypeOperation)
            {
                case OperationType.Retrait:
                    compte.Debiter(request.Montant);
                    break;
                case OperationType.Versement:
                    compte.Crediter(request.Montant);
                    break;
                default:
                    return Result<OperationDTO>.Failure(
                        "Type d'opération non pris en charge.", ErrorType.Validation);
            }

            var operation = new Operation
            {
                CompteId = compteId,
                TypeOperation = request.TypeOperation,
                Montant = request.Montant,
                DateOperation = DateTime.UtcNow,
                Libelle = request.Libelle
            };

            await comptes.AddOperationAsync(operation, compte.Solde, ct);
            return Result<OperationDTO>.Success(operation.ToDto());
        }
        catch (SoldeInsuffisantException ex)
        {
            return Result<OperationDTO>.Failure(ex.Message, ErrorType.Validation);
        }
        catch (DomainException ex)
        {
            return Result<OperationDTO>.Failure(ex.Message, ErrorType.Validation);
        }
    }

    public async Task<Result<int>> CountOperationsByClientAsync(int clientId, CancellationToken ct = default)
    {
        var client = await clients.GetByIdAsync(clientId, ct);
        if (client is null)
        {
            return Result<int>.Failure($"Le client {clientId} n'existe pas.", ErrorType.NotFound);
        }

        var nombre = await comptes.CountOperationsByClientAsync(clientId, ct);
        return Result<int>.Success(nombre);
    }

    public async Task<Result<int>> CountOperationsByAccountAsync(int compteId, CancellationToken ct = default)
    {
        var compte = await comptes.GetByIdAsync(compteId, ct);
        if (compte is null)
        {
            return Result<int>.Failure($"Le compte {compteId} n'existe pas.", ErrorType.NotFound);
        }

        var nombre = await comptes.CountOperationsAsync(compteId, ct);
        return Result<int>.Success(nombre);
    }

    public async Task<Result<IReadOnlyList<OperationDTO>>> GetStatementAsync(
        int compteId,
        DateTime? from,
        CancellationToken ct = default)
    {
        var compte = await comptes.GetByIdAsync(compteId, ct);
        if (compte is null)
        {
            return Result<IReadOnlyList<OperationDTO>>.Failure(
                $"Le compte {compteId} n'existe pas.", ErrorType.NotFound);
        }

        var dateDebut = from is null || from == default
            ? DatePlancher
            : NormaliserUtc(from.Value);

        var operations = await comptes.GetStatementAsync(compteId, dateDebut, ct);
        return Result<IReadOnlyList<OperationDTO>>.Success(operations.Select(o => o.ToDto()).ToList());
    }

    public async Task<Result<decimal>> GetTotalBalanceAsync(int clientId, CancellationToken ct = default)
    {
        var client = await clients.GetByIdAsync(clientId, ct);
        if (client is null)
        {
            return Result<decimal>.Failure($"Le client {clientId} n'existe pas.", ErrorType.NotFound);
        }

        var total = await comptes.GetTotalBalanceAsync(clientId, ct);
        return Result<decimal>.Success(total);
    }

    public async Task<Result> TransferAsync(VirementDTO request, CancellationToken ct = default)
    {
        if (request.CompteSourceId == request.CompteDestinationId)
        {
            return Result.Failure(
                "Les comptes source et destination doivent être différents.", ErrorType.Validation);
        }

        var source = await comptes.GetByIdAsync(request.CompteSourceId, ct);
        if (source is null)
        {
            return Result.Failure(
                $"Le compte source {request.CompteSourceId} n'existe pas.", ErrorType.NotFound);
        }

        var destination = await comptes.GetByIdAsync(request.CompteDestinationId, ct);
        if (destination is null)
        {
            return Result.Failure(
                $"Le compte destination {request.CompteDestinationId} n'existe pas.", ErrorType.NotFound);
        }

        try
        {
            source.Debiter(request.Montant);
            destination.Crediter(request.Montant);
        }
        catch (SoldeInsuffisantException ex)
        {
            return Result.Failure(ex.Message, ErrorType.Validation);
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message, ErrorType.Validation);
        }

        var operationSource = new Operation
        {
            CompteId = source.Id,
            TypeOperation = OperationType.Virement,
            Montant = request.Montant,
            DateOperation = DateTime.UtcNow,
            Libelle = request.Libelle ?? $"Virement vers le compte {destination.NumeroCompte}"
        };

        var operationDestination = new Operation
        {
            CompteId = destination.Id,
            TypeOperation = OperationType.Virement,
            Montant = request.Montant,
            DateOperation = DateTime.UtcNow,
            Libelle = request.Libelle ?? $"Virement depuis le compte {source.NumeroCompte}"
        };

        await comptes.TransferAsync(
            operationSource,
            operationDestination,
            source.Solde,
            destination.Solde,
            ct);

        return Result.Success();
    }

    public async Task<Result<decimal>> GetTotalWithdrawalsAsync(int clientId, CancellationToken ct = default)
    {
        var client = await clients.GetByIdAsync(clientId, ct);
        if (client is null)
        {
            return Result<decimal>.Failure($"Le client {clientId} n'existe pas.", ErrorType.NotFound);
        }

        var total = await comptes.GetTotalWithdrawalsAsync(clientId, ct);
        return Result<decimal>.Success(total);
    }

    private static DateTime NormaliserUtc(DateTime date) => date.Kind switch
    {
        DateTimeKind.Utc => date,
        DateTimeKind.Local => date.ToUniversalTime(),
        _ => DateTime.SpecifyKind(date, DateTimeKind.Utc)
    };
}
