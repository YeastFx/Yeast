using Microsoft.Extensions.DependencyInjection;
using System;

namespace Yeast.WebApi.Security
{
    /// <summary>
    /// Contains extension methods to <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds <see cref="JwtIssuer"/> to the service collection.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> reference.</param>
        /// <param name="configureOptions">The <see cref="JwtIssuerOptions"/> configuration action.</param>
        /// <returns>The <see cref="IServiceCollection"/> reference.</returns>
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, Action<JwtIssuerOptions> configureOptions)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            services.Configure(configureOptions);

            services.AddTransient<JwtIssuer>();

            return services;
        }
    }
}
