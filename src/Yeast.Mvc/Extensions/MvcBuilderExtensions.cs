using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;
using Yeast.Features.Abstractions;
using Yeast.Mvc.Abstractions;

namespace Yeast.Mvc
{
    public static class MvcBuilderExtensions
    {
        /// <summary>
        /// Adds enabled MvcModules assemblies as ApplicationParts
        /// </summary>
        /// <param name="builder">The <see cref="IMvcBuilder"/>.</param>
        /// <param name="featureManager">The <see cref="IFeatureManager"/></param>
        /// <returns>The <see cref="IMvcBuilder"/>.</returns>
        public static IMvcBuilder AddMvcModules(this IMvcBuilder builder, IFeatureManager featureManager)
        {
            foreach(var mvcModule in featureManager.EnabledFeatures.OfType<MvcModuleInfo>())
            {
                builder.AddApplicationPart(mvcModule.GetType().GetTypeInfo().Assembly);
            }

            return builder;
        }
    }
}
