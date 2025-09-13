namespace EasyPay.Common.Errors.Business
{
    public record DuplicateDocumentRequirementError() : Error(400, "This document requirement already exists for this account type.")
    {
    }
}