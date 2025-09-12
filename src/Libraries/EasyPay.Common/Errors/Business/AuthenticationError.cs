namespace EasyPay.Common.Errors.Business
{
    public record AuthenticationError(string message = "Invalid username or password") : Error(401, message)
    {
    }
}