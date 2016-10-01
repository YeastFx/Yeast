using System;
using Xunit;
using Yeast.Multitenancy.Tests.Mocks;

namespace Yeast.Multitenancy.Tests
{
    public class TenantContextTests
    {
        [Fact]
        public void ShouldThrowOnNullIServiceProvider()
        {
            Assert.Throws<ArgumentNullException>(() => new TenantContext(null));
        }

        [Fact]
        public void ShouldDisposeTenantServices()
        {
            var stubServiceProvider = new MockServiceProvider();
            using (var tenantContext = new TenantContext(stubServiceProvider)) { }
            Assert.True(stubServiceProvider.IsDisposed);
        }

    }
}
