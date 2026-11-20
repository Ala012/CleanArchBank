using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Persistence.Configurations;

/// <summary>Force les DateTime relus depuis MySQL en UTC (MySQL ne stocke pas le Kind).</summary>
public sealed class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
{
    public UtcDateTimeConverter()
        : base(
            v => v.Kind == DateTimeKind.Local ? v.ToUniversalTime() : v,
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
    {
    }
}

public sealed class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("Clients");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nom).HasMaxLength(80).IsRequired();
        builder.Property(c => c.Prenom).HasMaxLength(80).IsRequired();
        builder.Property(c => c.Email).HasMaxLength(180).IsRequired();
        builder.Property(c => c.Tel).HasMaxLength(30);
        builder.Property(c => c.Adresse).HasMaxLength(200);
        builder.Property(c => c.Image).HasMaxLength(300);
        builder.Property(c => c.DateInscription)
            .HasConversion<UtcDateTimeConverter>()
            .IsRequired();

        builder.HasIndex(c => c.Email).IsUnique();

        builder.HasMany(c => c.Comptes)
            .WithOne(c => c.Client)
            .HasForeignKey(c => c.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
