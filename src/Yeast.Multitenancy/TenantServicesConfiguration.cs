using Microsoft.Extensions.DependencyInjection;

namespace Yeast.Multitenancy
{
    public delegate void TenantServicesConfiguration<TTenant>(TTenant tenant, IServiceCollection services) where TTenant : ITenant;
}
