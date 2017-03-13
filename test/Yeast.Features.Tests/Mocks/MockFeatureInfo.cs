using Yeast.Features.Abstractions;

namespace Yeast.Features.Tests.Mocks
{
    public class MockFeatureInfo : FeatureInfo
    {
        private readonly string _featureName;

        public MockFeatureInfo(string featureName)
        {
            _featureName = featureName;
        }

        public override string Name => _featureName;
    }
}
