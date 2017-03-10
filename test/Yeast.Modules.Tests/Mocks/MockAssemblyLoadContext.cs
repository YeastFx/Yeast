using System;
using System.Reflection;
using System.Runtime.Loader;

namespace Yeast.Modules.Tests.Mocks
{
    class MockAssemblyLoadContext : AssemblyLoadContext
    {
        protected override Assembly Load(AssemblyName assemblyName)
        {
            throw new NotImplementedException();
        }
    }
}
