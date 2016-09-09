using System;
using Yeast.Core.Helpers;

namespace Yeast.Multitenancy
{
    public class TenantContext : IDisposable
    {
        private readonly IServiceProvider _tenantServices;

        public TenantContext(IServiceProvider tenantServices)
        {
            Ensure.Argument.NotNull(tenantServices, nameof(tenantServices));
            _tenantServices = tenantServices;
        }

        public IServiceProvider Services {
            get { return _tenantServices; }
        }

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
            Ensure.Argument.NotNull(tenant, nameof(tenant));

            _tenant = tenant;
        }

        public TTenant Tenant {
            get { return _tenant; }
        }
    }
}