using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public sealed class VirementDTO
{
    [Range(1, int.MaxValue, ErrorMessage = "Le compte source est obligatoire.")]
    public int CompteSourceId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Le compte destination est obligatoire.")]
    public int CompteDestinationId { get; set; }

    [Range(typeof(decimal), "0.01", "999999999999.99", ErrorMessage = "Le montant doit être supérieur à zéro.")]
    public decimal Montant { get; set; }

    [StringLength(250)]
    public string? Libelle { get; set; }
}
