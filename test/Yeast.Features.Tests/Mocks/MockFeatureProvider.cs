using System.Collections.Generic;
using Yeast.Features.Abstractions;

namespace Yeast.Features.Tests.Mocks
{
    public class MockFeatureProvider : IFeatureProvider
    {
        public IEnumerable<FeatureInfo> Features { get; set; }
    }
}
