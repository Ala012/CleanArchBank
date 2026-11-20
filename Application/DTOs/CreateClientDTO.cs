using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public sealed class CreateClientDTO
{
    [Required(ErrorMessage = "Le nom est obligatoire.")]
    [StringLength(80, MinimumLength = 2)]
    public string Nom { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le prénom est obligatoire.")]
    [StringLength(80, MinimumLength = 2)]
    public string Prenom { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'adresse e-mail est obligatoire.")]
    [EmailAddress(ErrorMessage = "L'adresse e-mail n'est pas valide.")]
    [StringLength(180)]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Le numéro de téléphone n'est pas valide.")]
    [StringLength(30)]
    public string? Tel { get; set; }

    [StringLength(200)]
    public string? Adresse { get; set; }

    [StringLength(300)]
    public string? Image { get; set; }
}
