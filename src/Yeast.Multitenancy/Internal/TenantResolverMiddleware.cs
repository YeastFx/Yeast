using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Yeast.Multitenancy.Internal
{
    internal class TenantResolverMiddleware<TTenant>
        where TTenant : ITenant
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public TenantResolverMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<TenantResolverMiddleware<TTenant>>();
        }

        public async Task Invoke(HttpContext httpContext, ITenantResolver<TTenant> tenantResolver)
        {
            Debug.Assert(tenantResolver != null, $"{nameof(tenantResolver)} must not be null.");

            _logger.LogDebug("Resolving TenantContext using {loggerType}.", tenantResolver.GetType().Name);

            var tenantContext = await tenantResolver.ResolveAsync(httpContext);

            if (tenantContext != null)
            {
                _logger.LogDebug("TenantContext Resolved.");

                httpContext.SetTenantContext(tenantContext);

                using (var scope = tenantContext.CreateServiceScope())
                {
                    httpContext.RequestServices = scope.ServiceProvider;

                    await _next.Invoke(httpContext);
                }
            }
            else
            {
                _logger.LogDebug("TenantContext not resolved.");

                await _next.Invoke(httpContext);
            }
        }
    }
}
