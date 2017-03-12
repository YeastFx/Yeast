namespace Yeast.Modules
{
    public interface IModuleLoader
    {
        /// <summary>
        /// Gets an <see cref="ILoadedModule"/> by its Name
        /// </summary>
        /// <param name="moduleName">The module name</param>
        /// <returns>The <see cref="ILoadedModule"/> instance if module was loaded.</returns>
        ILoadedModule GetLoadedModuleByName(string moduleName);

        /// <summary>
        /// Loads individual module from its directory path
        /// </summary>
        /// <param name="moduleDirectory">The module directory path.</param>
        void LoadModule(string moduleDirectory);

        /// <summary>
        /// Loads all modules from a module directory path
        /// </summary>
        /// <param name="modulesDirectory">The modules directory path</param>
        void LoadModules(string modulesDirectory);
    }
}