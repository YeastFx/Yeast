namespace Yeast.Features.Abstractions
{
    public abstract class FeatureInfo
    {
        /// <summary>
        /// Unique name of the Feature
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Feature's display name
        /// </summary>
        public virtual string DisplayName { get => Name; }

        /// <summary>
        /// Feature registration priority
        /// </summary>
        public virtual int Priority { get; }
    }
}
