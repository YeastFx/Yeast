using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Yeast.Partials.Internal
{
    internal class TypedPropertyAccessor : PropertyAccessor
    {
        private static Dictionary<Type, IEnumerable<TypedPropertyAccessor>> availablePropertiesCache = new Dictionary<Type, IEnumerable<TypedPropertyAccessor>>();

        private readonly PropertyInfo propInfo;

        public TypedPropertyAccessor(PropertyInfo propertyInfo)
        {
            propInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
        }

        public override string PropertyName => propInfo.Name;

        public override Type PropertyType => propInfo.PropertyType;

        public override object GetPropertyValue(object target)
        {
            return propInfo.GetValue(target);
        }

        public static IEnumerable<TypedPropertyAccessor> GetAvailableProperties(object target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            var targetType = target.GetType();

            if (availablePropertiesCache.ContainsKey(targetType))
            {
                return availablePropertiesCache[targetType];
            }
            else
            {
                var accessors = targetType
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(propInfo => propInfo.CanRead)
                    .Select(propInfo => new TypedPropertyAccessor(propInfo))
                    .ToList();

                availablePropertiesCache[targetType] = accessors;

                return accessors;
            }
        }
    }
}
