using Microsoft.Extensions.DependencyInjection;
using System;
using Yeast.Core.Helpers;

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
            Ensure.Argument.NotNull(services, nameof(services));
            Ensure.Argument.NotNull(resolverFactory, nameof(resolverFactory));

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
            Ensure.Argument.NotNull(services, nameof(services));
            Ensure.Argument.NotNull(resolverInstance, nameof(resolverInstance));

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
            Ensure.Argument.NotNull(services, nameof(services));

            services.AddSingleton<ITenantResolver<TTenant>, TResolver>();

            return services;
        }
    }
}
