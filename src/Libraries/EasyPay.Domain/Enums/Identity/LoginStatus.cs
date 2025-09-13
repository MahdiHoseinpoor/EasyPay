namespace EasyPay.Domain.Enums.Identity
{
    public enum VerifyPasswordStatus
    {
        Success,
        Failed,
        LockedOut,
        NotAllowed,
        RequiresTwoFactor
    }
}
