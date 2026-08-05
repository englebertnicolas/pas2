namespace PAS.AppHost;

internal static class AspireExtensions {

    public static IResourceBuilder<ProjectResource> WithScalarEndpoint(this IResourceBuilder<ProjectResource> builder) {
        return builder
            .WithUrlForEndpoint("http", url => url.Url = "/scalar")
            .WithUrlForEndpoint("https", url => url.Url = "/scalar");
    }
}
