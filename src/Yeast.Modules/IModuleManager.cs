using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Yeast.Modules
{
    public interface IModuleManager
    {
        /// <summary>
        /// Gets list of enabled module names
        /// </summary>
        IEnumerable<string> EnabledModules { get; }

        /// <summary>
        /// Configures enabled modules
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/> instance</param>
        /// <returns>The <see cref="IApplicationBuilder"/> instance</returns>
        IApplicationBuilder ConfigureModules(IApplicationBuilder builder);

        /// <summary>
        /// Configures enabled modules services
        /// </summary>
        /// <param name="services">The application <see cref="IServiceCollection"/></param>
        IServiceCollection ConfigureModulesServices(IServiceCollection services);

        /// <summary>
        /// Enables listed modules
        /// </summary>
        /// <param name="moduleNames">The list of module names to enable</param>
        void EnableModules(params string[] moduleNames);
    }
}