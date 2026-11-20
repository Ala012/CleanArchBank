using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class CreateCompteDTO
{
    [Range(1, int.MaxValue, ErrorMessage = "Numéro de compte invalide.")]
    public int NumeroCompte { get; set; }

    [Range(0, 1000000000, ErrorMessage = "Le solde initial ne peut pas être négatif.")]
    public decimal Solde { get; set; }

    [Required(ErrorMessage = "Le type de compte est obligatoire.")]
    [StringLength(30)]
    public string TypeCompte { get; set; } = null!;

    [Range(1, int.MaxValue, ErrorMessage = "ClientId invalide.")]
    public int ClientId { get; set; }
}