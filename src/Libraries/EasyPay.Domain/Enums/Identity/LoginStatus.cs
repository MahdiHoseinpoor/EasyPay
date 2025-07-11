namespace EasyPay.Domain.Enums.Identity
{
    public enum LoginStatus
    {
        Success,
        Failed,
        LockedOut,
        NotAllowed,
        RequiresTwoFactor
    }
}
