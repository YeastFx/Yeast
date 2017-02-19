using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Xunit;
using Yeast.Multitenancy.Tests.Mocks;

namespace Yeast.Multitenancy.Tests
{
    public class TenantPipelineMiddlewareTests
    {
        [Fact]
        public async void ShouldForkPipelineIfTenantResolved()
        {
            using (var server = CreateTestServer(
                new MockITenantResolver(
                    new[] { new MockTenant("tenant1"), new MockTenant("tenant2") },
                    (tenant) =>
                        new TenantContext<MockTenant>(tenant,
                            new ServiceCollection()
                                .AddSingleton(new MockStatefullService() { State = tenant.Identifier })
                                .BuildServiceProvider()
                        )
                )
            ))
            {
                var client = server.CreateClient();
                Assert.Equal("tenant1", await client.GetStringAsync("http://tenant1/"));
                Assert.Equal("tenant2", await client.GetStringAsync("http://tenant2/"));
            }
        }

        [Fact]
        public async void ShouldNotForkPipelineIfNoTenantResolved()
        {
            using (var server = CreateTestServer(
                new MockITenantResolver(
                    new[] { new MockTenant("tenant1"), new MockTenant("tenant2") },
                    (tenant) =>
                        new TenantContext<MockTenant>(tenant,
                            new ServiceCollection()
                                .AddSingleton(new MockStatefullService() { State = tenant.Identifier })
                                .BuildServiceProvider()
                        )
                )
            ))
            {
                var client = server.CreateClient();
                Assert.Equal("Default", await client.GetStringAsync("http://foo/"));
            }
        }

        [Fact]
        public async void ShouldConfigurePipelineOncePerTenant()
        {
            var counters = new Dictionary<string, int>()
            {
                { "tenant1", 0 },
                { "tenant2", 0 }
            };
            using (var server = CreateTestServer(
                new MockITenantResolver(
                    new[] { new MockTenant("tenant1"), new MockTenant("tenant2") },
                    (tenant) =>
                        new TenantContext<MockTenant>(tenant,
                            new ServiceCollection()
                                .AddSingleton(new MockStatefullService() { State = tenant.Identifier })
                                .BuildServiceProvider()
                        )
                ),
                (tenantApp, tenantCtx) => {
                    counters[tenantCtx.Tenant.Identifier]++;
                }
            ))
            {
                var client = server.CreateClient();
                await client.GetStringAsync("http://tenant1/");
                await client.GetStringAsync("http://tenant2/");
                await client.GetStringAsync("http://tenant1/");
                await client.GetStringAsync("http://tenant2/");
            }
            Assert.Equal(1, counters["tenant1"]);
            Assert.Equal(1, counters["tenant2"]);
        }

        [Fact]
        public async void ShouldConfigureTenantPipelineOnDemand()
        {
            var counters = new Dictionary<string, int>()
            {
                { "tenant1", 0 },
                { "tenant2", 0 }
            };
            using (var server = CreateTestServer(
                new MockITenantResolver(
                    new[] { new MockTenant("tenant1"), new MockTenant("tenant2") },
                    (tenant) =>
                        new TenantContext<MockTenant>(tenant,
                            new ServiceCollection()
                                .AddSingleton(new MockStatefullService() { State = tenant.Identifier })
                                .BuildServiceProvider()
                        )
                ),
                (tenantApp, tenantCtx) => {
                    counters[tenantCtx.Tenant.Identifier]++;
                }
            ))
            {
                var client = server.CreateClient();
                await client.GetStringAsync("http://tenant1/");
            }
            Assert.Equal(1, counters["tenant1"]);
            Assert.Equal(0, counters["tenant2"]);
        }

        #region Helper functions
        private static TestServer CreateTestServer(ITenantResolver<MockTenant> tenantResolver, Action<IApplicationBuilder, TenantContext<MockTenant>> additionalTenantConfiguration = null)
        {
            return new TestServer(
                new WebHostBuilder()
                    .ConfigureServices(
                        services =>
                        {
                            services.AddMultitenancy(tenantResolver);
                        })
                    .Configure(
                        app =>
                        {
                            app.UseMultitenancy<MockTenant>();
                            app.ConfigureTenant<MockTenant>((tenantApp, tenantCtx) =>
                            {
                                if (additionalTenantConfiguration != null)
                                {
                                    additionalTenantConfiguration.Invoke(tenantApp, tenantCtx);
                                }
                                tenantApp.Run(
                                    async ctx =>
                                    {
                                        await ctx.Response.WriteAsync(tenantCtx.Tenant.Identifier);
                                    }
                                );
                            });
                            app.Run(
                                async ctx =>
                                {
                                    await ctx.Response.WriteAsync("Default");
                                }
                            );
                        })
            );
        }

        #endregion
    }
}
