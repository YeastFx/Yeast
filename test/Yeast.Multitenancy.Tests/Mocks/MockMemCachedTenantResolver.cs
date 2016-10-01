using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Yeast.Multitenancy.Implementations;

namespace Yeast.Multitenancy.Tests.Mocks
{
    public class MockMemCachedTenantResolver : MemCachedTenantResolver<MockTenant>
    {
        private readonly Func<MockTenant, TenantContext<MockTenant>> _tenantContextFactory;
        private readonly IEnumerable<MockTenant> _tenants;

        public MemoryCacheEntryOptions CacheOptions = new MemoryCacheEntryOptions();

        public MockMemCachedTenantResolver(IMemoryCache cache, IEnumerable<MockTenant> tenants, Func<MockTenant, TenantContext<MockTenant>> tenantContextFactory) : base(cache)
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

        protected override TenantContext<MockTenant> BuildTenantContext(MockTenant tenant)
        {
            var tenantContext = _tenantContextFactory.Invoke(tenant);
            CacheContext(tenantContext, CacheOptions);
            return tenantContext;
        }
    }
}
