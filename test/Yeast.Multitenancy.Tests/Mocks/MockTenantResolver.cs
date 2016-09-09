using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yeast.Multitenancy.Tests.Mocks
{
    public class MockTenantResolver : ITenantResolver<string>
    {
        private readonly Func<string, TenantContext<string>> _tenantContextFactory;
        private readonly IEnumerable<string> _tenantNames;
        private readonly IDictionary<string, TenantContext<string>> _tenantContexts;

        public MockTenantResolver(IEnumerable<string> tenantNames, Func<string, TenantContext<string>> tenantContextFactory) {
            _tenantNames = tenantNames;
            _tenantContextFactory = tenantContextFactory;
            _tenantContexts = new Dictionary<string, TenantContext<string>>();
        }

        public Task<TenantContext<string>> ResolveAsync(HttpContext context)
        {
            return Task.Run(
                () =>
                {
                    string tenantName;
                    var path = context.Request.Path.Value.TrimStart('/');
                    var slashIndex = path.IndexOf('/');
                    if(slashIndex > 0)
                    {
                        tenantName = path.Substring(slashIndex);
                    }
                    else
                    {
                        tenantName = path;
                    }
                    if(_tenantContexts.ContainsKey(tenantName))
                    {
                        return _tenantContexts[tenantName];
                    }
                    else if (_tenantNames.Contains(tenantName)) {
                        var ctx = _tenantContextFactory.Invoke(tenantName);
                        _tenantContexts.Add(tenantName, ctx);
                        return ctx;
                    }
                    else
                    {
                        return null;
                    }
                }
            );
        }
    }
}
