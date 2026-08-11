namespace PAS.Core.Results;

public readonly record struct ErrorInfo(
    ErrorType Type, 
    string Message, 
    string? Code = null
) {
    public static ErrorInfo Unknown(string message, string? code = null) => new(ErrorType.Unknown, message, code);
    public static ErrorInfo Unprocessable(string message, string? code = null) => new(ErrorType.Unprocessable, message, code);
    public static ErrorInfo NotFound(string message, string? code = null) => new(ErrorType.NotFound, message, code);
    public static ErrorInfo Conflict(string message, string? code = null) => new(ErrorType.Conflict, message, code);
}
