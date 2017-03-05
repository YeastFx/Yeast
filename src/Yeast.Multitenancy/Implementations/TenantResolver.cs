using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yeast.Multitenancy.Implementations
{
    /// <summary>
    /// Base implementation of <see cref="ITenantResolver{TTenant}"/>
    /// </summary>
    /// <typeparam name="TTenant">Type of tenant</typeparam>
    public abstract class TenantResolver<TTenant> : ITenantResolver<TTenant>
        where TTenant : ITenant
    {
        protected readonly ConcurrentDictionary<string, object> buildingLocks = new ConcurrentDictionary<string, object>();
        protected readonly IEnumerable<TenantServicesConfiguration<TTenant>> _tenantServicesFactories;

        protected ILogger _logger = NullLogger.Instance;

        public TenantResolver(IEnumerable<TenantServicesConfiguration<TTenant>> tenantServicesFactories)
        {
            _tenantServicesFactories = tenantServicesFactories;
        }

        /// <summary>
        /// Identifies the tenant based on the <paramref name="context"/>
        /// </summary>
        /// <param name="context">Evaluated <see cref="HttpContext"/></param>
        /// <returns>Identified tenant or null</returns>
        protected abstract Task<TTenant> IdentifyTenantAsync(HttpContext context);

        /// <summary>
        /// Tries retrieving an already built <see cref="TenantContext{TTenant}"/>
        /// </summary>
        /// <param name="tenant">The resolved <see cref="TTenant"/> instance</param>
        /// <param name="tenantContext">Existing <see cref="TenantContext{TTenant}"/> if it was found, otherwise null</param>
        /// <returns>true if a <see cref="TenantContext{TTenant}"/> was found, otherwise false</returns>
        protected abstract bool TryGetTenantContext(TTenant tenant, out TenantContext<TTenant> tenantContext);

        /// <summary>
        /// Builds a new <see cref="TenantContext{TTenant}"/>
        /// </summary>
        /// <param name="tenant">The instance of <see cref="TTenant"/></param>
        /// <param name="tenantServices">Tenant's services collection</param>
        /// <returns>Newly built <see cref="TenantContext{TTenant}"/></returns>
        /// <remarks>Your implementation have to keep the built context reference</remarks>
        protected abstract TenantContext<TTenant> BuildTenantContext(TTenant tenant, IServiceCollection tenantServices);

        /// <summary>
        /// Resolves current tenant
        /// </summary>
        /// <param name="context">Evaluated HttpContext</param>
        /// <returns>Resolved TenantContext</returns>
        public async Task<TenantContext<TTenant>> ResolveAsync(HttpContext context)
        {
            if(context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var tenant = await IdentifyTenantAsync(context);

            if (tenant != null)
            {
                _logger.LogDebug("Tenant \"{identifier}\" identified.", tenant.Identifier);
                TenantContext<TTenant> tenantContext;

                if (TryGetTenantContext(tenant, out tenantContext))
                {
                    // Already built tenant context
                    _logger.LogDebug("Tenant context for \"{identifier}\" restored.", tenant.Identifier);
                    return tenantContext;
                }
                else
                {
                    // Need to build new tenant context
                    var buildingLock = buildingLocks.GetOrAdd(tenant.Identifier, (_) => new object());
                    lock (buildingLock)
                    {
                        if (TryGetTenantContext(tenant, out tenantContext))
                        {
                            _logger.LogDebug("Tenant context for \"{identifier}\" restored.", tenant.Identifier);
                            return tenantContext;
                        }
                        else
                        {
                            tenantContext = BuildTenantContext(tenant, GetTenantServices(tenant));
                            
                            if(tenantContext == null)
                            {
                                throw new InvalidOperationException("Tenant context build failed.");
                            }

                            _logger.LogInformation("Tenant context for \"{identifier}\" created.", tenant.Identifier);
                        }
                    }

                    return tenantContext;
                }
            }
            else
            {
                _logger.LogDebug("Unidentified tenant.");
                return null;
            }
        }

        protected virtual IServiceCollection GetTenantServices(TTenant tenant) {
            var tenantServices = new ServiceCollection();
            tenantServices.AddSingleton(typeof(TTenant), tenant);
            foreach (var provider in _tenantServicesFactories) {
                provider(tenant, tenantServices);
            }
            return tenantServices;
        }
    }
}
