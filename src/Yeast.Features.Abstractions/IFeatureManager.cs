using System.Collections.Generic;

namespace Yeast.Features.Abstractions
{
    public interface IFeatureManager
    {
        /// <summary>
        /// Gets list of available features
        /// </summary>
        IEnumerable<FeatureInfo> AvailableFeatures { get; }

        /// <summary>
        /// Gets list of enabled features
        /// </summary>
        IEnumerable<FeatureInfo> EnabledFeatures { get; }
    }
}