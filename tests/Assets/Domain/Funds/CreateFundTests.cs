using FluentAssertions;
using PAS.Assets.Domain.FundAggregate;

namespace PAS.Assets.Tests.Domain.Funds;

public class CreateFundTests : DomainTestBase {

    [Fact]
    public void Should_Success_When_Arguments_Are_Valid() {
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

    [Theory]
    [InlineData("")]
    [InlineData("BE123")]
    [InlineData("FR-12345-ABC")]
    public void Should_Fail_When_Isin_Is_Invalid(string invalidIsin) {
        // Act
        var eoIsin = Isin.Create(invalidIsin);

        // Assert
        eoIsin.IsFailure.Should().BeTrue();

        Log($"Error message: {eoIsin.FirstError.Message}");
    }
}
