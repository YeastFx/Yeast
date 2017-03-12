using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Yeast.Modules
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Applies enabled modules configuration
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> instance</param>
        /// <returns>The <see cref="IApplicationBuilder"/> instance</returns>
        public static IApplicationBuilder ConfigureModules(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            var moduleManager = app.ApplicationServices.GetRequiredService<IModuleManager>();

            return moduleManager.ConfigureModules(app);
        }
    }
}
