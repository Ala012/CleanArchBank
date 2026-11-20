using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class CompteConfiguration : IEntityTypeConfiguration<Compte>
{
    public void Configure(EntityTypeBuilder<Compte> builder)
    {
        builder.ToTable("Comptes");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.NumeroCompte).IsRequired();
        builder.Property(c => c.TypeCompte).HasMaxLength(30).IsRequired();
        builder.Property(c => c.ClientId).IsRequired();
        builder.Property(c => c.Solde).HasPrecision(18, 2).IsRequired();
        builder.Property(c => c.DateCreation)
            .HasConversion<UtcDateTimeConverter>()
            .IsRequired();

        builder.HasIndex(c => c.NumeroCompte).IsUnique();
        builder.HasIndex(c => c.ClientId);

        builder.HasMany(c => c.Operations)
            .WithOne(o => o.Compte)
            .HasForeignKey(o => o.CompteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
