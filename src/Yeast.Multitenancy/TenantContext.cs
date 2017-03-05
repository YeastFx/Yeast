using Microsoft.AspNetCore.Http;
using System;

namespace Yeast.Multitenancy
{
    public class TenantContext : IDisposable
    {
        private readonly IServiceProvider _tenantServices;

        public TenantContext(IServiceProvider tenantServices)
        {
            if (tenantServices == null)
            {
                throw new ArgumentNullException(nameof(tenantServices));
            }

            _tenantServices = tenantServices;
        }

        public IServiceProvider Services {
            get { return _tenantServices; }
        }

        internal RequestDelegate TenantPipeline { get; set; }

        public virtual void Dispose()
        {
            if(_tenantServices is IDisposable)
            {
                (_tenantServices as IDisposable).Dispose();
            }
        }
    }

    public class TenantContext<TTenant> : TenantContext
    {
        private readonly TTenant _tenant;

        public TenantContext(TTenant tenant, IServiceProvider tenantServices)
            : base(tenantServices)
        {
            if(tenant == null)
            {
                throw new ArgumentNullException(nameof(tenant));
            }

            _tenant = tenant;
        }

        public TTenant Tenant {
            get { return _tenant; }
        }
    }
}