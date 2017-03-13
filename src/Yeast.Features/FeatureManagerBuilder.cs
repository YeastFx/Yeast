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

        public FeatureManagerBuilder(ILoggerFactory loggerFactory)
        {
            _logger = (ILogger)loggerFactory?.CreateLogger<FeatureManager>() ?? NullLogger.Instance;

            _featureProviders = new HashSet<IFeatureProvider>();
            _enabledFeatures = new HashSet<string>();
        }

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

            var enabledFeatures = availableFeatures.Where(feature => _enabledFeatures.Contains(feature.Name));

            return new FeatureManager(availableFeatures.AsEnumerable(), enabledFeatures);
        }
    }
}
