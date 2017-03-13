namespace Yeast.Features.Abstractions
{
    public abstract class FeatureInfo
    {
        public virtual string Name {
            get {
                return GetType().Name;
            }
        }
    }
}
