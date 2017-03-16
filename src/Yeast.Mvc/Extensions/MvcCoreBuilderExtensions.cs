using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;
using Yeast.Features.Abstractions;
using Yeast.Mvc.Abstractions;

namespace Yeast.Mvc
{
    public static class MvcCoreBuilderExtensions
    {
        /// <summary>
        /// Adds enabled MvcModules assemblies as ApplicationParts
        /// </summary>
        /// <param name="builder">The <see cref="IMvcCoreBuilder"/>.</param>
        /// <param name="featureManager">The <see cref="IFeatureManager"/></param>
        /// <returns></returns>
        public static IMvcCoreBuilder AddMvcModules(this IMvcCoreBuilder builder, IFeatureManager featureManager)
        {
            foreach(var mvcModule in featureManager.EnabledFeatures.OfType<MvcModuleInfo>())
            {
                builder.AddApplicationPart(mvcModule.GetType().GetTypeInfo().Assembly);
            }

            return builder;
        }
    }
}
