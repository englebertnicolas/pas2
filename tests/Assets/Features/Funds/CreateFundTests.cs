using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PAS.Assets.Domain.CurrencyAggregate;
using PAS.Assets.Domain.FundAggregate;
using PAS.Assets.Features.Funds.Create;

namespace PAS.Assets.Tests.Features.Funds;

public class CreateProductTests(AppFixture f) : IntegrationTestBase(f) {

    [Fact]
    public async Task Should_Insert_Fund_In_Db_When_Arguments_Are_Valid() {
        // Arrange
        await Fixture.ExecuteDbContextAsync(db => {
            db.Currencies.Add(Currency.Create("EUR", "Euro", "€").Value);
            return db.SaveChangesAsync(TestContext.Current.CancellationToken);
        });
        var command = new CreateCollectiveFund.Command(null, "Equity Fund EUR", "BE0123456789", "EUR");

        // Act
        var response = await Client.PostAsJsonAsync("/funds/collective", command, TestContext.Current.CancellationToken);

        // Assert
        var result = await response.ReadSuccessContentFromJsonOrLogAsync<CreateCollectiveFund.Result>();
        result.Should().NotBeNull();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

        var fund = await Fixture.ExecuteDbContextAsync(db => 
            db.Funds.SingleOrDefaultAsync(x => x.Id == FundId.Hydrate(result.Id), TestContext.Current.CancellationToken));

        fund.Should().NotBeNull();
        fund.Name.Should().Be(command.Name);
        fund.Isin.Value.Should().Be(command.Isin);
        fund.CurrencyId.Value.Should().Be(command.Currency);
        fund.Status.Should().Be(FundStatus.Active);

        Log($"Generated fund ID: {fund.Id}");
    }
}
