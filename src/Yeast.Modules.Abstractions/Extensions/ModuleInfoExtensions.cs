using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Yeast.Modules.Abstractions
{
    public static class ModuleInfoExtensions
    {
        /// <summary>
        /// Indicates if a type is derived from <see cref="ModuleInfo"/>
        /// </summary>
        /// <param name="type">Tested type</param>
        /// <returns>True if type is derived from <see cref="ModuleInfo"/></returns>
        public static bool IsModuleInfo(this Type type)
        {
            var typeInfo = type.GetTypeInfo();
            return typeInfo.IsClass && !typeInfo.IsAbstract && !typeInfo.IsGenericType && typeof(ModuleInfo).IsAssignableFrom(type);
        }

        /// <summary>
        /// Finds the first type derived from <see cref="ModuleInfo"/> in the list
        /// </summary>
        /// <param name="types">The list of types to test</param>
        /// <returns>The first type derived from <see cref="ModuleInfo"/> or null</returns>
        public static Type FindModuleInfo(this IEnumerable<Type> types)
        {
            return types.FirstOrDefault(type => type.IsModuleInfo());
        }
    }
}
