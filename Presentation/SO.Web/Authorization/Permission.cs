namespace SO.Web.Authorization
{
    public static class Permission
    {
        // Users
        public const string Users_Admin_Dashboard = "Users.Admin.Dashboard";
        public const string Users_Admin_View = "Users.Admin.View";
        public const string Users_Admin_Export = "Users.Admin.Export";
        public const string Users_SuperAdmin_Dashboard = "Users.SuperAdmin.Dashboard";
        public const string Users_SuperAdmin_ManageUsers = "Users.SuperAdmin.ManageUsers";
        public const string Users_SuperAdmin_Export = "Users.SuperAdmin.Export";
        public const string Users_Account_Profile_Edit = "Users.Account.Profile.Edit";

        // Proposals
        public const string Proposals_ReadOwn = "Proposals.ReadOwn";
        public const string Proposals_ReadAll = "Proposals.ReadAll";
        public const string Proposals_Create = "Proposals.Create";
        public const string Proposals_Edit = "Proposals.Edit";
        public const string Proposals_Delete = "Proposals.Delete";
        public const string Proposals_Export = "Proposals.Export";

        // Accounts (Domain company accounts)
        public const string Accounts_Manage = "Accounts.Manage";
        public const string Accounts_Create = "Accounts.Create";
        public const string Accounts_Edit = "Accounts.Edit";
        public const string Accounts_Delete = "Accounts.Delete";

        // Addresses
        public const string Addresses_Manage = "Addresses.Manage";
        public const string Addresses_Create = "Addresses.Create";
        public const string Addresses_Edit = "Addresses.Edit";
        public const string Addresses_Delete = "Addresses.Delete";
    }
}


