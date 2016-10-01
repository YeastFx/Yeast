using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Xunit;
using Yeast.Multitenancy.Tests.Mocks;

namespace Yeast.Multitenancy.Tests
{
    public class TenantResolverTests
    {
        [Fact]
        public async void ShouldThrowOnNullHttpContext()
        {
            var resolver = new MockTenantResolver(
                new[] { new MockTenant("tenant1"), new MockTenant("tenant2") },
                (tenant) => new TenantContext<MockTenant>(tenant, new MockIServiceProvider())
            );
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await resolver.ResolveAsync(null));
        }

        [Fact]
        public async void ShouldReturnNullTenantContextIfUnresolved()
        {
            var resolver = new MockTenantResolver(
                new[] { new MockTenant("tenant1") },
                (tenant) => new TenantContext<MockTenant>(tenant, new MockIServiceProvider())
            );

            Assert.NotNull(await resolver.ResolveAsync(MockHttpContext.WithHostname("tenant1")));
            Assert.Null(await resolver.ResolveAsync(MockHttpContext.WithHostname("foo")));
        }

        [Fact]
        public void ShouldNotBuildSameTenantContextConcurrently()
        {
            var buildCount = 0;
            var resolver = new MockTenantResolver(
                new[] { new MockTenant("tenant1") },
                (tenant) =>
                {
                    buildCount++;
                    return new TenantContext<MockTenant>(tenant, new MockIServiceProvider());
                }
            );
            Parallel.For(0, 10, async (idx) => await resolver.ResolveAsync(MockHttpContext.WithHostname("tenant1")));
            Assert.Equal(1, buildCount);
        }
    }
}
