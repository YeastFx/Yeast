using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Yeast.Multitenancy.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace Yeast.Multitenancy.Tests.Mocks
{
    public class MockMemCachedTenantResolver : MemCachedTenantResolver<MockTenant>
    {
        private readonly Func<MockTenant, IServiceCollection, TenantContext<MockTenant>> _tenantContextFactory;
        private readonly IEnumerable<MockTenant> _tenants;

        public MemoryCacheEntryOptions CacheOptions = new MemoryCacheEntryOptions();

        public MockMemCachedTenantResolver(IEnumerable<TenantServicesConfiguration<MockTenant>> tenantServicesFactories, IMemoryCache cache, IEnumerable<MockTenant> tenants, Func<MockTenant, IServiceCollection, TenantContext<MockTenant>> tenantContextFactory) : base(tenantServicesFactories, cache)
        {
            _tenants = tenants;
            _tenantContextFactory = tenantContextFactory;
        }

        public string CacheKeyFor(MockTenant tenant) {
            return GetCacheKey(tenant.Identifier);
        }

        protected override Task<MockTenant> IdentifyTenantAsync(HttpContext context)
        {
            return Task.Run(() =>
            {
                string tenantName = context.Request.Host.Value;
                return _tenants.SingleOrDefault(t => t.Identifier == tenantName);
            });
        }

        protected override TenantContext<MockTenant> BuildTenantContext(MockTenant tenant, IServiceCollection tenantServices)
        {
            var tenantContext = _tenantContextFactory.Invoke(tenant, tenantServices);
            CacheContext(tenantContext, CacheOptions);
            return tenantContext;
        }
    }
}
