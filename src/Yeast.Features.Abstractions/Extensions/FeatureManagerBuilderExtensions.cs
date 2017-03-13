using System;

namespace Yeast.Features.Abstractions
{
    public static class FeatureManagerBuilderExtensions
    {
        /// <summary>
        /// Registers multiples <see cref="IFeatureProvider"/>
        /// </summary>
        /// <param name="builder">The <see cref="IFeatureManagerBuilder"/> reference</param>
        /// <param name="featureProviders">List of <see cref="IFeatureProvider"/> to register</param>
        /// <returns>The <see cref="IFeatureManagerBuilder"/> reference</returns>
        public static IFeatureManagerBuilder AddFeatureProviders(this IFeatureManagerBuilder builder, params IFeatureProvider[] featureProviders)
        {
            if(builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            foreach(var featureProvider in featureProviders)
            {
                builder.AddFeatureProvider(featureProvider);
            }

            return builder;
        }

        /// <summary>
        /// Enables multiple features by their types
        /// </summary>
        /// <param name="builder">The <see cref="IFeatureManagerBuilder"/> reference</param>
        /// <param name="featureTypes">List of feature types to activate</param>
        /// <returns>The <see cref="IFeatureManagerBuilder"/> reference</returns>
        public static IFeatureManagerBuilder EnableFeatures(this IFeatureManagerBuilder builder, params Type[] featureTypes)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            foreach (var featureName in featureTypes)
            {
                builder.EnableFeature(featureName);
            }

            return builder;
        }

        /// <summary>
        /// Enables multiple features by their names
        /// </summary>
        /// <param name="builder">The <see cref="IFeatureManagerBuilder"/> reference</param>
        /// <param name="featureNames">List of feature names to activate</param>
        /// <returns>The <see cref="IFeatureManagerBuilder"/> reference</returns>
        public static IFeatureManagerBuilder EnableFeatures(this IFeatureManagerBuilder builder, params string[] featureNames)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            foreach (var featureName in featureNames)
            {
                builder.EnableFeature(featureName);
            }

            return builder;
        }
    }
}
