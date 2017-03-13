using System;

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

        /// <summary>
        /// Enables a feature by instance
        /// </summary>
        /// <param name="feature">The feature to enable reference</param>
        /// <returns>The <see cref="IFeatureManagerBuilder"/> reference</returns>
        IFeatureManagerBuilder EnableFeature(FeatureInfo feature);

        /// <summary>
        /// Enables a feature by its type
        /// </summary>
        /// <param name="featureType">The feature type to enable</param>
        /// <returns>The <see cref="IFeatureManagerBuilder"/> reference</returns>
        IFeatureManagerBuilder EnableFeature(Type featureType);
    }
}