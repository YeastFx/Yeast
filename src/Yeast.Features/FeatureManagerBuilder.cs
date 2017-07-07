using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Yeast.Features.Abstractions;

namespace Yeast.Features
{
    public class FeatureManagerBuilder : IFeatureManagerBuilder
    {
        private readonly HashSet<IFeatureProvider> _featureProviders;
        private readonly HashSet<string> _enabledFeatureNames;
        private readonly HashSet<FeatureInfo> _enabledFeatureInstances;
        private readonly HashSet<Type> _enabledFeatureTypes;

        private readonly ILogger _logger;

        public FeatureManagerBuilder(ILoggerFactory loggerFactory = null)
        {
            _logger = (ILogger)loggerFactory?.CreateLogger<FeatureManager>() ?? NullLogger.Instance;

            _featureProviders = new HashSet<IFeatureProvider>();
            _enabledFeatureNames = new HashSet<string>();
            _enabledFeatureInstances = new HashSet<FeatureInfo>();
            _enabledFeatureTypes = new HashSet<Type>();
        }

        /// <inheritdoc />
        public IFeatureManagerBuilder AddFeatureProvider(IFeatureProvider featureProvider)
        {
            if(featureProvider == null)
            {
                throw new ArgumentNullException(nameof(featureProvider));
            }

            _featureProviders.Add(featureProvider);

            _logger.LogInformation($"Feature provider {featureProvider.GetType().Name} registered.");

            return this;
        }

        /// <inheritdoc />
        public IFeatureManagerBuilder EnableFeature(string featureName)
        {
            if (featureName == null)
            {
                throw new ArgumentNullException(nameof(featureName));
            }

            _enabledFeatureNames.Add(featureName);

            _logger.LogInformation($"Feature named {featureName} requested.");

            return this;
        }

        /// <inheritdoc />
        public IFeatureManagerBuilder EnableFeature(FeatureInfo feature)
        {
            if (feature == null)
            {
                throw new ArgumentNullException(nameof(feature));
            }

            _enabledFeatureInstances.Add(feature);

            _logger.LogInformation($"Feature instance of {feature.Name} requested.");

            return this;
        }

        /// <inheritdoc />
        public IFeatureManagerBuilder EnableFeature(Type featureType)
        {
            if (featureType == null)
            {
                throw new ArgumentNullException(nameof(featureType));
            }

            _enabledFeatureTypes.Add(featureType);

            _logger.LogInformation($"Feature of type {featureType} requested.");

            return this;
        }

        internal FeatureManager Build()
        {
            _logger.LogInformation($"Building {nameof(FeatureManager)}...");

            var availableFeatures = _featureProviders
                .SelectMany(featureProvider => featureProvider.Features)
                .Distinct()
                .OrderByDescending(feature => feature.Priority);

            var enabledFeatures = new List<FeatureInfo>();

            // Enable by instance
            foreach (var requestedFeature in _enabledFeatureInstances)
            {
                if (!availableFeatures.Contains(requestedFeature))
                {
                    _logger.LogError($"Missing requested feature instance : {requestedFeature.Name}");
                    throw new InvalidOperationException($"Feature instance {requestedFeature.Name} is not available.");
                }
                else
                {
                    enabledFeatures.Add(requestedFeature);
                }
            }

            // Enable by name
            foreach (var requestedFeatureName in _enabledFeatureNames)
            {
                if(enabledFeatures.Any(feature => feature.Name == requestedFeatureName))
                {
                    continue;
                }
                try
                {
                    enabledFeatures.Add(availableFeatures.First(feature => feature.Name == requestedFeatureName));
                }
                catch(InvalidOperationException)
                {
                    _logger.LogError($"Missing requested feature : {requestedFeatureName}");
                    throw new InvalidOperationException($"Feature named {requestedFeatureName} is not available.");
                }
            }

            // Enable by type
            foreach(var requestedFeatureType in _enabledFeatureTypes)
            {
                if (enabledFeatures.Any(feature => requestedFeatureType.GetTypeInfo().IsAssignableFrom(feature.GetType().GetTypeInfo())))
                {
                    continue;
                }
                try
                {
                    enabledFeatures.Add(availableFeatures.First(feature => requestedFeatureType.GetTypeInfo().IsAssignableFrom(feature.GetType().GetTypeInfo())));
                }
                catch (InvalidOperationException)
                {
                    _logger.LogError($"Missing requested feature type : {requestedFeatureType.Name}");
                    throw new InvalidOperationException($"Feature of type {requestedFeatureType.Name} is not available.");
                }
            }

            // Sort features
            var prioritizedFeatures = enabledFeatures.OrderByDescending(feature => feature.Priority);

            return new FeatureManager(availableFeatures, prioritizedFeatures);
        }
    }
}
