using PAS.Assets.Domain.CurrencyAggregate;
using PAS.Assets.Domain.FundAggregate.Events;
using PAS.Domain;

namespace PAS.Assets.Domain.FundAggregate;

public class Fund : Entity, IAggregateRoot {
    public FundType Type { get; private set; }
    public FundStatus Status { get; private set; }
    public string Name { get; private set; } = null!;
    public Isin Isin { get; private set; } = null!;
    public CurrencyId CurrencyId { get; private set; } = null!;

    private readonly List<FundNav> navs = [];
    public IReadOnlyCollection<FundNav> Navs => navs.AsReadOnly();

    private Fund() {
        // For EF hydration
    }

    internal Fund(FundType type, FundStatus status, string name, Isin isin, CurrencyId currencyId, IEnumerable<FundNav>? navs = null) {
        Type = type;
        Status = status;
        Name = name;
        Isin = isin;
        CurrencyId = currencyId;
        if (navs != null) this.navs = [.. navs];
    }

    public static Fund CreateCollectiveFund(FundStatus status, string name, Isin isin, CurrencyId currencyId, IEnumerable<FundNav>? navs = null) {
        ValidateFundCreation(name, isin);
        return new(FundType.Collective, status, name, isin, currencyId, navs);
    }

    public static Fund CreateDedicatedFund(FundStatus status, string name, Isin isin, CurrencyId currencyId, IEnumerable<FundNav>? navs = null) {
        ValidateFundCreation(name, isin);
        return new(FundType.Dedicated, status, name, isin, currencyId, navs);
    }

    private static void ValidateFundCreation(string name, Isin _) {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Invalid fund name.", nameof(Name));
    }

    public UpsertResult UpsertNav(DateTime date, double value) => UpsertNav(FundNav.Create(date, value));
    public UpsertResult UpsertNav(FundNav nav) {
        var existingNav = navs.FirstOrDefault(v => v.Date == nav.Date);
        if (existingNav != null) {
            if (existingNav.Value == nav.Value) return UpsertResult.Unchanged;
            navs.Remove(existingNav);
        }

        navs.Add(nav);

        AddDomainEvent(new FundNavChangedDomainEvent(Id, Isin.Value, CurrencyId.Value, nav.Date, existingNav?.Value, nav.Value));

        return existingNav == null ? UpsertResult.Created : UpsertResult.Updated;
    }

    public void SetClosedStatus() {
        if (Status == FundStatus.Closed)
            throw new DomainException($"Cannot change fund status from '{Status}' to '{FundStatus.Closed}'.", nameof(Status));

        AddDomainEvent(new FundStatusChangedToClosedDomainEvent(Id));
    }
}
