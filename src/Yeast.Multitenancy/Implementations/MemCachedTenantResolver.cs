using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Yeast.Multitenancy.Implementations
{
    public abstract class MemCachedTenantResolver<TTenant> : TenantResolver<TTenant>
        where TTenant : ITenant
    {
        protected const string CachePrefix = "TenantCtx:";
        protected readonly IMemoryCache _cache;

        public MemCachedTenantResolver(IEnumerable<TenantServicesConfiguration<TTenant>> tenantServicesFactories, IMemoryCache cache) :base(tenantServicesFactories)
        {
            if(cache == null)
            {
                throw new InvalidOperationException("AddMemoryCache must be called on the service collection.");
            }

            _cache = cache;
        }

        protected virtual string GetCacheKey(string tenantIdentifier)
        {
            Debug.Assert(!string.IsNullOrEmpty(tenantIdentifier), $"{nameof(tenantIdentifier)} must not be null or empty");

            return CachePrefix + tenantIdentifier;
        }

        /// <summary>
        /// Tries retrieving tenantContext from MemoryCache
        /// </summary>
        /// <param name="tenant">The resolved <see cref="TTenant"/> instance</param>
        /// <param name="tenantContext">Cached <see cref="TenantContext{TTenant}"/> if it was found, otherwise null</param>
        /// <returns>true if a <see cref="TenantContext{TTenant}"/> was found, otherwise false</returns>
        protected override bool TryGetTenantContext(TTenant tenant, out TenantContext<TTenant> tenantContext)
        {
            Debug.Assert(tenant != null, $"{nameof(tenant)} must not be null");

            return _cache.TryGetValue(GetCacheKey(tenant.Identifier), out tenantContext);
        }

        /// <summary>
        /// Stores a <see cref="TenantContext{TTenant}"/> in MemoryCache
        /// </summary>
        /// <param name="tenantContext">The <see cref="TenantContext{TTenant}"/> to store</param>
        /// <param name="cacheOptions">MemoryCache options</param>
        /// <remarks>Should be called by your implementation of <see cref="TenantResolver{TTenant}.BuildTenantContext(TTenant)"/></remarks>
        protected void CacheContext(TenantContext<TTenant> tenantContext, MemoryCacheEntryOptions cacheOptions)
        {
            Debug.Assert(tenantContext != null, $"{nameof(tenantContext)} must not be null");

            cacheOptions.RegisterPostEvictionCallback(
                (key, value, reason, state) =>
                {
                    DisposeTenantContext(value as TenantContext<TTenant>);
                }
            );

            _cache.Set(GetCacheKey(tenantContext.Tenant.Identifier), tenantContext, cacheOptions);
        }

        /// <summary>
        /// Removes a <see cref="TenantContext{TTenant}"/> from MemoryCache
        /// </summary>
        /// <param name="tenant">The <see cref="TTenant"/> to remove</param>
        protected virtual void RemoveContext(TTenant tenant)
        {
            Debug.Assert(tenant != null, $"{nameof(tenant)} must not be null");

            RemoveContext(tenant.Identifier);
        }

        /// <summary>
        /// Removes a <see cref="TenantContext{TTenant}"/> from MemoryCache
        /// </summary>
        /// <param name="tenantIdentifier">Identifier of the <see cref="TTenant"/> to remove</param>
        protected virtual void RemoveContext(string tenantIdentifier)
        {
            if(string.IsNullOrEmpty(tenantIdentifier))
            {
                throw new InvalidOperationException($"{nameof(tenantIdentifier)} cannot be null or empty.");
            }

            _logger.LogInformation("Removing context for \"{identifier}\".", tenantIdentifier);
            _cache.Remove(GetCacheKey(tenantIdentifier));
        }

        protected virtual void DisposeTenantContext(TenantContext<TTenant> tenantContext)
        {
            if(tenantContext == null)
            {
                throw new InvalidOperationException($"{nameof(tenantContext)} cannot be null.");
            }

            _logger.LogInformation("Disposing context for \"{identifier}\".", tenantContext.Tenant.Identifier);
            tenantContext.Dispose();
        }
    }
}
