using Application.Common;
using Application.DTOs;
using Application.Services;
using CleanArchBank.UnitTests.Fakes;
using Domain.Entities;

namespace CleanArchBank.UnitTests.Application;

public class CompteServiceTests
{
    private readonly FakeCompteRepository _comptes = new();
    private readonly FakeClientRepository _clients = new();
    private readonly CompteService _service;

    public CompteServiceTests()
    {
        _service = new CompteService(_comptes, _clients);
    }

    private Client AjouterClient(int id = 1)
    {
        var client = new Client
        {
            Id = id,
            Nom = "Benali",
            Prenom = "Amina",
            Email = $"amina{id}@example.fr"
        };
        _clients.Clients.Add(client);
        return client;
    }

    private Compte AjouterCompte(Client client, decimal solde, int id = 1, int numero = 1001)
    {
        var compte = new Compte(client.Id, numero, "Courant", solde)
        {
            Id = id,
            Client = client
        };
        _comptes.Comptes.Add(compte);
        client.Comptes.Add(compte);
        return compte;
    }

    [Fact]
    public async Task CreateAsync_ClientInexistant_RetourneValidation()
    {
        var resultat = await _service.CreateAsync(new CreateCompteDTO
        {
            ClientId = 99,
            NumeroCompte = 1001,
            TypeCompte = "Courant",
            Solde = 500m
        });

        Assert.False(resultat.IsSuccess);
        Assert.Equal(ErrorType.Validation, resultat.ErrorType);
        Assert.Empty(_comptes.Comptes);
    }

    [Fact]
    public async Task CreateAsync_ClientExistant_CreeLeCompte()
    {
        var client = AjouterClient();

        var resultat = await _service.CreateAsync(new CreateCompteDTO
        {
            ClientId = client.Id,
            NumeroCompte = 1001,
            TypeCompte = "Courant",
            Solde = 500m
        });

        Assert.True(resultat.IsSuccess);
        Assert.NotNull(resultat.Value);
        Assert.Equal(500m, resultat.Value!.Solde);
        Assert.Equal(1001, resultat.Value.NumeroCompte);
        Assert.Single(_comptes.Comptes);
    }

    [Fact]
    public async Task CreateOperationAsync_RetraitSoldeInsuffisant_RetourneBadRequestSansEcriture()
    {
        var client = AjouterClient();
        var compte = AjouterCompte(client, 100m);

        var resultat = await _service.CreateOperationAsync(compte.Id, new CreateOperationDTO
        {
            TypeOperation = OperationType.Retrait,
            Montant = 150m,
            Libelle = "Test"
        });

        Assert.False(resultat.IsSuccess);
        Assert.Equal(ErrorType.Validation, resultat.ErrorType);
        Assert.Contains("Solde insuffisant", resultat.Error);
        Assert.False(_comptes.AddOperationAppele);
        Assert.Equal(100m, compte.Solde);
    }

    [Fact]
    public async Task CreateOperationAsync_RetraitValide_DebiteEtPersiste()
    {
        var client = AjouterClient();
        var compte = AjouterCompte(client, 100m);

        var resultat = await _service.CreateOperationAsync(compte.Id, new CreateOperationDTO
        {
            TypeOperation = OperationType.Retrait,
            Montant = 40m,
            Libelle = "Retrait test"
        });

        Assert.True(resultat.IsSuccess);
        Assert.Equal(60m, compte.Solde);
        Assert.Equal(60m, _comptes.DernierSoldeEnregistre);
        Assert.True(_comptes.AddOperationAppele);
        Assert.Single(_comptes.Operations);
        Assert.Equal(OperationType.Retrait, _comptes.Operations[0].TypeOperation);
    }

    [Fact]
    public async Task CreateOperationAsync_Versement_CrediteLeCompte()
    {
        var client = AjouterClient();
        var compte = AjouterCompte(client, 100m);

        var resultat = await _service.CreateOperationAsync(compte.Id, new CreateOperationDTO
        {
            TypeOperation = OperationType.Versement,
            Montant = 25m,
            Libelle = "Versement test"
        });

        Assert.True(resultat.IsSuccess);
        Assert.Equal(125m, compte.Solde);
        Assert.Equal(125m, _comptes.DernierSoldeEnregistre);
    }

    [Fact]
    public async Task DeleteAsync_CompteAvecOperations_RetourneConflit()
    {
        var client = AjouterClient();
        var compte = AjouterCompte(client, 100m);
        _comptes.Operations.Add(new Operation
        {
            CompteId = compte.Id,
            Montant = 10m,
            TypeOperation = OperationType.Retrait
        });

        var resultat = await _service.DeleteAsync(compte.Id);

        Assert.False(resultat.IsSuccess);
        Assert.Equal(ErrorType.Conflict, resultat.ErrorType);
        Assert.False(_comptes.DeleteAppele);
    }

    [Fact]
    public async Task TransferAsync_ComptesIdentiques_RetourneValidation()
    {
        var resultat = await _service.TransferAsync(new VirementDTO
        {
            CompteSourceId = 1,
            CompteDestinationId = 1,
            Montant = 100m
        });

        Assert.False(resultat.IsSuccess);
        Assert.Equal(ErrorType.Validation, resultat.ErrorType);
        Assert.False(_comptes.TransferAppele);
    }

    [Fact]
    public async Task TransferAsync_SoldeInsuffisant_NePersistePasLeVirement()
    {
        var client = AjouterClient();
        var source = AjouterCompte(client, 100m, 1, 1001);
        var destination = AjouterCompte(client, 50m, 2, 1002);

        var resultat = await _service.TransferAsync(new VirementDTO
        {
            CompteSourceId = source.Id,
            CompteDestinationId = destination.Id,
            Montant = 500m
        });

        Assert.False(resultat.IsSuccess);
        Assert.Equal(ErrorType.Validation, resultat.ErrorType);
        Assert.False(_comptes.TransferAppele);
        Assert.Equal(100m, source.Solde);
        Assert.Equal(50m, destination.Solde);
    }

    [Fact]
    public async Task TransferAsync_Valide_MetAJourLesDeuxSoldesEtLesEcritures()
    {
        var client = AjouterClient();
        var source = AjouterCompte(client, 300m, 1, 1001);
        var destination = AjouterCompte(client, 50m, 2, 1002);

        var resultat = await _service.TransferAsync(new VirementDTO
        {
            CompteSourceId = source.Id,
            CompteDestinationId = destination.Id,
            Montant = 200m
        });

        Assert.True(resultat.IsSuccess);
        Assert.Equal(100m, source.Solde);
        Assert.Equal(250m, destination.Solde);
        Assert.True(_comptes.TransferAppele);
        Assert.Equal(2, _comptes.Operations.Count);
        Assert.All(_comptes.Operations, o => Assert.Equal(OperationType.Virement, o.TypeOperation));
    }

    [Fact]
    public async Task GetTotalBalanceAsync_ClientInexistant_RetourneNotFound()
    {
        var resultat = await _service.GetTotalBalanceAsync(42);

        Assert.False(resultat.IsSuccess);
        Assert.Equal(ErrorType.NotFound, resultat.ErrorType);
    }

    [Fact]
    public async Task GetTotalBalanceAsync_ClientExistant_AdditionneLesSoldes()
    {
        var client = AjouterClient();
        AjouterCompte(client, 100m, 1, 1001);
        AjouterCompte(client, 250m, 2, 1002);

        var resultat = await _service.GetTotalBalanceAsync(client.Id);

        Assert.True(resultat.IsSuccess);
        Assert.Equal(350m, resultat.Value);
    }
}
