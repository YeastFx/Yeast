using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Yeast.Partials.Internal
{
    internal class PartialObject : DynamicObject
    {
        private readonly Dictionary<string, object> values = new Dictionary<string, object>();

        public PartialObject(object source, Func<object, IEnumerable<PropertyAccessor>> propertiesParser, IEnumerable<string> fields, IEqualityComparer<string> fieldNameComparer)
        {
            if(source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var propertyAccessors = propertiesParser(source);

            if(fields != null && fields.Any())
            {
                values = PopulateValues(source, propertyAccessors, fields, fieldNameComparer);
            }
            else
            {
                values = PopulateDefaultValues(source, propertyAccessors);
            }
        }

        #region DynamicObject overrides
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return values.Keys;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binder">The binder provided by the call site.</param>
        /// <param name="result">The result of the get operation.</param>
        /// <returns>true if the operation is complete, false if the call site should determine behavior.</returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (values.Keys.Contains(binder.Name))
            {
                result = values[binder.Name];
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        #endregion

        #region Static methods
        /// <summary>
        /// Projects an object
        /// </summary>
        /// <param name="value">Source object</param>
        /// <param name="fields">The list of property names to evaluate</param>
        /// <param name="fieldNameComparer">Field names equality comparer</param>
        /// <returns></returns>
        internal static object ProjectObject(object value, IEnumerable<string> fields, IEqualityComparer<string> fieldNameComparer)
        {
            if (value == null)
            {
                return null;
            }
            var valueType = value.GetType();
            if (valueType == typeof(string) || valueType.GetTypeInfo().IsValueType)
            {
                return value;
            }
            else if (valueType.IsArray)
            {
                return ProjectArray((Array)value, fields, fieldNameComparer);
            }
            else if (IsDynamic(valueType))
            {
                return new PartialObject(value, DynamicPropertyAccessor.GetAvailableProperties, fields, fieldNameComparer);
            }
            else if (IsDictionary(valueType))
            {
                return ProjectDictionary((IDictionary)value, fields, fieldNameComparer);
            }
            else if (IsEnumerable(valueType))
            {
                return ProjectEnumerable((IEnumerable)value, fields, fieldNameComparer);
            }
            else
            {
                return new PartialObject(value, TypedPropertyAccessor.GetAvailableProperties, fields, fieldNameComparer);
            }
        }

        /// <summary>
        /// Populates values with default fields
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="fieldNames">The list of property names to evaluate</param>
        /// <param name="propertyAccessors">The list of field value accessors</param>
        /// <param name="fieldNameComparer">Field names equality comparer</param>
        /// <returns>Property values dictionary</returns>
        private static Dictionary<string, object> PopulateValues(object source, IEnumerable<PropertyAccessor> propertyAccessors , IEnumerable<string> fieldNames, IEqualityComparer<string> fieldNameComparer)
        {
            var ownProperties = fieldNames.Select(fieldname =>
            {
                int idx = fieldname.IndexOf('.');
                return idx >= 0 ?
                    new Tuple<string, string>(fieldname.Substring(0, idx), fieldname.Substring(idx + 1)) :
                    new Tuple<string, string>(fieldname, null);
            })
            .GroupBy(field => field.Item1, field => field.Item2, fieldNameComparer)
            .Join(propertyAccessors, ownField => ownField.Key, propertyInfo => propertyInfo.PropertyName, (ownField, propertyAccessor) => new Tuple<PropertyAccessor, IEnumerable<string>>(propertyAccessor, ownField.Where(navField => navField != null)), fieldNameComparer);

            var result = new Dictionary<string, object>();
            foreach (var ownProperty in ownProperties)
            {
                result[ownProperty.Item1.PropertyName] = ProjectObject(ownProperty.Item1.GetPropertyValue(source), ownProperty.Item2, fieldNameComparer);
            }
            return result;
        }

        /// <summary>
        /// Populates values with default fields
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="propertyAccessors">The list of field value accessors</param>
        /// <returns>Property values dictionary</returns>
        private static Dictionary<string, object> PopulateDefaultValues(object source, IEnumerable<PropertyAccessor> propertyAccessors)
        {
            var availableProperties = propertyAccessors
                .Where(propAccessor => propAccessor.PropertyType == typeof(string) || propAccessor.PropertyType.GetTypeInfo().IsValueType);

            var result = new Dictionary<string, object>();
            foreach (var property in availableProperties)
            {
                result[property.PropertyName] = ProjectObject(property.GetPropertyValue(source), null, null);
            }
            return result;
        }

        private static Array ProjectArray(Array srcArray, IEnumerable<string> fields, IEqualityComparer<string> fieldNameComparer)
        {
            List<int> dimensions = new List<int>();
            for (int i = 0; i < srcArray.Rank; i++)
            {
                dimensions.Add(srcArray.GetUpperBound(i) + 1);
            }
            var result = Array.CreateInstance(typeof(object), dimensions.ToArray());
            Action<int[]> SetDimension = null;
            SetDimension = (indexes) => {
                for (int i = 0; i < dimensions[indexes.Length]; i++)
                {
                    var currIndexes = new int[indexes.Length + 1];
                    indexes.CopyTo(currIndexes, 0);
                    currIndexes[indexes.Length] = i;
                    if (indexes.Length == dimensions.Count - 1)
                    {
                        var value = srcArray.GetValue(currIndexes);
                        result.SetValue(ProjectObject(value, fields, fieldNameComparer), currIndexes);
                    }
                    else
                    {
                        SetDimension(currIndexes);
                    }
                }
            };
            SetDimension(new int[0]);
            return result;
        }

        private static IEnumerable ProjectEnumerable(IEnumerable srcEnumerable, IEnumerable<string> fields, IEqualityComparer<string> fieldNameComparer)
        {
            var result = new List<object>();
            foreach (var value in srcEnumerable)
            {
                result.Add(ProjectObject(value, fields, fieldNameComparer));
            }
            return result;
        }

        private static IDictionary ProjectDictionary(IDictionary srcDictionary, IEnumerable<string> fields, IEqualityComparer<string> fieldNameComparer)
        {
            var result = new Dictionary<object, object>();
            foreach (var key in srcDictionary.Keys)
            {
                result.Add(key, ProjectObject(srcDictionary[key], fields, fieldNameComparer));
            }
            return result;
        }

        private static bool IsDynamic(Type type)
        {
            return type.GetInterfaces()
                .Any(t => t == typeof(IDynamicMetaObjectProvider));
        }

        private static bool IsEnumerable(Type type)
        {
            return type.GetInterfaces()
                .Any(t => t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        }

        private static bool IsDictionary(Type type)
        {
            return type.GetInterfaces()
                .Any(t => t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition() == typeof(IDictionary<,>));
        }

        #endregion
    }
}
