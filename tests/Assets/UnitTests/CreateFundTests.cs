using FluentAssertions;
using PAS.Assets.Domain.FundAggregate;

namespace PAS.Assets.UnitTests;

public class CreateFundTests {

    [Fact]
    public void Create_WithCorrectData() {
        // Arrange
        var name = "Global Equity Fund";
        var isin = "BE1234567890";
        var currency = "EUR";

        // Act
        var eoFund = Fund.CreateCollectiveFund(null, FundStatus.Active, name, isin, currency);

        // Assert
        eoFund.IsSuccess.Should().BeTrue();
        var fund = eoFund.Value;
        fund.Name.Should().Be(name);
        fund.Isin.Value.Should().Be(isin);
        fund.CurrencyId.Value.Should().Be(currency);
    }

    [Fact]
    public void InvalidIsin_ShouldFail() {
        // Arrange
        var invalidIsin = "BE123456789";

        // Act
        var eoIsin = Isin.Create(invalidIsin);

        // Assert
        eoIsin.IsSuccess.Should().BeFalse();
        eoIsin.Errors.Should().ContainSingle()
           .Which.Message.Should().Contain("must be exactly 12 characters long");
    }
}
