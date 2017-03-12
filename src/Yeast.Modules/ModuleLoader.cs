using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;

namespace Yeast.Modules
{
    /// <summary>
    /// Loads assemblies from module directories
    /// </summary>
    public class ModuleLoader
    {
        private readonly ILogger _logger;
        private readonly HashSet<LoadedModule> _loadedModules;

        /// <summary>
        /// Creates a new instance without Logging
        /// </summary>
        public ModuleLoader() : this(null) { }

        /// <summary>
        /// Creates a new instance with Logging
        /// </summary>
        public ModuleLoader(ILoggerFactory loggerFactory)
        {
            _logger = (ILogger)loggerFactory?.CreateLogger<ModuleLoader>() ?? NullLogger.Instance;
            _loadedModules = new HashSet<LoadedModule>();
        }

        /// <summary>
        /// Loads all modules from a module directory path
        /// </summary>
        /// <param name="modulesDirectory">The modules directory path</param>
        public void LoadModules(string modulesDirectory)
        {
            if(string.IsNullOrEmpty(modulesDirectory))
            {
                throw new ArgumentException($"{nameof(modulesDirectory)} cannot be null or empty.");
            }

            if (Directory.Exists(modulesDirectory))
            {
                foreach (var modulePath in Directory.EnumerateDirectories(modulesDirectory))
                {
                    LoadModuleInternal(modulePath);
                }
            }
        }

        /// <summary>
        /// Loads individual module from its directory path
        /// </summary>
        /// <param name="moduleDirectory">The module directory path.</param>
        public void LoadModule(string moduleDirectory)
        {
            if (Directory.Exists(moduleDirectory))
            {
                LoadModuleInternal(moduleDirectory);
            }
        }

        /// <summary>
        /// Gets a <see cref="LoadedModule"/> by its Name
        /// </summary>
        /// <param name="moduleName">The module name</param>
        /// <returns>The <see cref="LoadedModule"/> instance if module was loaded.</returns>
        public LoadedModule GetLoadedModuleByName(string moduleName)
        {
            return _loadedModules.FirstOrDefault(loadedModule => loadedModule.Name == moduleName);
        }

        private void LoadModuleInternal(string modulePath)
        {
            var moduleName = new DirectoryInfo(modulePath).Name;
            _logger.LogDebug($"Loadding module {moduleName}...");
            foreach (var assemblyFile in Directory.EnumerateFiles(modulePath, $"{moduleName}.dll", SearchOption.AllDirectories))
            {
                try
                {
                    var assemblyPath = Path.GetFullPath(assemblyFile);
                    var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
                    _loadedModules.Add(new LoadedModule(modulePath, assembly));
                    break;
                }
                catch (Exception exp)
                {
                    _logger.LogWarning($"Loadding file {assemblyFile} failed.{Environment.NewLine}{exp.Message}");
                }
            }
        }
    }
}
