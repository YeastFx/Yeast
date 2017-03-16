using System.Collections.Generic;
using Yeast.Features.Abstractions;

namespace Yeast.Mvc.Tests.Mocks
{
    public class MockFeatureManager : IFeatureManager
    {
        public IEnumerable<FeatureInfo> AvailableFeatures { get; set; }

        public IEnumerable<FeatureInfo> EnabledFeatures { get; set; }
    }
}
