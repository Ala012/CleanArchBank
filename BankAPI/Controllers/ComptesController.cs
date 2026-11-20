using Application.Abstractions.Services;
using Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BankAPI.Controllers;

/// <summary>Gestion des comptes bancaires et des opérations.</summary>
[ApiController]
[Route("api/comptes")]
[Produces("application/json")]
public sealed class ComptesController(ICompteService service) : ApiControllerBase
{
    /// <summary>Liste tous les comptes, triés par date de création, avec leur client.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CompteDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CompteDTO>>> GetAll(CancellationToken ct)
        => OkOrProblem(await service.GetAllAsync(ct));

    /// <summary>Retourne un compte par son identifiant.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CompteDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CompteDTO>> GetById(int id, CancellationToken ct)
        => OkOrProblem(await service.GetByIdAsync(id, ct));

    /// <summary>Crée un compte après vérification que le client existe.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CompteDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CompteDTO>> Create(
        [FromBody] CreateCompteDTO request,
        CancellationToken ct)
    {
        var result = await service.CreateAsync(request, ct);
        if (!result.IsSuccess)
        {
            return Problem(result);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>Met à jour les données d'un compte.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(CompteDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CompteDTO>> Update(
        int id,
        [FromBody] UpdateCompteDTO request,
        CancellationToken ct)
        => OkOrProblem(await service.UpdateAsync(id, request, ct));

    /// <summary>Supprime un compte sans opération. Retourne 409 si le compte est mouvementé.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Delete(int id, CancellationToken ct)
        => NoContentOrProblem(await service.DeleteAsync(id, ct));

    /// <summary>Recherche les comptes d'un client.</summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IReadOnlyList<CompteDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<CompteDTO>>> Search(
        [FromQuery] int clientId,
        CancellationToken ct)
        => OkOrProblem(await service.SearchByClientAsync(clientId, ct));

    /// <summary>
    /// Ajoute une opération de retrait ou de versement à un compte.
    /// Un retrait supérieur au solde retourne 400 Bad Request.
    /// </summary>
    [HttpPost("{compteId:int}/operations")]
    [ProducesResponseType(typeof(OperationDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OperationDTO>> CreateOperation(
        int compteId,
        [FromBody] CreateOperationDTO request,
        CancellationToken ct)
    {
        var result = await service.CreateOperationAsync(compteId, request, ct);
        if (!result.IsSuccess)
        {
            return Problem(result);
        }

        return StatusCode(StatusCodes.Status201Created, result.Value);
    }

    /// <summary>Nombre d'opérations réalisées par un client (tous ses comptes).</summary>
    [HttpGet("{id:int}/nombre-operations")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<int>> NombreOperations(int id, CancellationToken ct)
        => OkOrProblem(await service.CountOperationsByClientAsync(id, ct));

    /// <summary>Nombre d'opérations réalisées sur un compte donné.</summary>
    [HttpGet("{id:int}/operations/nombre")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<int>> NombreOperationsCompte(int id, CancellationToken ct)
        => OkOrProblem(await service.CountOperationsByAccountAsync(id, ct));

    /// <summary>Extrait des opérations d'un compte à partir d'une date, triées par date.</summary>
    [HttpGet("{num:int}/extrait")]
    [ProducesResponseType(typeof(IReadOnlyList<OperationDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<OperationDTO>>> Extrait(
        int num,
        [FromQuery] DateTime? date,
        CancellationToken ct)
        => OkOrProblem(await service.GetStatementAsync(num, date, ct));

    /// <summary>Somme des soldes de tous les comptes d'un client.</summary>
    [HttpGet("solde-total/{clientId:int}")]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<decimal>> SoldeTotal(int clientId, CancellationToken ct)
        => OkOrProblem(await service.GetTotalBalanceAsync(clientId, ct));

    /// <summary>Virement atomique entre deux comptes (débit + crédit + écritures).</summary>
    [HttpPost("virement")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Virement([FromBody] VirementDTO request, CancellationToken ct)
        => NoContentOrProblem(await service.TransferAsync(request, ct));

    /// <summary>Somme des retraits effectués par un client.</summary>
    [HttpGet("total-retraits/{clientId:int}")]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<decimal>> TotalRetraits(int clientId, CancellationToken ct)
        => OkOrProblem(await service.GetTotalWithdrawalsAsync(clientId, ct));
}
