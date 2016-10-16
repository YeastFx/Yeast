using Microsoft.Extensions.DependencyInjection;

namespace Yeast.Multitenancy
{
    public delegate void TenantServicesFactory<TTenant>(TTenant tenant, IServiceCollection services) where TTenant : ITenant;
}
