using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
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
                Enumerable.Empty<TenantServicesConfiguration<MockTenant>>(),
                new[] { new MockTenant("tenant1"), new MockTenant("tenant2") },
                (tenant, tenantServices) => new TenantContext<MockTenant>(tenant, new MockIServiceProvider())
            );
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await resolver.ResolveAsync(null));
        }

        [Fact]
        public async void ShouldReturnNullTenantContextIfUnresolved()
        {
            var resolver = new MockTenantResolver(
                Enumerable.Empty<TenantServicesConfiguration<MockTenant>>(),
                new[] { new MockTenant("tenant1") },
                (tenant, tenantServices) => new TenantContext<MockTenant>(tenant, new MockIServiceProvider())
            );

            Assert.NotNull(await resolver.ResolveAsync(MockHttpContext.WithHostname("tenant1")));
            Assert.Null(await resolver.ResolveAsync(MockHttpContext.WithHostname("foo")));
        }

        [Fact]
        public void ShouldNotBuildSameTenantContextConcurrently()
        {
            // Arrange
            var buildCount = 0;
            var resolver = new MockTenantResolver(
                Enumerable.Empty<TenantServicesConfiguration<MockTenant>>(),
                new[] { new MockTenant("tenant1") },
                (tenant, tenantServices) =>
                {
                    buildCount++;
                    return new TenantContext<MockTenant>(tenant, new MockIServiceProvider());
                }
            );

            // Act
            Parallel.For(0, 10, (idx) => resolver.ResolveAsync(MockHttpContext.WithHostname("tenant1")).Wait());

            // Assert
            Assert.Equal(1, buildCount);
        }

        [Fact]
        public async void ShouldAddTenantAsSignleton()
        {
            IServiceCollection providedServices = null;
            var resolver = new MockTenantResolver(
                Enumerable.Empty<TenantServicesConfiguration<MockTenant>>(),
                new[] { new MockTenant("tenant1") },
                (tenant, tenantServices) =>
                {
                    providedServices = tenantServices;
                    return new TenantContext<MockTenant>(tenant, new MockIServiceProvider());
                }
            );
            var tenantCtx = await resolver.ResolveAsync(MockHttpContext.WithHostname("tenant1"));
            Assert.NotNull(providedServices);
            Assert.Contains(providedServices, sd => sd.ServiceType == typeof(MockTenant) && sd.Lifetime == ServiceLifetime.Singleton);
        }
    }
}
