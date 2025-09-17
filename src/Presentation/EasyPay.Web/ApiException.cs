using EasyPay.Common.Errors;

namespace EasyPay.Web
{
    public class ApiException : Exception
    {
        public Error ApiError { get; }

        public ApiException(Error apiError) : base(apiError.message)
        {
            ApiError = apiError;
        }
    }
}
