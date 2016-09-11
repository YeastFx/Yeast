using System;

namespace Yeast.Multitenancy.Tests.Mocks
{
    public class MockIServiceProvider : IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }
    }
}
