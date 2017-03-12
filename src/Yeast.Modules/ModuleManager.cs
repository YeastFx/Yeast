using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using Yeast.Modules.Abstractions;

namespace Yeast.Modules
{
    public class ModuleManager : IModuleManager
    {
        private readonly HashSet<string> _enabledModules;
        private readonly ILogger _logger;
        private readonly IModuleLoader _moduleLoader;

        public ModuleManager(IModuleLoader moduleLoader) : this(moduleLoader, null) { }

        public ModuleManager(IModuleLoader moduleLoader, ILoggerFactory loggerFactory)
        {
            _moduleLoader = moduleLoader ?? throw new ArgumentNullException(nameof(moduleLoader));

            _logger = (ILogger)loggerFactory?.CreateLogger<ModuleManager>() ?? NullLogger.Instance;

            _enabledModules = new HashSet<string>();
        }

        /// <inheritdoc />
        public IEnumerable<string> EnabledModules {
            get { return _enabledModules.AsEnumerable(); }
        }

        /// <inheritdoc />
        public void EnableModules(params string[] moduleNames)
        {
            foreach(var moduleName in moduleNames)
            {
                if(_moduleLoader.IsLoaded(moduleName))
                {
                    _enabledModules.Add(moduleName);
                }
                else
                {
                    _logger.LogError($"Missing module : {moduleName}");
                }
            }
        }

        /// <inheritdoc />
        public void ConfigureModulesServices(IServiceCollection services)
        {
            foreach(var startup in GetStartups())
            {
                startup.ConfigureServices(services);
            }
        }

        /// <inheritdoc />
        public void ConfigureModules(IApplicationBuilder builder)
        {
            foreach (var startup in GetStartups())
            {
                startup.Configure(builder);
            }
        }

        private IEnumerable<IStartup> GetStartups()
        {
            return _enabledModules.SelectMany(moduleName => _moduleLoader.GetLoadedModuleByName(moduleName).Startups);
        }
    }
}
