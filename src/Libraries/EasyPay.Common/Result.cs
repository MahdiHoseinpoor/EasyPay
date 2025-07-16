using EasyPay.Common.Errors;

namespace EasyPay.Common;

public record Result<T>(bool IsSuccess, T? Value, Error? error) : Result(IsSuccess, error)
{

}
public record Result(bool IsSuccess, Error? error)
{
    public static Result Success() => new(true, null);
    public static Result Failure(Error error) => new(false, error);
    public static Result<T> Success<T>(T value) => new(true, value, null);
}