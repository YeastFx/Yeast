using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Yeast.Modules.Abstractions
{
    /// <summary>
    /// Interface used to configure modules
    /// </summary>
    public interface IStartup
    {
        /// <summary>
        /// Method used to configure application services
        /// </summary>
        /// <param name="services">The application <see cref="IServiceCollection"/></param>
        void ConfigureServices(IServiceCollection services);

        /// <summary>
        /// Method used to configure the request pipeline
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/> instance</param>
        void Configure(IApplicationBuilder builder);
    }
}
