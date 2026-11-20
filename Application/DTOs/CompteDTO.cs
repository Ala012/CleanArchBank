namespace Application.DTOs;

/// <summary>Compte bancaire exposé par l'API, avec son client et son solde.</summary>
public sealed class CompteDTO
{
    public int Id { get; set; }

    public int NumeroCompte { get; set; }

    public string TypeCompte { get; set; } = string.Empty;

    public int ClientId { get; set; }

    public string ClientNomComplet { get; set; } = string.Empty;

    public string ClientEmail { get; set; } = string.Empty;

    public decimal Solde { get; set; }

    public DateTime DateCreation { get; set; }
}
