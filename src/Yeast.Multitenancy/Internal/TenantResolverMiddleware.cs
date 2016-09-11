using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Yeast.Core.Helpers;

namespace Yeast.Multitenancy.Internal
{
    public class TenantResolverMiddleware<TTenant>
        where TTenant : ITenant
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public TenantResolverMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            Ensure.Argument.NotNull(next, nameof(next));
            Ensure.Argument.NotNull(loggerFactory, nameof(loggerFactory));

            _next = next;
            _logger = loggerFactory.CreateLogger<TenantResolverMiddleware<TTenant>>();
        }

        public async Task Invoke(HttpContext httpContext, ITenantResolver<TTenant> tenantResolver)
        {
            Ensure.Argument.NotNull(httpContext, nameof(httpContext));
            Ensure.Argument.NotNull(tenantResolver, nameof(tenantResolver));

            _logger.LogDebug("Resolving TenantContext using {loggerType}.", tenantResolver.GetType().Name);

            var tenantContext = await tenantResolver.ResolveAsync(httpContext);

            if (tenantContext != null)
            {
                _logger.LogDebug("TenantContext Resolved.");
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
