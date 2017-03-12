namespace Yeast.Modules
{
    public static class ModuleLoaderExtensions
    {
        /// <summary>
        /// Tests if a module was loaded
        /// </summary>
        /// <param name="moduleName">Module name to test</param>
        /// <returns>True if module was loaded, otherwise False</returns>
        public static bool IsLoaded(this IModuleLoader moduleLoader, string moduleName)
        {
            return moduleLoader.GetLoadedModuleByName(moduleName) != null;
        }

    }
}
