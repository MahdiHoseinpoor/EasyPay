namespace EasyPay.Application.Common
{
    public static class Permissions
    {
        public static class Accounts
        {
            public const string View = "Permissions.Accounts.View";
            public const string Create = "Permissions.Accounts.Create";
            public const string Edit = "Permissions.Accounts.Edit";
            public const string Delete = "Permissions.Accounts.Delete";
            public const string HardDelete = "Permissions.Accounts.HardDelete";
        }

        public static class AccountTypes
        {
            public const string View = "Permissions.AccountTypes.View";
            public const string Create = "Permissions.AccountTypes.Create";
            public const string Edit = "Permissions.AccountTypes.Edit";
            public const string Delete = "Permissions.AccountTypes.Delete";
        }
        public static class Roles
        {
            public const string View = "Permissions.Roles.View";
            public const string Create = "Permissions.Roles.Create";
            public const string Edit = "Permissions.Roles.Edit";
            public const string ManagePermissions = "Permissions.Roles.ManagePermissions";
        }
    }
}