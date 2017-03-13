using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Yeast.Modules
{
    /// <summary>
    /// Contains extension methods to <see cref="IServiceCollection"/> for using modules
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds <see cref="ModuleLoader"/> to the service collection
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> reference</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> instance</param>
        /// <param name="config">The <see cref="ModuleLoaderConfiguration"/></param>
        /// <returns>The <see cref="IModuleLoader"/> reference</returns>
        public static IModuleLoader AddModuleLoader(this IServiceCollection services, ILoggerFactory loggerFactory = null, IModuleLoaderConfiguration config = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var moduleLoader = new ModuleLoader(loggerFactory);

            config?.Invoke(moduleLoader);

            services.AddSingleton<IModuleLoader>(moduleLoader);

            return moduleLoader;
        }
    }
}
