using System.ComponentModel.DataAnnotations;
using Domain.Exceptions;

namespace Domain.Entities;

/// <summary>
/// Compte bancaire appartenant à un client. Le solde est modifié
/// exclusivement au travers des méthodes métier de l'entité.
/// </summary>
public class Compte
{
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Le numéro de compte est obligatoire.")]
    public int NumeroCompte { get; set; }

    [Required(ErrorMessage = "Le type de compte est obligatoire.")]
    [StringLength(30)]
    public string TypeCompte { get; set; } = "Courant";

    [Required(ErrorMessage = "Le client est obligatoire.")]
    public int ClientId { get; set; }

    public Client Client { get; set; } = null!;

    [Range(0, 999_999_999_999.99, ErrorMessage = "Le solde ne peut pas être négatif.")]
    public decimal Solde { get; private set; }

    public DateTime DateCreation { get; set; } = DateTime.UtcNow;

    public ICollection<Operation> Operations { get; set; } = new List<Operation>();

    public Compte()
    {
    }

    public Compte(int clientId, int numeroCompte, string typeCompte, decimal soldeInitial = 0m)
    {
        if (clientId <= 0)
        {
            throw new DomainException("L'identifiant du client est invalide.");
        }

        if (numeroCompte <= 0)
        {
            throw new DomainException("Le numéro de compte est invalide.");
        }

        if (string.IsNullOrWhiteSpace(typeCompte))
        {
            throw new DomainException("Le type de compte est obligatoire.");
        }

        ClientId = clientId;
        NumeroCompte = numeroCompte;
        TypeCompte = typeCompte.Trim();
        AjusterSolde(soldeInitial);
    }

    /// <summary>Crédite le compte d'un montant strictement positif.</summary>
    public void Crediter(decimal montant)
    {
        if (montant <= 0)
        {
            throw new DomainException("Le montant de l'opération doit être strictement positif.");
        }

        Solde += montant;
    }

    /// <summary>Débite le compte si le solde est suffisant.</summary>
    public void Debiter(decimal montant)
    {
        if (montant <= 0)
        {
            throw new DomainException("Le montant de l'opération doit être strictement positif.");
        }

        if (Solde < montant)
        {
            throw new SoldeInsuffisantException(Solde, montant);
        }

        Solde -= montant;
    }

    /// <summary>Fixe le solde (ajustement administratif ou relecture EF Core).</summary>
    public void AjusterSolde(decimal nouveauSolde)
    {
        if (nouveauSolde < 0)
        {
            throw new DomainException("Le solde d'un compte ne peut pas être négatif.");
        }

        Solde = nouveauSolde;
    }
}
