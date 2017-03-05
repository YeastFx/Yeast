using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace Yeast.Multitenancy
{
    public static class TenantContextExtensions
    {
        /// <summary>
        /// Extensions method to create service scope on <paramref name="tenantContext"/>.
        /// </summary>
        /// <param name="tenantContext">Created <see cref="IServiceScope"/></param>
        /// <returns></returns>
        public static IServiceScope CreateServiceScope(this TenantContext tenantContext)
        {
            Debug.Assert(tenantContext != null, $"{nameof(tenantContext)} must not be null");

            return tenantContext.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        }
    }
}
