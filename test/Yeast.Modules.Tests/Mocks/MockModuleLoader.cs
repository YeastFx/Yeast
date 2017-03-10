using System.Runtime.Loader;

namespace Yeast.Modules.Tests.Mocks
{
    class MockModuleLoader : ModuleLoader
    {
        public MockModuleLoader() : base()
        {
            var assemblyLoadContext = new MockAssemblyLoadContext();
        }

        public AssemblyLoadContext AssemblyLoadContext {
            get { return _assemblyLoadContext; }
        }
    }
}
