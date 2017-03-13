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

    public class MockInheritedFeatureInfo : MockFeatureInfo
    {
        public MockInheritedFeatureInfo(string featureName) : base(featureName) { }
    }

    public interface IMockInheritedFeatureInfo { }

    public class MockInheritedFeatureInfoWithInterface : MockFeatureInfo, IMockInheritedFeatureInfo
    {
        public MockInheritedFeatureInfoWithInterface(string featureName) : base(featureName) { }
    }
}
