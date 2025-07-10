namespace EasyPay.Common;

public record Result<T>(bool IsSuccess, T? Value, Error? error)
{
    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(Error error) => new(false, default, error);
}