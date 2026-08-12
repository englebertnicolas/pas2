namespace PAS.Assets.Tests.Domain;

public abstract class DomainTestBase {

    protected static void Log(string message) {
        if (TestContext.Current.TestOutputHelper != null) {
            TestContext.Current.TestOutputHelper?.WriteLine(message);
        } else {
            TestContext.Current.SendDiagnosticMessage(message);
        }
    }
}
