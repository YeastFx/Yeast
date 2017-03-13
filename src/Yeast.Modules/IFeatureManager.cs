using System.Collections.Generic;
using Yeast.Features.Abstractions;

namespace Yeast.Modules
{
    public interface IFeatureManager
    {
        /// <summary>
        /// Gets list of enabled features
        /// </summary>
        IEnumerable<FeatureInfo> EnabledFeatures { get; }
    }
}