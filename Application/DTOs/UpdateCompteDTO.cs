using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public sealed class UpdateCompteDTO
{
    [Range(1, int.MaxValue, ErrorMessage = "Numéro de compte invalide.")]
    public int NumeroCompte { get; set; }

    [Required(ErrorMessage = "Le type de compte est obligatoire.")]
    [StringLength(30)]
    public string TypeCompte { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "ClientId invalide.")]
    public int ClientId { get; set; }
}
