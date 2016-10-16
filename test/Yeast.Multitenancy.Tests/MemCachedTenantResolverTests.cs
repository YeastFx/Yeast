using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using Xunit;
using Yeast.Multitenancy.Tests.Mocks;

namespace Yeast.Multitenancy.Tests
{
    public class MemCachedTenantResolverTests
    {
        [Fact]
        public async void ShouldCacheTenantContext()
        {
            var testTenant = new MockTenant("tenant1");
            TenantContext<MockTenant> resolvedContext, cachedContext;

            using (var cache = new MemoryCache(new MemoryCacheOptions())) {
                var resolver = new MockMemCachedTenantResolver(
                    Enumerable.Empty<TenantServicesFactory<MockTenant>>(),
                    cache,
                    new[] { testTenant },
                    (tenant, tenantServices) => new TenantContext<MockTenant>(tenant, new MockIServiceProvider())
                );
                resolvedContext = await resolver.ResolveAsync(MockHttpContext.WithHostname(testTenant.Identifier));
                cachedContext = cache.Get<TenantContext<MockTenant>>(resolver.CacheKeyFor(testTenant));
            }

            Assert.NotNull(resolvedContext);
            Assert.NotNull(cachedContext);
            Assert.Equal(resolvedContext, cachedContext);
        }

        [Fact]
        public async void ShouldRestoreTenantContextFromCache()
        {
            var testTenant = new MockTenant("tenant1");
            Func<MockTenant, IServiceCollection, TenantContext<MockTenant>> tenantContextFactory = (tenant, tenantServices) => new TenantContext<MockTenant>(tenant, new MockIServiceProvider());
            TenantContext<MockTenant> resolvedContext, cachedContext;

            using (var cache = new MemoryCache(new MemoryCacheOptions()))
            {
                var resolver = new MockMemCachedTenantResolver(
                    Enumerable.Empty<TenantServicesFactory<MockTenant>>(),
                    cache,
                    new[] { testTenant },
                    tenantContextFactory
                );
                cachedContext = tenantContextFactory(testTenant, new ServiceCollection());
                cache.Set(resolver.CacheKeyFor(testTenant), cachedContext);
                resolvedContext = await resolver.ResolveAsync(MockHttpContext.WithHostname(testTenant.Identifier));
            }

            Assert.NotNull(resolvedContext);
            Assert.NotNull(cachedContext);
            Assert.Equal(resolvedContext, cachedContext);
        }

        [Fact]
        public async void CanRemoveTenantContext() {
            var testTenant = new MockTenant("tenant1");
            TenantContext<MockTenant> firstResolvedContext, secondResolvedContext;

            using (var cache = new MemoryCache(new MemoryCacheOptions())) {
                var resolver = new MockMemCachedTenantResolver(
                    Enumerable.Empty<TenantServicesFactory<MockTenant>>(),
                    cache,
                    new[] { testTenant },
                    (tenant, tenantServices) => new TenantContext<MockTenant>(tenant, new MockIServiceProvider())
                );
                firstResolvedContext = await resolver.ResolveAsync(MockHttpContext.WithHostname(testTenant.Identifier));
                cache.Remove(resolver.CacheKeyFor(testTenant));
                secondResolvedContext = await resolver.ResolveAsync(MockHttpContext.WithHostname(testTenant.Identifier));
            }

            Assert.NotNull(firstResolvedContext);
            Assert.NotNull(secondResolvedContext);
            Assert.NotEqual(firstResolvedContext, secondResolvedContext);
        }

        [Fact]
        public async void ShouldDisposeTenantServicesOnEviction() {
            var testTenant = new MockTenant("tenant1");
            TenantContext<MockTenant> resolvedContext;
            var testClock = new MockISystemClock();
            var memCacheOptions = new MemoryCacheOptions()
            {
                Clock = testClock,
                ExpirationScanFrequency = TimeSpan.FromMilliseconds(10)
            };

            using (var cache = new MemoryCache(memCacheOptions))
            {
                var resolver = new MockMemCachedTenantResolver(
                    Enumerable.Empty<TenantServicesFactory<MockTenant>>(),
                    cache,
                    new[] { testTenant },
                    (tenant, tenantServices) => new TenantContext<MockTenant>(tenant, new MockServiceProvider())
                )
                {
                    CacheOptions = new MemoryCacheEntryOptions()
                    {
                        AbsoluteExpiration = testClock.UtcNow.AddMinutes(5)
                    }
                };
                resolvedContext = await resolver.ResolveAsync(MockHttpContext.WithHostname(testTenant.Identifier));
                // Expires the cached context
                testClock.Add(TimeSpan.FromMinutes(6));
                Thread.Sleep(TimeSpan.FromMilliseconds(20));
                // Ensures that context was removed from cache and triggers the cache eviction process
                Assert.Null(cache.Get(resolver.CacheKeyFor(testTenant)));
                Thread.Sleep(TimeSpan.FromMilliseconds(20));
            }

            Assert.NotNull(resolvedContext);
            Assert.NotNull(resolvedContext.Services);
            Assert.True((resolvedContext.Services as MockServiceProvider).IsDisposed);
        }
    }
}
