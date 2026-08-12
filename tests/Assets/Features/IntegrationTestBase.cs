namespace PAS.Assets.Tests.Features;

[Collection("AppCollection")]
public abstract class IntegrationTestBase(AppFixture fixture) : IAsyncLifetime {
    protected AppFixture Fixture => fixture;
    protected HttpClient Client { get; } = fixture.CreateClient();

    public async ValueTask InitializeAsync() {
        // Cleaning db before every test
        await fixture.ResetDatabaseAsync();
    }

    public ValueTask DisposeAsync() { 
        GC.SuppressFinalize(this); 
        return ValueTask.CompletedTask; 
    }

    protected static void Log(string message) {
        if (TestContext.Current.TestOutputHelper != null) {
            TestContext.Current.TestOutputHelper?.WriteLine(message);
        } else {
            TestContext.Current.SendDiagnosticMessage(message);
        }
    }
}
