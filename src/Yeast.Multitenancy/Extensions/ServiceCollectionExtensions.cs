using Microsoft.Extensions.DependencyInjection;
using System;

namespace Yeast.Multitenancy
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds tenant resolution service with specified <paramref name="resolverFactory"/>
        /// </summary>
        /// <typeparam name="TTenant">The type of the tenant</typeparam>
        /// <param name="services">The <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection"/> to add the service to.</param>
        /// <param name="resolverFactory">The factory that creates the <see cref="Yeast.Multitenancy.ITenantResolver{TTenant}"/></param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddMultitenancy<TTenant>(this IServiceCollection services, Func<IServiceProvider, ITenantResolver<TTenant>> resolverFactory)
            where TTenant : ITenant
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (resolverFactory == null)
            {
                throw new ArgumentNullException(nameof(resolverFactory));
            }

            services.AddSingleton(resolverFactory);

            return services;
        }

        /// <summary>
        /// Adds tenant resolution service with the specified instance of <see cref="ITenantResolver{TTenant}"/>.
        /// </summary>
        /// <typeparam name="TTenant">The type of the tenant</typeparam>
        /// <param name="services">The <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection"/> to add the service to.</param>
        /// <param name="resolverInstance">The instance of <see cref="ITenantResolver{TTenant}"/></param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddMultitenancy<TTenant>(this IServiceCollection services, ITenantResolver<TTenant> resolverInstance)
            where TTenant : ITenant
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (resolverInstance == null)
            {
                throw new ArgumentNullException(nameof(resolverInstance));
            }


            services.AddSingleton(resolverInstance);

            return services;
        }

        /// <summary>
        /// Adds tenant resolution service of the type specified in <typeparamref name="TResolver"/>.
        /// </summary>
        /// <typeparam name="TTenant">The type of the tenant</typeparam>
        /// <typeparam name="TResolver">The type of the <see cref="ITenantResolver{TTenant}"/> implementation to use.</typeparam>
        /// <param name="services">The <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddMultitenancy<TTenant, TResolver>(this IServiceCollection services)
            where TResolver : class, ITenantResolver<TTenant>
            where TTenant : ITenant
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<ITenantResolver<TTenant>, TResolver>();

            return services;
        }

        /// <summary>
        /// Configures per tenant services
        /// </summary>
        /// <typeparam name="TTenant">The type of the tenant</typeparam>
        /// <param name="services">The tenant's <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection"/> to configure.</param>
        /// <param name="servicesFactory">The funtion that configures per tenant services.</param>
        /// <returns>The tenent's <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection"/> reference</returns>
        public static IServiceCollection ConfigureTenantServices<TTenant>(this IServiceCollection services, TenantServicesConfiguration<TTenant> servicesFactory)
            where TTenant : ITenant
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (servicesFactory == null)
            {
                throw new ArgumentNullException(nameof(servicesFactory));
            }

            services.AddSingleton(servicesFactory);

            return services;
        }
    }
}
