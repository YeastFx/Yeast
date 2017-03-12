using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using Yeast.Modules.Abstractions;

namespace Yeast.Modules.Tests.Mocks
{
    public class MockStartup : IStartup
    {
        private int _configureCallsCount = 0;
        private int _configureServicesCallsCount = 0;

        public void Configure(IApplicationBuilder builder)
        {
            _configureCallsCount++;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            _configureServicesCallsCount++;
        }

        public int ConfigureCallsCount => _configureCallsCount;

        public int ConfigureServicesCallsCount => _configureServicesCallsCount;
    }
}
