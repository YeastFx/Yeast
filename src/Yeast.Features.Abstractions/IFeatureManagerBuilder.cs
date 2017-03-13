namespace Yeast.Features.Abstractions
{
    public interface IFeatureManagerBuilder
    {
        IFeatureManagerBuilder AddFeatureProvider(IFeatureProvider featureProvider);
        IFeatureManagerBuilder EnableFeature(string featureName);
    }
}