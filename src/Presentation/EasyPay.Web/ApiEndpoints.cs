namespace EasyPay.Web;

public static class ApiEndpoints
{
    private const string Base = "api/v1";

    public static class Identity
    {
        public const string VerifyPassword = $"{Base}/identity/VerifyPassword";
        public const string RegisterRequestCode = $"{Base}/identity/register/request-phone-verification";
        public const string RegisterVerifyPhone = $"{Base}/identity/register/verify-phone";
        public const string RegisterCompleteProfile = $"{Base}/identity/register/complete-profile";
    }

    public static class AccountTypes
    {
        public const string GetAll = $"{Base}/account-types";
    }

    public static class Accounts
    {
        public const string GetAll = $"{Base}/accounts";
        public static string GetById(Guid accountId) => $"{GetAll}/{accountId}";
        public const string Create = $"{Base}/accounts";
        public static string Update(Guid accountId) => $"{GetAll}/{accountId}";
        public static string Delete(Guid accountId) => $"{GetAll}/{accountId}";
    }

    public static class Transaction
    {
        public const string Transfer = $"{Base}/transaction/transfer";
    }

    public static class Bills
    {
        public const string PayMobileBill = $"{Base}/mobile-bill";
    }

    public static class Profile
    {
        public const string MyAccounts = $"{Base}/profile/accounts";
        public static string MyTransactionHistory(Guid accountId) => $"{MyAccounts}/{accountId}/transactions";
    }
}