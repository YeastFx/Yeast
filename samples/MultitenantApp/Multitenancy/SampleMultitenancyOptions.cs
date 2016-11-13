using System.Collections.ObjectModel;

namespace MultitenantApp.Multitenancy
{
    public class SampleMultitenancyOptions
    {
        public Collection<SampleTenant> Tenants { get; set; }
    }
}
