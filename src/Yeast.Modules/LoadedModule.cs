using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Yeast.Features.Abstractions;
using Yeast.Modules.Abstractions;

namespace Yeast.Modules
{
    public class LoadedModule : ILoadedModule
    {
        private readonly string _modulePath;
        private readonly Assembly _assembly;
        private readonly ModuleInfo _infos;
        private readonly HashSet<FeatureInfo> _features;

        internal LoadedModule(string modulePath, Assembly assembly)
        {
            if (string.IsNullOrEmpty(modulePath))
            {
                throw new ArgumentException("Cannot be null or empty", nameof(modulePath));
            }

            _modulePath = modulePath;

            _assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));

            _features = new HashSet<FeatureInfo>(GetAvailableFeatures(_assembly));

            try
            {
                _infos = (ModuleInfo)_features.Single(feature => typeof(ModuleInfo).IsAssignableFrom(feature.GetType()));
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException($"A module must export one and only one {nameof(ModuleInfo)} implementation.");
            }
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

        public IEnumerable<FeatureInfo> Features {
            get { return _features; }
        }

        private static IEnumerable<FeatureInfo> GetAvailableFeatures(Assembly assembly)
        {
            foreach (var startupType in assembly.ExportedTypes.Where(type => {
                var typeInfo = type.GetTypeInfo();
                return typeInfo.IsClass && !typeInfo.IsAbstract && !typeInfo.IsGenericType && typeof(FeatureInfo).IsAssignableFrom(type);
            }))
            {
                yield return (FeatureInfo)Activator.CreateInstance(startupType);
            }
        }
    }
}
