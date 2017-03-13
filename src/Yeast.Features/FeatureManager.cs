using System.Collections.Generic;
using Yeast.Features.Abstractions;

namespace Yeast.Features
{
    public class FeatureManager : IFeatureManager
    {
        private readonly IEnumerable<FeatureInfo> _availableFeatures;
        private readonly IEnumerable<FeatureInfo> _enabledFeatures;

        internal FeatureManager(IEnumerable<FeatureInfo> availableFeatures, IEnumerable<FeatureInfo> enabledFeatures)
        {
            _availableFeatures = availableFeatures;
            _enabledFeatures = enabledFeatures;
        }

        /// <inheritdoc />
        public IEnumerable<FeatureInfo> EnabledFeatures => _enabledFeatures;

        /// <inheritdoc />
        public IEnumerable<FeatureInfo> AvailableFeatures => _availableFeatures;
    }
}
