using Microsoft.AspNetCore.Builder;

namespace Yeast.Multitenancy
{
    public delegate void TenantApplicationConfiguration<TTenant>(IApplicationBuilder tenantApp, TTenant tenant) where TTenant : ITenant;
}
