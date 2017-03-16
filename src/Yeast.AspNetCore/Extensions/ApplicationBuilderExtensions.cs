using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Yeast.AspNetCore.Abstractions;
using Yeast.Features.Abstractions;

namespace Yeast.AspNetCore
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds enabled features pipeline configurations
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        public static IApplicationBuilder UseFeatures(this IApplicationBuilder app)
        {
            if(app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            var featureManager = app.ApplicationServices.GetRequiredService<IFeatureManager>();

            foreach(var pipelineConfig in featureManager.EnabledFeatures.OfType<IConfigurePipeline>())
            {
                pipelineConfig.Configure(app);
            }

            return app;
        }
    }
}
