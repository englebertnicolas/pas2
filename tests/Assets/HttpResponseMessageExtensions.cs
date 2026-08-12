using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace PAS.Assets.Tests;

public static class HttpResponseMessageExtensions {

    private static readonly JsonSerializerOptions DefaultOptions = new() { PropertyNameCaseInsensitive = true };

    public static async Task<T?> ReadSuccessContentFromJsonOrLogAsync<T>(this HttpResponseMessage response) {
        if (response.IsSuccessStatusCode) {
            return await response.Content.ReadFromJsonAsync<T>(DefaultOptions, TestContext.Current.CancellationToken);
        }

        var rawBody = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        TestContext.Current.TestOutputHelper?.WriteLine(
            $"❌ API Error [{(int)response.StatusCode} {response.StatusCode}]\n" +
            $"Route: {response.RequestMessage?.RequestUri}\n" +
            $"--- RAW RESPONSE BODY ---\n{rawBody}\n-------------------------");

        return default;
    }

    public static async Task<HttpValidationProblemDetails?> ReadContentAsProblemDetailsAsync(this HttpResponseMessage response) {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>(options, TestContext.Current.CancellationToken);
    }
}
