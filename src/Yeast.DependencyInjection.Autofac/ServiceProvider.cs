using Autofac;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Yeast.DependencyInjection.Autofac
{
    public class ServiceProvider : IServiceProvider, ISupportRequiredService
    {
        private readonly IComponentContext _componentContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceProvider"/> class.
        /// </summary>
        /// <param name="componentContext">The <see cref="IComponentContext"/> from which services should be resolved.</param>
        public ServiceProvider(IComponentContext componentContext)
        {
            _componentContext = componentContext;
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
            return _componentContext.Resolve(serviceType);
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">Type of the requested service</param>
        /// <returns>A service object of type <paramref name="serviceType" /> or null</returns>
        public object GetService(Type serviceType)
        {
            return _componentContext.ResolveOptional(serviceType);
        }
    }
}