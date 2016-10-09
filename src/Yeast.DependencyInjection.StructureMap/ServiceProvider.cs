using Microsoft.Extensions.DependencyInjection;
using StructureMap;
using System;

namespace Yeast.DependencyInjection.StructureMap
{
    public class ServiceProvider : IServiceProvider, ISupportRequiredService
    {
        private readonly IContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceProvider"/> class.
        /// </summary>
        /// <param name="IContainer">The <see cref="IContainer"/> from which services should be resolved.</param>
        public ServiceProvider(IContainer container)
        {
            _container = container;
        }

        /// <summary>
        /// Gets service of type <paramref name="serviceType"/> from the <see cref="IServiceProvider"/> implementing
        /// this interface.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>A service object of type <paramref name="serviceType"/>.
        /// Throws an exception if the <see cref="IServiceProvider"/> cannot create the object.</returns>
        public object GetRequiredService(Type serviceType)
        {
            // From StructureMap.Microsoft.DependencyInjection
            /*if (serviceType.IsGenericEnumerable())
            {
                return GetRequiredService(serviceType);
            }*/
            return _container.TryGetInstance(serviceType);
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">Type of the requested service</param>
        /// <returns>A service object of type <paramref name="serviceType" /> or null</returns>
        public object GetService(Type serviceType)
        {
            return _container.GetInstance(serviceType);
        }
    }
}