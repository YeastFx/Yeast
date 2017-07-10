using System;

namespace Yeast.WebApi.Internal
{
    internal abstract class PropertyAccessor
    {
        public abstract string PropertyName { get; }

        public abstract Type PropertyType { get; }

        public abstract object GetPropertyValue(object target);
    }
}
