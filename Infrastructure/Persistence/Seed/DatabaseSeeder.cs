using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seed;

/// <summary>Jeu de données de démonstration (uniquement si la base est vide).</summary>
public static class DatabaseSeeder
{
    public static async Task SeedAsync(BankDbContext db, CancellationToken ct = default)
    {
        if (await db.Clients.AnyAsync(ct))
        {
            return;
        }

        var maintenant = DateTime.UtcNow;

        var clients = new List<Client>
        {
            new()
            {
                Nom = "Benali",
                Prenom = "Amina",
                Email = "amina.benali@example.fr",
                Tel = "0612345678",
                Adresse = "12 rue des Lilas, 75011 Paris",
                DateInscription = maintenant.AddMonths(-10)
            },
            new()
            {
                Nom = "El Amrani",
                Prenom = "Youssef",
                Email = "youssef.elamrani@example.fr",
                Tel = "0698765432",
                Adresse = "8 avenue de la République, 69003 Lyon",
                DateInscription = maintenant.AddMonths(-6)
            },
            new()
            {
                Nom = "Martin",
                Prenom = "Sophie",
                Email = "sophie.martin@example.fr",
                Tel = "0711223344",
                Adresse = "3 place du Capitole, 31000 Toulouse",
                DateInscription = maintenant.AddMonths(-3)
            }
        };

        db.Clients.AddRange(clients);
        await db.SaveChangesAsync(ct);

        var compteAmina = new Compte(clients[0].Id, 1001, "Courant", 3_200m)
        {
            DateCreation = maintenant.AddMonths(-9)
        };
        var compteEpargne = new Compte(clients[0].Id, 1002, "Épargne", 1_500m)
        {
            DateCreation = maintenant.AddMonths(-4)
        };
        var compteYoussef = new Compte(clients[1].Id, 2001, "Courant", 800m)
        {
            DateCreation = maintenant.AddMonths(-5)
        };
        var compteSophie = new Compte(clients[2].Id, 3001, "Courant", 5_400m)
        {
            DateCreation = maintenant.AddMonths(-2)
        };

        db.Comptes.AddRange(compteAmina, compteEpargne, compteYoussef, compteSophie);
        await db.SaveChangesAsync(ct);

        var operations = new List<Operation>
        {
            new()
            {
                CompteId = compteAmina.Id,
                TypeOperation = OperationType.Versement,
                Montant = 500m,
                DateOperation = maintenant.AddDays(-40),
                Libelle = "Versement salaire"
            },
            new()
            {
                CompteId = compteAmina.Id,
                TypeOperation = OperationType.Retrait,
                Montant = 300m,
                DateOperation = maintenant.AddDays(-20),
                Libelle = "Retrait DAB"
            },
            new()
            {
                CompteId = compteEpargne.Id,
                TypeOperation = OperationType.Versement,
                Montant = 1_500m,
                DateOperation = maintenant.AddDays(-30),
                Libelle = "Ouverture épargne"
            },
            new()
            {
                CompteId = compteYoussef.Id,
                TypeOperation = OperationType.Retrait,
                Montant = 200m,
                DateOperation = maintenant.AddDays(-10),
                Libelle = "Achat en ligne"
            },
            new()
            {
                CompteId = compteSophie.Id,
                TypeOperation = OperationType.Versement,
                Montant = 400m,
                DateOperation = maintenant.AddDays(-5),
                Libelle = "Prime"
            }
        };

        db.Operations.AddRange(operations);
        await db.SaveChangesAsync(ct);
    }
}
