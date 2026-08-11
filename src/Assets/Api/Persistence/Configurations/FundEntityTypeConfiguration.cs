using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PAS.Assets.Domain.CurrencyAggregate;
using PAS.Assets.Domain.FundAggregate;

namespace PAS.Assets.Persistence.Configurations;

internal class FundEntityTypeConfiguration : IEntityTypeConfiguration<Fund> {

    public void Configure(EntityTypeBuilder<Fund> builder) {
        builder.ToTable("Funds");
        builder.Ignore(e => e.DomainEvents);

        builder.HasKey(e => e.Id)
            .IsClustered(false); // To avoid fragmentation, since the Id is a Guid and not sequential

        builder.Property(e => e.Id)
            .HasConversion(
                id => id.Value,
                value => FundId.Hydrate(value)
            );

        builder.Property(e => e.Type)
            .HasConversion<string>()
            .HasMaxLength(128);

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(128);

        builder.Property(e => e.Name)
            .HasMaxLength(128);

        builder.ComplexProperty(e => e.Isin, isinBuilder => {
            isinBuilder
                .Property(e => e.Value)
                .HasColumnName("Isin")
                .HasMaxLength(12);
        });

        builder.Property(e => e.CurrencyId)
            .HasMaxLength(3)
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => CurrencyId.Hydrate(value)
            );

        builder.OwnsMany(e => e.Navs, navsBuilder => {
            navsBuilder.ToTable("FundNavs");
            navsBuilder
                .Property<long>("Id")
                .UseHiLo("FundNavSeq");
            navsBuilder.HasKey("Id");

            navsBuilder.HasIndex(e => e.Date).IsUnique();
        });

        builder.HasOne<Currency>()
            .WithMany()
            .HasForeignKey(f => f.CurrencyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index creation on ComplexProperty is not yet supported in EF Core: https://github.com/dotnet/efcore/issues/31246
        // Fixed in Net11: https://github.com/dotnet/efcore/pull/38192
        //builder.HasIndex(e => e.Isin).IsUnique();

        builder.HasIndex(e => e.Name).IsUnique();
        builder.HasIndex(e => e.Status);
    }
}
