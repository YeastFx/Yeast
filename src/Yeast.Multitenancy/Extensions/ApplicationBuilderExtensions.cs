using Microsoft.AspNetCore.Builder;
using System;
using Yeast.Multitenancy.Internal;

namespace Yeast.Multitenancy
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds <see cref="TenantResolverMiddleware{TTenant}"/> to the application's request pipeline.
        /// </summary>
        /// <typeparam name="TTenant">Type of tenant</typeparam>
        /// <param name="app">The <see cref="IApplicationBuilder"/> instance</param>
        /// <returns></returns>
        public static IApplicationBuilder UseMultitenancy<TTenant>(this IApplicationBuilder app)
            where TTenant :ITenant
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<TenantResolverMiddleware<TTenant>>();
        }

        /// <summary>
        /// Configures tenant's pipeline
        /// </summary>
        /// <typeparam name="TTenant">Type of tenant</typeparam>
        /// <param name="rootApp">The <see cref="IApplicationBuilder"/> instance</param>
        /// <param name="tenantConfiguration">The per-tenant configuration</param>
        /// <returns>Reference to the root <see cref="IApplicationBuilder"/></returns>
        public static IApplicationBuilder ConfigureTenant<TTenant>(this IApplicationBuilder rootApp, TenantApplicationConfiguration<TTenant> tenantConfiguration)
            where TTenant : ITenant
        {
            if (rootApp == null)
            {
                throw new ArgumentNullException(nameof(rootApp));
            }

            if (tenantConfiguration == null)
            {
                throw new ArgumentNullException(nameof(tenantConfiguration));
            }
            
            return rootApp.UseMiddleware<TenantPipelineMiddleware<TTenant>>(rootApp, tenantConfiguration);
        }
    }
}
