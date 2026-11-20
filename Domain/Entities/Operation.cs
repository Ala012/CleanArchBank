using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Operation
{
    public int Id { get; set; }

    [Required]
    public OperationType TypeOperation { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, 1000000000, ErrorMessage = "Le montant doit être positif.")]
    public decimal Montant { get; set; }

    public DateTime DateOperation { get; set; } = DateTime.Now;

    [Required]
    public int CompteId { get; set; }

    public Compte Compte { get; set; } = null!;
}