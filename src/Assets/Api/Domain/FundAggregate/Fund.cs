using PAS.Assets.Domain.CurrencyAggregate;
using PAS.Assets.Domain.FundAggregate.Events;
using PAS.Domain;

namespace PAS.Assets.Domain.FundAggregate;

public class Fund : Entity<FundId>, IAggregateRoot {
    public FundType Type { get; private set; }
    public FundStatus Status { get; private set; }
    public string Name { get; private set; } = null!;
    public Isin Isin { get; private set; } = null!;
    public CurrencyId CurrencyId { get; private set; }

    private readonly List<FundNav> navs = [];
    public IReadOnlyCollection<FundNav> Navs => navs.AsReadOnly();

    private Fund() {
        // For EF hydration
    }

    private Fund(FundId id, FundType type, FundStatus status, string name, Isin isin, CurrencyId currencyId, IEnumerable<FundNav>? navs = null) {
        Id = id;
        Type = type;
        Status = status;
        Name = name;
        Isin = isin;
        CurrencyId = currencyId;
        if (navs != null) this.navs = [.. navs];
    }

    public static ErrorOr<Fund> CreateCollectiveFund(Guid? id, FundStatus status, string name, string isin, string currencyId, IEnumerable<FundNav>? navs = null) {
        var eoFundId = FundId.FromOrNew(id);
        if (eoFundId.IsFailure)
            return eoFundId.Errors;

        var eoCurrencyId = CurrencyId.From(currencyId);
        if (eoCurrencyId.IsFailure)
            return eoCurrencyId.Errors;

        var eoIsin = Isin.Create(isin);
        if (eoIsin.IsFailure)
            return eoIsin.Errors;

        if (string.IsNullOrWhiteSpace(name))
            return ErrorInfo.Unprocessable("Invalid fund name.");

        return new Fund(eoFundId.Value, FundType.Collective, status, name, eoIsin.Value, eoCurrencyId.Value, navs);
    }

    public static ErrorOr<Fund> CreateDedicatedFund(Guid? id, FundStatus status, string name, string isin, string currencyId, IEnumerable<FundNav>? navs = null) {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Add or update a fund NAV for the given date.
    /// </summary>
    /// <remarks>
    /// Precondition: The Fund aggregate should be loaded with navs filtered 
    /// to include at least the nav at the given date (if it exists).
    /// </remarks>
    public ErrorOr<UpsertResult> UpsertNav(DateTime navDate, double navValue) {
        var eoNav = FundNav.Create(navDate, navValue);
        if (eoNav.IsFailure) return eoNav.Errors;
        var nav = eoNav.Value;

        var existingNav = navs.FirstOrDefault(v => v.Date == nav.Date);
        if (existingNav != null) {
            if (existingNav.Value == nav.Value) return UpsertResult.Unchanged;
            navs.Remove(existingNav);
        }

        navs.Add(nav);
        AddDomainEvent(new FundNavChangedDomainEvent(Id.Value, nav.Date, existingNav?.Value, nav.Value));
        return existingNav == null ? UpsertResult.Created : UpsertResult.Updated;
    }

    public ErrorOr<Success> Close() {
        if (Status == FundStatus.Closed)
            return ErrorInfo.Unprocessable($"Cannot change fund status from '{Status}' to '{FundStatus.Closed}'.");

        AddDomainEvent(new FundClosedDomainEvent(Id.Value));
        return Success.Value;
    }
}
