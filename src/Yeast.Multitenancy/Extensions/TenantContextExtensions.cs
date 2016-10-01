using Microsoft.Extensions.DependencyInjection;
using Yeast.Core.Helpers;

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
            Ensure.Argument.NotNull(tenantContext, nameof(tenantContext));

            return tenantContext.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        }
    }
}
