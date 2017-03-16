using Microsoft.Extensions.DependencyInjection;

namespace Yeast.Features.Abstractions
{
    public abstract class FeatureInfo
    {
        /// <summary>
        /// Unique name of the Feature
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Feature's display name
        /// </summary>
        public virtual string DisplayName { get => Name; }

        /// <summary>
        /// Feature registration priority
        /// </summary>
        public virtual int Priority { get; }

        /// <summary>
        /// Method to configure the feature
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> reference.</param>
        /// <param name="featureManager">The <see cref="IFeatureManager"/> reference.</param>
        public virtual void ConfigureServices(IServiceCollection services, IFeatureManager featureManager) { }
    }
}
