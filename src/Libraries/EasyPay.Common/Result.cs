using EasyPay.Common.Errors;
using System.Text.Json.Serialization;

namespace EasyPay.Common
{
    public record Result<T> : Result
    {
        public T? Value { get; init; }
        public Result() { }
        private Result(bool isSuccess, T? value, Error? error) : base(isSuccess, error)
        {
            Value = value;
        }

        public static Result<T> Success(T value) => new(true, value, null);
        public new static Result<T> Failure(Error error) => new(false, default, error);

        public TResult Match<TResult>(
           Func<T, TResult> onSuccess,
           Func<Error, TResult> onFailure)
        {
            return IsSuccess ? onSuccess(Value!) : onFailure(error!);
        }
    }

    public record Result
    {
        public bool IsSuccess { get; init; }
        public Error? error { get; init; }
        public Result() { }
        [JsonConstructor]
        public Result(bool isSuccess, Error? error)
        {
            IsSuccess = isSuccess;
            this.error = error;
        }

        public static Result Success() => new(true, null);
        public static Result Failure(Error error) => new(false, error);

        public TResult Match<TResult>(
           Func<TResult> onSuccess,
           Func<Error, TResult> onFailure)
        {
            return IsSuccess ? onSuccess() : onFailure(error!);
        }
    }
}