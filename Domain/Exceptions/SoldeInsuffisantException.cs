namespace Domain.Exceptions;

/// <summary>Levée lorsqu'un retrait dépasse le solde disponible.</summary>
public class SoldeInsuffisantException : DomainException
{
    public decimal Solde { get; }

    public decimal Montant { get; }

    public SoldeInsuffisantException(decimal solde, decimal montant)
        : base($"Solde insuffisant : solde disponible {solde:N2} €, montant demandé {montant:N2} €.")
    {
        Solde = solde;
        Montant = montant;
    }
}
