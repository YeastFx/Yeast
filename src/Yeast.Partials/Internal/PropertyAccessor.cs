using System;

namespace Yeast.Partials.Internal
{
    internal abstract class PropertyAccessor
    {
        public abstract string PropertyName { get; }

        public abstract Type PropertyType { get; }

        public abstract object GetPropertyValue(object target);
    }
}
