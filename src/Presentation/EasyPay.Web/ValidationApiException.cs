using EasyPay.Common.Errors;

namespace EasyPay.Web
{
    public class ValidationApiException : ApiException
    {
        public Dictionary<string, string[]> Errors { get; }
        public IEnumerable<string> AllMessages => Errors.Values.SelectMany(v => v);

        public ValidationApiException(Dictionary<string, string[]> errors)
            : base(new ValidationError(errors))
        {
            Errors = errors;
        }
    }
}