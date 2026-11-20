using Domain.Entities;
using Domain.Exceptions;

namespace CleanArchBank.UnitTests.Domain;

public class CompteTests
{
    [Fact]
    public void Crediter_AugmenteLeSolde()
    {
        var compte = new Compte(1, 1001, "Courant", 100m);

        compte.Crediter(50m);

        Assert.Equal(150m, compte.Solde);
    }

    [Fact]
    public void Debiter_DiminueLeSolde()
    {
        var compte = new Compte(1, 1001, "Courant", 100m);

        compte.Debiter(40m);

        Assert.Equal(60m, compte.Solde);
    }

    [Fact]
    public void Debiter_MontantEgalAuSolde_MetLeSoldeAZero()
    {
        var compte = new Compte(1, 1001, "Courant", 100m);

        compte.Debiter(100m);

        Assert.Equal(0m, compte.Solde);
    }

    [Fact]
    public void Debiter_SoldeInsuffisant_LeveSoldeInsuffisantException()
    {
        var compte = new Compte(1, 1001, "Courant", 100m);

        var exception = Assert.Throws<SoldeInsuffisantException>(() => compte.Debiter(150m));

        Assert.Equal(100m, exception.Solde);
        Assert.Equal(150m, exception.Montant);
        Assert.Equal(100m, compte.Solde);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Crediter_MontantNonPositif_LeveDomainException(decimal montant)
    {
        var compte = new Compte(1, 1001, "Courant", 100m);

        Assert.Throws<DomainException>(() => compte.Crediter(montant));
    }

    [Fact]
    public void AjusterSolde_Negatif_LeveDomainException()
    {
        var compte = new Compte(1, 1001, "Courant", 100m);

        Assert.Throws<DomainException>(() => compte.AjusterSolde(-1m));
    }

    [Fact]
    public void Constructeur_NumeroInvalide_LeveDomainException()
    {
        Assert.Throws<DomainException>(() => new Compte(1, 0, "Courant", 100m));
    }
}
