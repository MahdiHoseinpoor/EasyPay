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
        public static string GetRequirements(int accountTypeId) => $"{GetAll}/{accountTypeId}/document-requirements";
    }
    public static class BankCards
    {
        public const string GetAll = $"{Base}/bank-cards";
        public static string GetById(int cardId) => $"{GetAll}/{cardId}";
        public const string Create = $"{Base}/bank-cards";
        public static string Update(int cardId) => $"{GetAll}/{cardId}";
        public static string Delete(int cardId) => $"{GetAll}/{cardId}";
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
        public const string WithdrawToBankCard = $"{Base}/transaction/withdraw-to-card";
    }

    public static class Bills
    {
        public const string PayMobileBill = $"{Base}/mobile-bill";
    }
    public static class AuthItems
    {
        public const string GetAll = $"{Base}/auth-items";
    }
    public static class Profile
    {
        public const string MyAccounts = $"{Base}/profile/accounts";
        public const string MyBankCards = $"{Base}/profile/bank-cards";
        public static string MyTransactionHistory(Guid accountId) => $"{MyAccounts}/{accountId}/transactions";
        public const string MySubmittedDocuments = $"{Base}/profile/auth-item-values";
    }
}