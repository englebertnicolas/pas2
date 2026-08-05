using Microsoft.AspNetCore.Http;

namespace PAS.AspNetCore;

public class HttpException : Exception {
    public int StatusCode { get; }

    private HttpException(int statusCode, string? message) : base(message) {
        StatusCode = statusCode;
    }

    private HttpException(int statusCode, string? message, Exception? innerException) : base(message, innerException) {
        StatusCode = statusCode;
    }

    public static HttpException CreateStatus404NotFound(string? message) => new(StatusCodes.Status404NotFound, message);
    public static HttpException CreateStatus404NotFound(string? message, Exception? innerException) => new(StatusCodes.Status404NotFound, message, innerException);

    public static HttpException CreateStatus409Conflict(string? message) => new(StatusCodes.Status409Conflict, message);
    public static HttpException CreateStatus409Conflict(string? message, Exception? innerException) => new(StatusCodes.Status409Conflict, message, innerException);

    public static HttpException CreateStatus422Unprocessable(string? message) => new(StatusCodes.Status422UnprocessableEntity, message);
    public static HttpException CreateStatus422Unprocessable(string? message, Exception? innerException) => new(StatusCodes.Status422UnprocessableEntity, message, innerException);
}
