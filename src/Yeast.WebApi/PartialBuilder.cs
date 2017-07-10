using System.Collections.Generic;
using Yeast.WebApi.Internal;

namespace Yeast.WebApi
{
    /// <summary>
    /// Helper class to project objects with filtered fields
    /// </summary>
    public static class PartialBuilder
    {
        /// <summary>
        /// Projects an objects with only readable value type properties
        /// </summary>
        /// <param name="source">The object to project</param>
        /// <returns>Projected object</returns>
        public static object ToPartial(object source)
        {
            return ToPartial(source, null, null);
        }

        /// <summary>
        /// Projects an objects with only specified fields
        /// </summary>
        /// <param name="source">The object to project</param>
        /// <param name="fields">Field names list</param>
        /// <returns>Projected object</returns>
        public static object ToPartial(object source, IEnumerable<string> fields)
        {
            return ToPartial(source, fields, null);
        }

        /// <summary>
        /// Projects an objects with only specified fields
        /// </summary>
        /// <param name="source">The object to project</param>
        /// <param name="fields">Field names list</param>
        /// <param name="fieldNameComparer">Field names equality comparer</param>
        /// <returns>Projected object</returns>
        public static object ToPartial(object source, IEnumerable<string> fields, IEqualityComparer<string> fieldNameComparer)
        {
            if (source == null)
            {
                return null;
            }

            return PartialObject.ProjectObject(source, fields, fieldNameComparer);
        }
    }
}
