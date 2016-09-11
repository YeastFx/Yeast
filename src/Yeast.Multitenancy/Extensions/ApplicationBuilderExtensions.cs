using Microsoft.AspNetCore.Builder;
using Yeast.Core.Helpers;
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
            Ensure.Argument.NotNull(app, nameof(app));
            return app.UseMiddleware<TenantResolverMiddleware<TTenant>>();
        }
    }
}
