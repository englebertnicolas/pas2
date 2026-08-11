using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PAS.Assets.Domain.CurrencyAggregate;

namespace PAS.Assets.Persistence.Configurations;

internal class CurrencyEntityTypeConfiguration : IEntityTypeConfiguration<Currency> {

    public void Configure(EntityTypeBuilder<Currency> builder) {
        builder.ToTable("Currencies");
        builder.Ignore(e => e.DomainEvents);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasMaxLength(3)
            .HasConversion(
                id => id.Value,
                value => CurrencyId.Hydrate(value)
            );

        builder
            .Property(e => e.EnglishName)
            .HasMaxLength(128);

        builder.ComplexProperty(e => e.Symbol, symbolBuilder => {
            symbolBuilder
                .Property(e => e.Value)
                .HasColumnName("Symbol")
                .HasMaxLength(3);
        });
    }
}
