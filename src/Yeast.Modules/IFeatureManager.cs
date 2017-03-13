using System.Collections.Generic;
using Yeast.Modules.Abstractions;

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