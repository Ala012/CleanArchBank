using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class OperationConfiguration : IEntityTypeConfiguration<Operation>
{
    public void Configure(EntityTypeBuilder<Operation> builder)
    {
        builder.ToTable("Operations");
        builder.HasKey(o => o.Id);

        builder.Property(o => o.CompteId).IsRequired();
        builder.Property(o => o.TypeOperation)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(o => o.Montant).HasPrecision(18, 2).IsRequired();
        builder.Property(o => o.DateOperation)
            .HasConversion<UtcDateTimeConverter>()
            .IsRequired();
        builder.Property(o => o.Libelle).HasMaxLength(250);

        builder.HasIndex(o => new { o.CompteId, o.DateOperation });
    }
}
