namespace PAS.Core.Results;

/// <summary>
/// Represent the result of an operation that can either succeed with a value of type T or fail with one or more errors.
/// </summary>
public readonly record struct ErrorOr<T> {
    private readonly T? value;
    private readonly ErrorInfo[]? errors;

    public bool IsSuccess { get; } = true;
    public bool IsFailure => !IsSuccess;
    public T Value => IsSuccess ? value! : throw new InvalidOperationException("Cannot access Value when the result is a failure.");
    public ReadOnlySpan<ErrorInfo> Errors => errors ?? [];
    public ErrorInfo FirstError => Errors.Length > 0 ? Errors[0] : throw new InvalidOperationException("Cannot access FirstError when the result is a success.");

    private ErrorOr(T value) {
        IsSuccess = true;
        errors = null;
        this.value = value;
    }

    private ErrorOr(ErrorInfo[] errors) {
        if (errors.Length == 0) throw new ArgumentException("Errors cannot be empty.", nameof(errors));
        IsSuccess = false;
        this.errors = errors;
        value = default;
    }

    public static ErrorOr<T> Success(T value) => new(value);
    public static ErrorOr<T> Failure(IEnumerable<ErrorInfo> errors) => new([.. errors]);
    public static ErrorOr<T> Failure(params ErrorInfo[] errors) => new(errors);
    public static ErrorOr<T> Failure(ReadOnlySpan<ErrorInfo> errors) => new(errors.ToArray());

    public static implicit operator ErrorOr<T>(T value) => Success(value);
    public static implicit operator ErrorOr<T>(ErrorInfo error) => Failure(error);
    public static implicit operator ErrorOr<T>(ErrorInfo[] errors) => Failure(errors);
    public static implicit operator ErrorOr<T>(ReadOnlySpan<ErrorInfo> errors) => Failure(errors.ToArray());
}
