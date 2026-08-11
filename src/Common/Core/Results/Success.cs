namespace PAS.Core.Results;

/// <summary>
/// Represent a successful operation without a return value.
/// </summary>
public readonly record struct Success {
    public static readonly Success Value = new();
}
