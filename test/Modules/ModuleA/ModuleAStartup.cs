using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using Yeast.Modules.Abstractions;

namespace ModuleA
{
    public class ModuleAStartup : IStartup
    {
        public void Configure(IApplicationBuilder builder, IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            throw new NotImplementedException();
        }
    }
}
