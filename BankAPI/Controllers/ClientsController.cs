using Application.Abstractions.Services;
using Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BankAPI.Controllers;

/// <summary>Gestion des clients de la banque.</summary>
[ApiController]
[Route("api/clients")]
[Produces("application/json")]
public sealed class ClientsController(IClientService service) : ApiControllerBase
{
    /// <summary>Liste tous les clients avec le nombre de comptes et leur solde total.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ClientDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ClientDTO>>> GetAll(CancellationToken ct)
        => OkOrProblem(await service.GetAllAsync(ct));

    /// <summary>Retourne un client par son identifiant.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ClientDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClientDTO>> GetById(int id, CancellationToken ct)
        => OkOrProblem(await service.GetByIdAsync(id, ct));

    /// <summary>Crée un nouveau client.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ClientDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ClientDTO>> Create(
        [FromBody] CreateClientDTO request,
        CancellationToken ct)
    {
        var result = await service.CreateAsync(request, ct);
        if (!result.IsSuccess)
        {
            return Problem(result);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }
}
