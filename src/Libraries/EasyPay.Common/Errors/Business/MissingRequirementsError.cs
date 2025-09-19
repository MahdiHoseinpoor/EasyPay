namespace EasyPay.Common.Errors.Business
{
    public record MissingRequirementsError(List<string> missingRequirements) : Error(400, $"User is missing the following approved documents: {string.Join(", ", missingRequirements)}")
    {
    }
}