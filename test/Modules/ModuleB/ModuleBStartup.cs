using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using Yeast.Modules.Abstractions;

namespace ModuleB
{
    public class ModuleBStartup : IStartup
    {
        public void Configure(IApplicationBuilder builder)
        {
            throw new NotImplementedException();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            throw new NotImplementedException();
        }
    }
}
