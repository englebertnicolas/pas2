using System.Text.Json.Serialization.Metadata;
using Microsoft.AspNetCore.OpenApi;

namespace PAS.AspNetCore.OpenApi;

internal static class SchemaReferenceIdHelper {

    /// <summary>
    /// Creates an Open API schema reference ID.
    /// Takes care of nested types, generic types and handler class names.
    /// Returns <see langword="null"/> if the schema should be inlined.
    /// </summary>
    public static string? CreateSchemaReferenceId(JsonTypeInfo jsonTypeInfo) {
        var defaultId = OpenApiOptions.CreateDefaultSchemaReferenceId(jsonTypeInfo);
        if (defaultId is null) return null;
        return CreateSchemaReferenceId(jsonTypeInfo.Type);
    }

    private static string CreateSchemaReferenceId(Type type) {
        // Generic types
        if (type.IsGenericType) {
            var baseName = type.GetGenericTypeDefinition().Name;
            var backtickIndex = baseName.IndexOf('`');
            if (backtickIndex > 0) baseName = baseName[..backtickIndex];

            var arguments = type.GetGenericArguments().Select(CreateSchemaReferenceId);
            return $"{baseName}Of{string.Join("And", arguments)}";
        }

        // Nested types
        if (type.IsNested && type.DeclaringType != null) {
            return $"{CreateSchemaReferenceId(type.DeclaringType)}{GetTypeName(type)}";
        }

        return GetTypeName(type);
    }

    private static string GetTypeName(Type type) {
        var typeName = type.Name;
        if (typeName.Length > "Handler".Length && typeName.EndsWith("Handler"))
            return typeName[..^"Handler".Length];

        return typeName;
    }
}
