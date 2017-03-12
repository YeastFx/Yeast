using System;
using System.Collections.Generic;
using System.Reflection;
using Yeast.Modules.Abstractions;

namespace Yeast.Modules.Tests.Mocks
{
    class MockILoadedModule : ILoadedModule
    {
        private readonly string _moduleName;
        public MockILoadedModule(string moduleName)
        {
            _moduleName = moduleName;
        }

        public Assembly Assembly => typeof(MockIModuleLoader).GetTypeInfo().Assembly;

        public ModuleInfo Infos => throw new NotImplementedException();

        public string Name => _moduleName;

        public string Path => throw new NotImplementedException();

        public IEnumerable<IStartup> Startups { get; set; }
    }
}
