using Domain.Entities;

namespace Application.DTOs;

public sealed class OperationDTO
{
    public int Id { get; set; }

    public int CompteId { get; set; }

    public OperationType TypeOperation { get; set; }

    public decimal Montant { get; set; }

    public DateTime DateOperation { get; set; }

    public string? Libelle { get; set; }
}
