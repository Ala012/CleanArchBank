using Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace BankAPI.Controllers;

/// <summary>Traduction des résultats applicatifs en réponses HTTP RFC 7807.</summary>
[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected ActionResult<T> OkOrProblem<T>(Result<T> result)
        => result.IsSuccess ? Ok(result.Value) : Problem(result);

    protected ActionResult NoContentOrProblem(Result result)
        => result.IsSuccess ? NoContent() : Problem(result);

    protected ObjectResult Problem(Result result)
        => CreateProblem(result.ErrorType, result.Error);

    protected ObjectResult Problem<T>(Result<T> result)
        => CreateProblem(result.ErrorType, result.Error);

    private ObjectResult CreateProblem(ErrorType errorType, string? error)
    {
        var statusCode = errorType switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unexpected => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status400BadRequest
        };

        var title = errorType switch
        {
            ErrorType.NotFound => "Ressource introuvable",
            ErrorType.Conflict => "Conflit métier",
            ErrorType.Unexpected => "Erreur interne",
            _ => "Requête invalide"
        };

        return base.Problem(
            statusCode: statusCode,
            title: title,
            detail: error);
    }
}
