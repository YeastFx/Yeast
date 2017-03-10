using System;
using System.Reflection;
using Yeast.Modules.Abstractions;

namespace Yeast.Modules
{
    public class LoadedModule
    {
        private readonly string _modulePath;
        private readonly Assembly _assembly;
        private readonly ModuleInfo _infos;

        internal LoadedModule(string modulePath, Assembly assembly)
        {
            _modulePath = modulePath;
            _assembly = assembly;

            var moduleInfoType = _assembly.ExportedTypes.FindModuleInfo();

            if(moduleInfoType != null)
            {
                _infos = (ModuleInfo)Activator.CreateInstance(moduleInfoType);
            }
            else
            {
                throw new InvalidOperationException($"Unable to find a {nameof(ModuleInfo)} implementation in module exported types.");
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
    }
}
