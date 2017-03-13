using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using Yeast.Features.Abstractions;

namespace Yeast.Modules
{
    /// <summary>
    /// Loads assemblies from module directories
    /// </summary>
    public class ModuleLoader : IModuleLoader
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

        /// <inheritdoc />
        public IEnumerable<FeatureInfo> Features => _loadedModules.SelectMany(module => module.Features);

        /// <inheritdoc />
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

        /// <inheritdoc />
        public void LoadModule(string moduleDirectory)
        {
            if (Directory.Exists(moduleDirectory))
            {
                LoadModuleInternal(moduleDirectory);
            }
        }

        /// <inheritdoc />
        public ILoadedModule GetLoadedModuleByName(string moduleName)
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
