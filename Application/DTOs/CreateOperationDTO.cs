using System.ComponentModel.DataAnnotations;
using Domain.Entities;

namespace Application.DTOs;

public class CreateOperationDTO
{
    [Range(1, int.MaxValue, ErrorMessage = "CompteId invalide.")]
    public int CompteId { get; set; }

    [Required]
    public OperationType TypeOperation { get; set; }

    [Range(0.01, 1000000000, ErrorMessage = "Le montant doit être positif.")]
    public decimal Montant { get; set; }
}