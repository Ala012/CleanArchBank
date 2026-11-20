using System.ComponentModel.DataAnnotations;
using Domain.Entities;

namespace Application.DTOs;

public sealed class CreateOperationDTO
{
    [Required(ErrorMessage = "Le type d'opération est obligatoire.")]
    [EnumDataType(typeof(OperationType), ErrorMessage = "Le type d'opération est invalide.")]
    public OperationType TypeOperation { get; set; }

    [Range(typeof(decimal), "0.01", "999999999999.99", ErrorMessage = "Le montant doit être supérieur à zéro.")]
    public decimal Montant { get; set; }

    [StringLength(250)]
    public string? Libelle { get; set; }
}
