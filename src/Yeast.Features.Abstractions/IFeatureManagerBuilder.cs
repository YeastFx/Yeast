namespace Yeast.Features.Abstractions
{
    public interface IFeatureManagerBuilder
    {
        /// <summary>
        /// Registers an <see cref="IFeatureProvider"/>
        /// </summary>
        /// <param name="featureProvider">The <see cref="IFeatureProvider"/> instance</param>
        /// <returns>The <see cref="IFeatureManagerBuilder"/> reference</returns>
        IFeatureManagerBuilder AddFeatureProvider(IFeatureProvider featureProvider);

        /// <summary>
        /// Enables a feature by its name
        /// </summary>
        /// <param name="featureName">The feature name to enable</param>
        /// <returns>The <see cref="IFeatureManagerBuilder"/> reference</returns>
        IFeatureManagerBuilder EnableFeature(string featureName);
    }
}