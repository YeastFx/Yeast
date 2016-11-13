using Microsoft.AspNetCore.Http;
using Yeast.Core.Helpers;

namespace Yeast.Multitenancy
{
    /// <summary>
    /// Multitenant extensions for <see cref="HttpContext"/>.
    /// </summary>
    public static class HttpContextEntensions
    {
        private const string TenantContextKey = "Yeast.TenantContext";

        /// <summary>
        /// Stores current <see cref="TenantContext{TTenant}"/> in <see cref="HttpContext"/>
        /// </summary>
        /// <typeparam name="TTenant">Type of tenant</typeparam>
        /// <param name="httpContext">The current <see cref="HttpContext"/></param>
        /// <param name="tenantContext">The <see cref="TenantContext{TTenant}"/> to store</param>
        internal static void SetTenantContext<TTenant>(this HttpContext httpContext, TenantContext<TTenant> tenantContext)
            where TTenant : ITenant
        {
            Ensure.Argument.NotNull(httpContext, nameof(httpContext));
            Ensure.Argument.NotNull(tenantContext, nameof(tenantContext));

            httpContext.Items[TenantContextKey] = tenantContext;
        }

        /// <summary>
        /// Retrieves the current <see cref="TenantContext{TTenant}"/> from <see cref="HttpContext"/>
        /// </summary>
        /// <typeparam name="TTenant">Type of tenant</typeparam>
        /// <param name="httpContext">The current <see cref="HttpContext"/></param>
        /// <returns>The current <see cref="TenantContext{TTenant}"/></returns>
        public static TenantContext<TTenant> GetTenantContext<TTenant>(this HttpContext httpContext)
        {
            Ensure.Argument.NotNull(httpContext, nameof(httpContext));

            object tenantContext;
            if (httpContext.Items.TryGetValue(TenantContextKey, out tenantContext))
            {
                return tenantContext as TenantContext<TTenant>;
            }

            return null;
        }

        /// <summary>
        /// Retrieves the current tenant from <see cref="HttpContext"/>
        /// </summary>
        /// <typeparam name="TTenant">Type of tenant</typeparam>
        /// <param name="httpContext">The current <see cref="HttpContext"/></param>
        /// <returns>The current tenant</returns>
        public static TTenant GetTenant<TTenant>(this HttpContext httpContext)
        {
            Ensure.Argument.NotNull(httpContext, nameof(httpContext));

            var tenantContext = GetTenantContext<TTenant>(httpContext);

            if (tenantContext != null)
            {
                return tenantContext.Tenant;
            }

            return default(TTenant);
        }
    }
}
