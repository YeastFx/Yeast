using System;
using System.Collections.Generic;
using System.Linq;

namespace Yeast.Modules.Tests.Mocks
{
    public class MockIModuleLoader : IModuleLoader
    {
        private readonly HashSet<ILoadedModule> _loadedModules;

        public MockIModuleLoader(params ILoadedModule[] loadedModules)
        {
            _loadedModules = new HashSet<ILoadedModule>(loadedModules);
        }

        public ILoadedModule GetLoadedModuleByName(string moduleName)
        {
            return _loadedModules.FirstOrDefault(loadedModule => loadedModule.Name == moduleName);
        }

        public void LoadModule(string moduleDirectory)
        {
            throw new NotImplementedException();
        }

        public void LoadModules(string modulesDirectory)
        {
            throw new NotImplementedException();
        }
    }
}
