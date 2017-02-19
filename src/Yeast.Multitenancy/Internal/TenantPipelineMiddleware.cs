using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Yeast.Core.Helpers;

namespace Yeast.Multitenancy.Internal
{
    internal class TenantPipelineMiddleware<TTenant>
        where TTenant : ITenant
    {
        protected readonly ConcurrentDictionary<string, object> buildingLocks = new ConcurrentDictionary<string, object>();

        private readonly RequestDelegate _next;
        private readonly IApplicationBuilder _rootApp;
        private readonly TenantApplicationConfiguration<TTenant> _configuration;

        public TenantPipelineMiddleware(
            RequestDelegate next,
            IApplicationBuilder rootApp,
            TenantApplicationConfiguration<TTenant> configuration)
        {
            Ensure.Argument.NotNull(next, nameof(next));
            Ensure.Argument.NotNull(rootApp, nameof(rootApp));
            Ensure.Argument.NotNull(configuration, nameof(configuration));

            _next = next;
            _rootApp = rootApp;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            Ensure.Argument.NotNull(context, nameof(context));

            var tenantContext = context.GetTenantContext<TTenant>();

            if (tenantContext != null)
            {
                var tenantPipeline = GetTenantPipeline(tenantContext);
                await tenantPipeline.Invoke(context);
            }
            else
            {
                await _next.Invoke(context);
            }
        }

        private RequestDelegate GetTenantPipeline(TenantContext<TTenant> tenantContext)
        {
            if (tenantContext.TenantPipeline == null) {
                // Need to build tenant pipeline
                var buildingLock = buildingLocks.GetOrAdd(tenantContext.Tenant.Identifier, (_) => new object());
                lock (buildingLock)
                {
                    if (tenantContext.TenantPipeline == null)
                    {
                        tenantContext.TenantPipeline = BuildTenantPipeline(tenantContext);
                    }
                }

            }
            return tenantContext.TenantPipeline;
        }

        private RequestDelegate BuildTenantPipeline(TenantContext<TTenant> tenantContext)
        {
            var branchBuilder = _rootApp.New();

            _configuration(branchBuilder, tenantContext);

            return branchBuilder.Build();
        }
    }
}
