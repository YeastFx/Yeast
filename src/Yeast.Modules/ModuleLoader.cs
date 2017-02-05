using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.IO;
using System.Runtime.Loader;

namespace Yeast.Modules
{
    /// <summary>
    /// Loads assemblies from module directories
    /// </summary>
    public class ModuleLoader
    {
        private const string DefaultModulesDirectory = "Modules";

        private readonly ILogger _logger;

        /// <summary>
        /// Creates a new instance without Logging
        /// </summary>
        public ModuleLoader() {
            _logger = NullLogger.Instance;
        }

        /// <summary>
        /// Creates a new instance with Logging
        /// </summary>
        public ModuleLoader(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ModuleLoader>();
        }

        /// <summary>
        /// Loads all modules from a module directory path
        /// </summary>
        /// <param name="modulesDirectory">The modules directory path</param>
        public void LoadModules(string modulesDirectory = DefaultModulesDirectory)
        {
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
        /// <param name="moduleDirectory">The module directory path</param>
        public void LoadModule(string moduleDirectory)
        {
            if (Directory.Exists(moduleDirectory))
            {
                LoadModuleInternal(moduleDirectory);
            }
        }

        private void LoadModuleInternal(string modulePath)
        {
            var moduleName = Path.GetDirectoryName(modulePath);
            _logger.LogDebug($"Loadding module {moduleName}...");
            var binPath = Path.Combine(modulePath, "bin");
            if (!Directory.Exists(binPath))
            {
                // Nothing to load
                return;
            }
            foreach (var assemblyFile in Directory.EnumerateFiles(binPath, "*.dll", SearchOption.AllDirectories))
            {
                try
                {
                    var assemblyPath = Path.GetFullPath(assemblyFile);
                    var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);

                }
                catch (Exception exp)
                {
                    _logger.LogWarning($"Loadding file {assemblyFile} failed.{Environment.NewLine}{exp.Message}");
                }
            }

        }
    }
}
