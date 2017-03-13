using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Yeast.Features.Abstractions;

namespace Yeast.Features.Extensions
{
    /// <summary>
    /// Contains extension methods to <see cref="IServiceCollection"/> for using features
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds <see cref="FeatureManager"/> to the service collection
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> reference</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> instance</param>
        /// <param name="builderConfiguration">The <see cref="IFeatureManagerBuilder"/> configuration action</param>
        /// <returns>The <see cref="IServiceCollection"/> reference</returns>
        public static IServiceCollection AddFeatures(this IServiceCollection services, ILoggerFactory loggerFactory = null, Action<IFeatureManagerBuilder> builderConfiguration = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var builder = new FeatureManagerBuilder(loggerFactory);

            builderConfiguration?.Invoke(builder);

            return services.AddSingleton<IFeatureManager>(builder.Build());
        }
    }
}
