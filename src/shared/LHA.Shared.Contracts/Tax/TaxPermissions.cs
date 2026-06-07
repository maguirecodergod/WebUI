namespace LHA.Shared.Contracts.Tax;

public static class TaxPermissions
{
    public const string GroupName = "Tax";

    public static class Jurisdictions
    {
        public const string Default = GroupName + ".Jurisdictions";
        public const string Read = Default + ".Read";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";

        public static class L
        {
            public const string Default = "Permissions.Tax.Jurisdictions.Default";
            public const string Read = "Permissions.Tax.Jurisdictions.Read";
            public const string Create = "Permissions.Tax.Jurisdictions.Create";
            public const string Update = "Permissions.Tax.Jurisdictions.Update";
            public const string Delete = "Permissions.Tax.Jurisdictions.Delete";
        }
    }

    public static class TaxRates
    {
        public const string Default = GroupName + ".TaxRates";
        public const string Read = Default + ".Read";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";

        public static class L
        {
            public const string Default = "Permissions.Tax.TaxRates.Default";
            public const string Read = "Permissions.Tax.TaxRates.Read";
            public const string Create = "Permissions.Tax.TaxRates.Create";
            public const string Update = "Permissions.Tax.TaxRates.Update";
            public const string Delete = "Permissions.Tax.TaxRates.Delete";
        }
    }

    public static class ProductCategories
    {
        public const string Default = GroupName + ".ProductCategories";
        public const string Read = Default + ".Read";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";

        public static class L
        {
            public const string Default = "Permissions.Tax.ProductCategories.Default";
            public const string Read = "Permissions.Tax.ProductCategories.Read";
            public const string Create = "Permissions.Tax.ProductCategories.Create";
            public const string Update = "Permissions.Tax.ProductCategories.Update";
            public const string Delete = "Permissions.Tax.ProductCategories.Delete";
        }
    }

    public static class CustomerProfiles
    {
        public const string Default = GroupName + ".CustomerProfiles";
        public const string Read = Default + ".Read";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";

        public static class L
        {
            public const string Default = "Permissions.Tax.CustomerProfiles.Default";
            public const string Read = "Permissions.Tax.CustomerProfiles.Read";
            public const string Create = "Permissions.Tax.CustomerProfiles.Create";
            public const string Update = "Permissions.Tax.CustomerProfiles.Update";
            public const string Delete = "Permissions.Tax.CustomerProfiles.Delete";
        }
    }

    public static class BusinessRegistrations
    {
        public const string Default = GroupName + ".BusinessRegistrations";
        public const string Read = Default + ".Read";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";

        public static class L
        {
            public const string Default = "Permissions.Tax.BusinessRegistrations.Default";
            public const string Read = "Permissions.Tax.BusinessRegistrations.Read";
            public const string Create = "Permissions.Tax.BusinessRegistrations.Create";
            public const string Update = "Permissions.Tax.BusinessRegistrations.Update";
            public const string Delete = "Permissions.Tax.BusinessRegistrations.Delete";
        }
    }

    public static class TaxDetermination
    {
        public const string Default = GroupName + ".TaxDetermination";
        public const string Calculate = Default + ".Calculate";
        public const string Read = Default + ".Read";
        public const string History = Default + ".History";

        public static class L
        {
            public const string Default = "Permissions.Tax.TaxDetermination.Default";
            public const string Calculate = "Permissions.Tax.TaxDetermination.Calculate";
            public const string Read = "Permissions.Tax.TaxDetermination.Read";
            public const string History = "Permissions.Tax.TaxDetermination.History";
        }
    }

    public static class TaxReturns
    {
        public const string Default = GroupName + ".TaxReturns";
        public const string Read = Default + ".Read";
        public const string Generate = Default + ".Generate";
        public const string File = Default + ".File";

        public static class L
        {
            public const string Default = "Permissions.Tax.TaxReturns.Default";
            public const string Read = "Permissions.Tax.TaxReturns.Read";
            public const string Generate = "Permissions.Tax.TaxReturns.Generate";
            public const string File = "Permissions.Tax.TaxReturns.File";
        }
    }

    public static class Configuration
    {
        public const string Default = GroupName + ".Configuration";
        public const string Read = Default + ".Read";
        public const string Manage = Default + ".Manage";

        public static class L
        {
            public const string Default = "Permissions.Tax.Configuration.Default";
            public const string Read = "Permissions.Tax.Configuration.Read";
            public const string Manage = "Permissions.Tax.Configuration.Manage";
        }
    }

    public static class AuditLogs
    {
        public const string Default = GroupName + ".AuditLogs";
        public const string Read = Default + ".Read";

        public static class L
        {
            public const string Default = "Permissions.Tax.AuditLogs.Default";
            public const string Read = "Permissions.Tax.AuditLogs.Read";
        }
    }

    public static class L
    {
        public const string Group = "Permissions.Tax.Group";
    }
}
