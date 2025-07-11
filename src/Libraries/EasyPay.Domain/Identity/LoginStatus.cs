namespace EasyPay.Domain.Identity
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
