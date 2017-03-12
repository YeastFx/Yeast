using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Yeast.Modules.Abstractions;

namespace Yeast.Modules
{
    public class LoadedModule : ILoadedModule
    {
        private readonly string _modulePath;
        private readonly Assembly _assembly;
        private readonly ModuleInfo _infos;
        private readonly Lazy<IEnumerable<IStartup>> _stratups;

        internal LoadedModule(string modulePath, Assembly assembly)
        {
            if (string.IsNullOrEmpty(modulePath))
            {
                throw new ArgumentException("Cannot be null or empty", nameof(modulePath));
            }

            _modulePath = modulePath;

            _assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));

            var moduleInfoType = _assembly.ExportedTypes.FindModuleInfo();

            if(moduleInfoType != null)
            {
                _infos = (ModuleInfo)Activator.CreateInstance(moduleInfoType);
            }
            else
            {
                throw new InvalidOperationException($"Unable to find a {nameof(ModuleInfo)} implementation in module exported types.");
            }

            _stratups = new Lazy<IEnumerable<IStartup>>(GetStartups);
        }

        public string Path {
            get { return _modulePath; }
        }

        public string Name {
            get { return _infos.Name; }
        }

        public ModuleInfo Infos {
            get { return _infos; }
        }

        public Assembly Assembly {
            get { return _assembly; }
        }

        public IEnumerable<IStartup> Startups {
            get { return _stratups.Value; }
        }

        private IEnumerable<IStartup> GetStartups()
        {
            foreach (var startupType in Assembly.ExportedTypes.Where(type => {
                var typeInfo = type.GetTypeInfo();
                return typeInfo.IsClass && !typeInfo.IsAbstract && !typeInfo.IsGenericType && typeof(IStartup).IsAssignableFrom(type);
            }))
            {
                yield return (IStartup)Activator.CreateInstance(startupType);
            }
        }
    }
}
