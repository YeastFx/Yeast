using System;

namespace Yeast.Multitenancy.Tests.Mocks
{
    public class MockServiceProvider : IServiceProvider, IDisposable
    {
        private bool disposed = false;

        public bool IsDisposed { get { return disposed; } }

        public object GetService(Type serviceType)
        {
            return null;
        }

        public void Dispose()
        {
            disposed = true;
        }
    }
}
