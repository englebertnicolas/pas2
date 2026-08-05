namespace PAS.Core;

public static class StringExtensions {

    public static string ToSentenceCase(this string? input) {
        if (string.IsNullOrEmpty(input))
            return input ?? string.Empty;

        return char.ToUpper(input[0]) + input[1..].ToLower();
    }
}
