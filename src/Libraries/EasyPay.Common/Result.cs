using EasyPay.Common.Errors;

namespace EasyPay.Common;

public record Result<T>(bool IsSuccess, T? Value, Error? error) : Result(IsSuccess, error)
{
    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(Error error) => new(false,default, error);
    public TResult Match<TResult>(
       Func<T, TResult> onSuccess,
       Func<Error, TResult> onFailure)
    {
        return IsSuccess ? onSuccess(Value) : onFailure(error);
    }
}
public record Result(bool IsSuccess, Error? error)
{
    public static Result Success() => new(true, null);
    public static Result Failure(Error error) => new(false, error);
    public TResult Match<TResult>(
       Func<TResult> onSuccess,
       Func<Error, TResult> onFailure)
    {
        return IsSuccess ? onSuccess() : onFailure(error);
    }

}