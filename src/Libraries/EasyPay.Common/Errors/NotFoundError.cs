
namespace EasyPay.Common.Errors
{
    public record NotFoundError(string message = "there is a NotfoundError") : Error(404,message)
    {
    }
}
