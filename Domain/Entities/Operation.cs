using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

/// <summary>
/// Opération effectuée sur un compte : retrait, versement ou virement.
/// </summary>
public class Operation
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le compte est obligatoire.")]
    public int CompteId { get; set; }

    public Compte Compte { get; set; } = null!;

    [Required(ErrorMessage = "Le type d'opération est obligatoire.")]
    [EnumDataType(typeof(OperationType), ErrorMessage = "Le type d'opération est invalide.")]
    public OperationType TypeOperation { get; set; }

    [Range(0.01, 999_999_999_999.99, ErrorMessage = "Le montant doit être supérieur à zéro.")]
    public decimal Montant { get; set; }

    public DateTime DateOperation { get; set; } = DateTime.UtcNow;

    [StringLength(250, ErrorMessage = "Le libellé ne peut pas dépasser 250 caractères.")]
    public string? Libelle { get; set; }
}
