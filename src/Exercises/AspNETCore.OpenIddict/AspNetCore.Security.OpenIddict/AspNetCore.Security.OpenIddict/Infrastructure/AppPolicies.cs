namespace AspNetCore.Security.OpenIddict.Infrastructure
{
    public static class AppPolicies
    {
        public const string CanCreateCustomer = "CanCreateCustomer";
        public const string CanUpdateCustomer = "CanUpdateCustomer";
        public const string CanDeleteCustomer = "CanDeleteCustomer";
        public const string CanReadCustomerAge = "CanReadCustomerAge";
        public const string CanAccessCustomerSaleHistory = nameof(CanAccessCustomerSaleHistory);
    }
}
