using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public sealed class CreateCompteDTO
{
    [Range(1, int.MaxValue, ErrorMessage = "Numéro de compte invalide.")]
    public int NumeroCompte { get; set; }

    [Required(ErrorMessage = "Le type de compte est obligatoire.")]
    [StringLength(30)]
    public string TypeCompte { get; set; } = "Courant";

    [Range(typeof(decimal), "0", "999999999999.99", ErrorMessage = "Le solde initial ne peut pas être négatif.")]
    public decimal Solde { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "ClientId invalide.")]
    public int ClientId { get; set; }
}
