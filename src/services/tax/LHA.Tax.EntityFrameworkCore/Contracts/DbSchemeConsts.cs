namespace LHA.Tax.EntityFrameworkCore
{
    internal static class DbSchemeConsts
    {
        public const string ServiceName = "Tax";

        private const string Sep = "_";

        // ─── Group prefixes ──────────────────────────────────────────
        private const string PTax = "Tax" + Sep;
        private const string PAudit = "Audit" + Sep;
        private const string PEvent = "Event" + Sep;

        // ─── Tax Domain ──────────────────────────────────────────────
        public static class Tax
        {
            public const string Jurisdiction = PTax + "Jurisdiction";
            public const string Regime = PTax + "Regime";
            public const string Rate = PTax + "Rate";
            public const string ProductCategory = PTax + "ProductCategory";
            public const string RegistrationThreshold = PTax + "RegistrationThreshold";
            public const string CustomerTaxProfile = PTax + "CustomerTaxProfile";
            public const string CustomerTaxIdentifier = PTax + "CustomerTaxIdentifier";
            public const string CustomerTaxExemption = PTax + "CustomerTaxExemption";
            public const string BusinessTaxRegistration = PTax + "BusinessTaxRegistration";
            public const string DeterminationRequest = PTax + "DeterminationRequest";
            public const string DeterminationResult = PTax + "DeterminationResult";
            public const string InvoiceTaxLine = PTax + "InvoiceTaxLine";
            public const string PeriodSummary = PTax + "PeriodSummary";
        }

        // ─── Audit Log ───────────────────────────────────────────────
        public static class Audit
        {
            public const string Log = PAudit + "Log";
            public const string Action = PAudit + "Action";
            public const string EntityChange = PAudit + "EntityChange";
            public const string PropertyChange = PAudit + "PropertyChange";
        }

        // ─── Event Bus ───────────────────────────────────────────────
        public static class Event
        {
            public const string Outbox = PEvent + "Outbox";
            public const string Inbox = PEvent + "Inbox";
        }
    }
}