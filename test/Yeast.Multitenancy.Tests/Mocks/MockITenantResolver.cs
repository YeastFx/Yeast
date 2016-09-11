using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yeast.Multitenancy.Tests.Mocks
{
    public class MockITenantResolver : ITenantResolver<MockTenant>
    {
        private readonly Func<MockTenant, TenantContext<MockTenant>> _tenantContextFactory;
        private readonly IEnumerable<MockTenant> _tenants;
        private readonly IDictionary<string, TenantContext<MockTenant>> _tenantContexts = new Dictionary<string, TenantContext<MockTenant>>();

        public MockITenantResolver(IEnumerable<MockTenant> tenants, Func<MockTenant, TenantContext<MockTenant>> tenantContextFactory) {
            _tenants = tenants;
            _tenantContextFactory = tenantContextFactory;
        }

        public Task<TenantContext<MockTenant>> ResolveAsync(HttpContext context)
        {
            return Task.Run(() =>
            {
                string tenantName = context.Request.Host.Value;
                if(_tenantContexts.ContainsKey(tenantName))
                {
                    return _tenantContexts[tenantName];
                }
                else {
                    var tenant = _tenants.SingleOrDefault(t => t.Identifier == tenantName);
                    if(tenant != null)
                    {
                        var ctx = _tenantContextFactory.Invoke(tenant);
                        _tenantContexts.Add(tenantName, ctx);
                        return ctx;
                    }
                    else
                    {
                        return null;
                    }
                }
            });
        }
    }
}
