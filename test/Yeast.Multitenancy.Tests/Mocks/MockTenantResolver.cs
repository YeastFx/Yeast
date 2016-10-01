using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yeast.Multitenancy.Implementations;

namespace Yeast.Multitenancy.Tests.Mocks
{
    public class MockTenantResolver : TenantResolver<MockTenant>
    {
        private readonly Func<MockTenant, TenantContext<MockTenant>> _tenantContextFactory;
        private readonly IEnumerable<MockTenant> _tenants;
        private readonly IDictionary<string, TenantContext<MockTenant>> _tenantContexts = new Dictionary<string, TenantContext<MockTenant>>();

        public MockTenantResolver(IEnumerable<MockTenant> tenants, Func<MockTenant, TenantContext<MockTenant>> tenantContextFactory)
        {
            _tenants = tenants;
            _tenantContextFactory = tenantContextFactory;
        }

        protected override Task<MockTenant> IdentifyTenantAsync(HttpContext context)
        {
            return Task.Run(() =>
            {
                string tenantName = context.Request.Host.Value;
                return _tenants.SingleOrDefault(t => t.Identifier == tenantName);
            });
        }

        protected override bool TryGetTenantContext(MockTenant tenant, out TenantContext<MockTenant> tenantContext)
        {
            return _tenantContexts.TryGetValue(tenant.Identifier, out tenantContext);
        }

        protected override TenantContext<MockTenant> BuildTenantContext(MockTenant tenant)
        {
            var tenantContext = _tenantContextFactory.Invoke(tenant);
            _tenantContexts[tenant.Identifier] = tenantContext;
            return tenantContext;
        }
    }
}
