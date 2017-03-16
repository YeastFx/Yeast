using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Yeast.Features.Abstractions;

namespace Yeast.Features
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
        /// <param name="builderConfiguration">The <see cref="IFeatureManagerBuilder"/> configuration action</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> instance</param>
        /// <returns>The <see cref="IFeatureManager"/> reference</returns>
        public static IFeatureManager AddFeatures(this IServiceCollection services, Action<IFeatureManagerBuilder> builderConfiguration, ILoggerFactory loggerFactory = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (builderConfiguration == null)
            {
                throw new ArgumentNullException(nameof(builderConfiguration));
            }

            var builder = new FeatureManagerBuilder(loggerFactory);

            builderConfiguration?.Invoke(builder);

            var featureManager = builder.Build();

            services.AddSingleton<IFeatureManager>(featureManager);

            foreach(var enabledFeature in featureManager.EnabledFeatures)
            {
                enabledFeature.ConfigureServices(services, featureManager);
            }

            return featureManager;
        }
    }
}
