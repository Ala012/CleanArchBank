using Application.Common;
using Application.DTOs;
using Application.Services;
using CleanArchBank.UnitTests.Fakes;
using Domain.Entities;

namespace CleanArchBank.UnitTests.Application;

public class ClientServiceTests
{
    private readonly FakeClientRepository _clients = new();

    [Fact]
    public async Task CreateAsync_EmailDejaUtilise_RetourneConflit()
    {
        _clients.Clients.Add(new Client
        {
            Id = 1,
            Nom = "Martin",
            Prenom = "Sophie",
            Email = "sophie@example.fr"
        });
        var service = new ClientService(_clients);

        var resultat = await service.CreateAsync(new CreateClientDTO
        {
            Nom = "Autre",
            Prenom = "Personne",
            Email = "sophie@example.fr"
        });

        Assert.False(resultat.IsSuccess);
        Assert.Equal(ErrorType.Conflict, resultat.ErrorType);
        Assert.Single(_clients.Clients);
    }

    [Fact]
    public async Task CreateAsync_EmailNouveau_CreeLeClient()
    {
        var service = new ClientService(_clients);

        var resultat = await service.CreateAsync(new CreateClientDTO
        {
            Nom = "Dubois",
            Prenom = "Camille",
            Email = "camille@example.fr",
            Tel = "0611223344",
            Adresse = "5 rue des Fleurs, 75012 Paris"
        });

        Assert.True(resultat.IsSuccess);
        Assert.Equal("Camille Dubois", resultat.Value!.NomComplet);
        Assert.Equal("0611223344", resultat.Value.Tel);
        Assert.Single(_clients.Clients);
    }
}
