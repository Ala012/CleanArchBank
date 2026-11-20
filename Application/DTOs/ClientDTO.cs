namespace Application.DTOs;

/// <summary>Client exposé par l'API, avec le nombre de comptes et le solde total.</summary>
public sealed class ClientDTO
{
    public int Id { get; set; }

    public string Nom { get; set; } = string.Empty;

    public string Prenom { get; set; } = string.Empty;

    public string NomComplet { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Tel { get; set; }

    public string? Adresse { get; set; }

    public string? Image { get; set; }

    public int NombreComptes { get; set; }

    public decimal SoldeTotal { get; set; }
}
