namespace PAS.Core.Results;

public static class ErrorOrExtensions {

    /// <summary>
    /// Transform the value of a success. If it's a failure, propagate the error without executing the function.
    /// </summary>
    public static ErrorOr<TOut> Map<TIn, TOut>(this ErrorOr<TIn> result, Func<TIn, TOut> func) {
        if (result.IsFailure)
            return ErrorOr<TOut>.Failure([.. result.Errors]);

        var v = func(result.Value);
        return ErrorOr<TOut>.Success(v);
    }

    /// <summary>
    /// Transform the value of a success. If it's a failure, propagate the error without executing the function.
    /// </summary>
    public static async Task<ErrorOr<TOut>> MapAsync<TIn, TOut>(this Task<ErrorOr<TIn>> resultTask, Func<TIn, TOut> func) {
        var result = await resultTask;
        if (result.IsFailure)
            return ErrorOr<TOut>.Failure([.. result.Errors]);

        var v = func(result.Value);
        return ErrorOr<TOut>.Success(v);
    }

    /// <summary>
    /// Chain with another operation that returns an OperationResult itself (avoid nested types).
    /// </summary>
    public static ErrorOr<TOut> Bind<TIn, TOut>(this ErrorOr<TIn> result, Func<TIn, ErrorOr<TOut>> func) {
        if (result.IsFailure)
            return ErrorOr<TOut>.Failure([.. result.Errors]);

        return func(result.Value);
    }

    /// <summary>
    /// Chain with another operation that returns an OperationResult itself (avoid nested types).
    /// </summary>
    public static async Task<ErrorOr<TOut>> BindAsync<TIn, TOut>(this ErrorOr<TIn> result, Func<TIn, Task<ErrorOr<TOut>>> func) {
        if (result.IsFailure)
            return ErrorOr<TOut>.Failure([.. result.Errors]);

        return await func(result.Value);
    }

    /// <summary>
    /// Execute an action if the result is a success, then return the initial result unchanged.
    /// </summary>
    public static ErrorOr<T> Tap<T>(this ErrorOr<T> result, Action<T> action) {
        if (result.IsSuccess) {
            action(result.Value);
        }
        return result;
    }

    /// <summary>
    /// Execute an action if the result is a success, then return the initial result unchanged.
    /// </summary>
    public static async Task<ErrorOr<T>> TapAsync<T>(this ErrorOr<T> result, Func<T, Task> action) {
        if (result.IsSuccess) {
            await action(result.Value);
        }
        return result;
    }
}
