using Microsoft.AspNetCore.Builder;

namespace Yeast.Multitenancy
{
    public delegate void TenantApplicationConfiguration<TTenant>(IApplicationBuilder tenantApp, TenantContext<TTenant> tenantContext) where TTenant : ITenant;
}
