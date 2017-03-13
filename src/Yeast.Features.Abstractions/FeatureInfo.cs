namespace Yeast.Features.Abstractions
{
    public abstract class FeatureInfo
    {
        public string Name {
            get {
                return GetType().Name;
            }
        }
    }
}
