using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Yeast.Multitenancy.Tests.Mocks;

namespace Yeast.Multitenancy.Tests
{
    public class TenantResolverMiddlewareTests
    {
        [Fact]
        public async void ShouldReplaceRequestServicesWhenTenantResolved()
        {
            using (var server = CreateTestServer(
                (appServices) => appServices.AddSingleton(new MockStatefullService() { State = "appRoot" }),
                new MockITenantResolver(
                    new[] { new MockTenant("tenant1"), new MockTenant("tenant2") },
                    (tenant) =>
                        new TenantContext<MockTenant>(tenant,
                            new ServiceCollection()
                                .AddSingleton(new MockStatefullService() { State = tenant.Identifier})
                                .BuildServiceProvider()
                        )
                )
            ))
            {
                var client = server.CreateClient();
                Assert.Equal("tenant1", await client.GetStringAsync("http://tenant1/"));
                Assert.Equal("tenant2", await client.GetStringAsync("http://tenant2/"));
                Assert.Equal("appRoot", await client.GetStringAsync("http://foo/"));
            }
        }

        [Fact]
        public async void ShouldCreateServiceScopePerRequest()
        {
            using (var server = CreateTestServer(
                (appServices) => appServices.AddSingleton(new MockStatefullService()),
                new MockITenantResolver(
                    new[] { new MockTenant("tenant1"), new MockTenant("tenant2") },
                    (tenant) =>
                        new TenantContext<MockTenant>(tenant,
                            new ServiceCollection()
                                .AddScoped((_) => new MockStatefullService() { State = tenant.Identifier })
                                .BuildServiceProvider()
                        )
                )
            ))
            {
                var client = server.CreateClient();
                var resonse = await client.PostAsync("http://tenant1/", new StringContent("foo"));
                Assert.Equal("foo", await resonse.Content.ReadAsStringAsync());
                Assert.Equal("tenant1", await client.GetStringAsync("http://tenant1/"));
            }
        }

        #region Helper functions
        private static TestServer CreateTestServer(Action<IServiceCollection> appServices, ITenantResolver<MockTenant> tenantResolver)
        {
            return new TestServer(
                new WebHostBuilder()
                    .ConfigureServices(
                        services =>
                        {
                            appServices.Invoke(services);
                            services.AddMultitenancy(tenantResolver);
                        })
                    .Configure(
                        app =>
                        {
                            app.UseMultitenancy<MockTenant>();
                            app.Run(
                                async ctx =>
                                {
                                    var service = ctx.RequestServices.GetService<MockStatefullService>();
                                    switch (ctx.Request.Method)
                                    {
                                        case "GET":
                                            await ctx.Response.WriteAsync(service.State);
                                            break;
                                        case "POST":
                                            using (var reader = new StreamReader(ctx.Request.Body))
                                            {
                                                service.State = await reader.ReadToEndAsync();
                                            }
                                            await ctx.Response.WriteAsync(service.State);
                                            break;
                                    }
                                }
                            );
                        })
            );
        }

        #endregion
    }
}
