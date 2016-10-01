using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Yeast.Core.Helpers;

namespace Yeast.Multitenancy.Implementations
{
    public abstract class MemCachedTenantResolver<TTenant> : TenantResolver<TTenant>
        where TTenant : ITenant
    {
        protected const string CachePrefix = "TenantCtx:";
        protected readonly IMemoryCache _cache;

        public MemCachedTenantResolver(IMemoryCache cache)
        {
            Ensure.Argument.NotNull(cache, nameof(cache));

            _cache = cache;
        }

        protected virtual string GetCacheKey(string tenantIdentifier)
        {
            Ensure.Argument.NotNullOrEmpty(tenantIdentifier, nameof(tenantIdentifier));

            return CachePrefix + tenantIdentifier;
        }

        /// <summary>
        /// Tries retrieving tenantContext from MemoryCache
        /// </summary>
        /// <param name="tenant">The resolved <see cref="TTenant"/> instance</param>
        /// <param name="tenantContext">Cached <see cref="TenantContext{TTenant}"/> if it was found, otherwise null</param>
        /// <returns>true if a <see cref="TenantContext{TTenant}"/> was found, otherwise false</returns>
        protected override bool TryGetTenantContext(TTenant tenant, out TenantContext<TTenant> tenantContext) {
            Ensure.Argument.NotNull(tenant, nameof(tenant));

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
            Ensure.Argument.NotNull(tenantContext, nameof(tenantContext));
            Ensure.Argument.NotNull(cacheOptions, nameof(cacheOptions));

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
            Ensure.Argument.NotNull(tenant, nameof(tenant));

            RemoveContext(tenant.Identifier);
        }

        /// <summary>
        /// Removes a <see cref="TenantContext{TTenant}"/> from MemoryCache
        /// </summary>
        /// <param name="tenantIdentifier">Identifier of the <see cref="TTenant"/> to remove</param>
        protected virtual void RemoveContext(string tenantIdentifier)
        {
            Ensure.Argument.NotNullOrEmpty(tenantIdentifier, nameof(tenantIdentifier));

            _logger.LogInformation("Removing context for \"{identifier}\".", tenantIdentifier);
            _cache.Remove(GetCacheKey(tenantIdentifier));
        }

        protected virtual void DisposeTenantContext(TenantContext<TTenant> tenantContext)
        {
            Ensure.Argument.NotNull(tenantContext, nameof(tenantContext));

            _logger.LogInformation("Disposing context for \"{identifier}\".", tenantContext.Tenant.Identifier);
            tenantContext.Dispose();
        }
    }
}
