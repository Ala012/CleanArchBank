using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// Représente un client de la banque. Un client peut posséder plusieurs comptes.
/// </summary>
public class Client
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le nom du client est obligatoire.")]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "Le nom doit contenir entre 2 et 80 caractères.")]
    public string Nom { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le prénom du client est obligatoire.")]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "Le prénom doit contenir entre 2 et 80 caractères.")]
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

    public DateTime DateInscription { get; set; } = DateTime.UtcNow;

    public ICollection<Compte> Comptes { get; set; } = new List<Compte>();

    [NotMapped]
    public string NomComplet => $"{Prenom} {Nom}".Trim();
}
