using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Yeast.Multitenancy
{
    /// <summary>
    /// Provides abstraction for tenant resolution logic
    /// </summary>
    /// <typeparam name="TTenant">Type of resolved tenant</typeparam>
    /// <remarks>Used by TenantResolverMiddleware</remarks>
    public interface ITenantResolver<TTenant>
        where TTenant : ITenant
    {
        /// <summary>
        /// Resolves current tenant
        /// </summary>
        /// <param name="context">Evaluated HttpContext</param>
        /// <returns>Resolved TenantContext</returns>
        Task<TenantContext<TTenant>> ResolveAsync(HttpContext context);
    }
}
