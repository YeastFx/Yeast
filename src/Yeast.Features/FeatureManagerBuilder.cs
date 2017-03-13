using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using Yeast.Features.Abstractions;

namespace Yeast.Features
{
    public class FeatureManagerBuilder : IFeatureManagerBuilder
    {
        private readonly HashSet<IFeatureProvider> _featureProviders;
        private readonly HashSet<string> _enabledFeatures;

        private readonly ILogger _logger;

        public FeatureManagerBuilder(ILoggerFactory loggerFactory = null)
        {
            _logger = (ILogger)loggerFactory?.CreateLogger<FeatureManager>() ?? NullLogger.Instance;

            _featureProviders = new HashSet<IFeatureProvider>();
            _enabledFeatures = new HashSet<string>();
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

            _enabledFeatures.Add(featureName);

            _logger.LogInformation($"Feature {featureName} requested.");

            return this;
        }

        internal FeatureManager Build()
        {
            _logger.LogInformation($"Building {nameof(FeatureManager)}...");

            var availableFeatures = _featureProviders.SelectMany(featureProvider => featureProvider.Features).Distinct();

            var enabledFeatures = new HashSet<FeatureInfo>();
            foreach(var requestedFeature in _enabledFeatures)
            {
                try
                {
                    enabledFeatures.Add(availableFeatures.First(feature => feature.Name == requestedFeature));
                }
                catch(InvalidOperationException)
                {
                    _logger.LogError($"Missing requested feature : {requestedFeature}");
                    throw new InvalidOperationException($"Feature named {requestedFeature} is not available.");
                }
            }

            return new FeatureManager(availableFeatures.AsEnumerable(), enabledFeatures);
        }
    }
}
