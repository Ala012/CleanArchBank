using Domain.Entities;

namespace Application.DTOs;

public class OperationDTO
{
    public int Id { get; set; }
    public OperationType TypeOperation { get; set; }
    public decimal Montant { get; set; }
    public DateTime DateOperation { get; set; }
    public int CompteId { get; set; }
}