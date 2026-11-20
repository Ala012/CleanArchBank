using Application.DTOs;
using Domain.Entities;

namespace Application.Mappings;

/// <summary>Conversion des entités du domaine vers les DTO exposés par l'API.</summary>
public static class MappingExtensions
{
    public static ClientDTO ToDto(this Client client) => new()
    {
        Id = client.Id,
        Nom = client.Nom,
        Prenom = client.Prenom,
        NomComplet = client.NomComplet,
        Email = client.Email,
        Tel = client.Tel,
        Adresse = client.Adresse,
        Image = client.Image,
        NombreComptes = client.Comptes.Count,
        SoldeTotal = client.Comptes.Sum(c => c.Solde)
    };

    public static CompteDTO ToDto(this Compte compte) => new()
    {
        Id = compte.Id,
        NumeroCompte = compte.NumeroCompte,
        TypeCompte = compte.TypeCompte,
        ClientId = compte.ClientId,
        ClientNomComplet = compte.Client?.NomComplet ?? string.Empty,
        ClientEmail = compte.Client?.Email ?? string.Empty,
        Solde = compte.Solde,
        DateCreation = compte.DateCreation
    };

    public static OperationDTO ToDto(this Operation operation) => new()
    {
        Id = operation.Id,
        CompteId = operation.CompteId,
        TypeOperation = operation.TypeOperation,
        Montant = operation.Montant,
        DateOperation = operation.DateOperation,
        Libelle = operation.Libelle
    };
}
