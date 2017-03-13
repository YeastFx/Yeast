using System.Collections.Generic;

namespace Yeast.Features.Abstractions
{
    public interface IFeatureProvider
    {
        /// <summary>
        /// The collection of provided features
        /// </summary>
        IEnumerable<FeatureInfo> Features { get; }
    }
}
