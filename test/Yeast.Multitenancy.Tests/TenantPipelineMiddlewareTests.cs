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
            // Arrange
            string response1, response2;

            // Act
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
                (app) => {
                    app.ConfigureTenant<MockTenant>((tenantApp, tenantCtx) =>
                    {
                        tenantApp.Run(
                            async ctx =>
                            {
                                await ctx.Response.WriteAsync(tenantCtx.Tenant.Identifier);
                            }
                        );
                    });
                }
            ))
            {
                var client = server.CreateClient();
                response1 = await client.GetStringAsync("http://tenant1/");
                response2 = await client.GetStringAsync("http://tenant2/");
            }

            // Assert
            Assert.Equal("tenant1", response1);
            Assert.Equal("tenant2", response2);
        }

        [Fact]
        public async void ShouldNotForkPipelineIfNoTenantResolved()
        {
            // Arrange
            string response;

            // Act
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
                response = await client.GetStringAsync("http://foo/");
            }

            // Assert
            Assert.Equal("Default", response);
        }

        [Fact]
        public async void ShouldConfigurePipelineOncePerTenant()
        {
            // Arrange
            var counters = new Dictionary<string, int>()
            {
                { "tenant1", 0 },
                { "tenant2", 0 }
            };

            // Act
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
                (app) => {
                    app.ConfigureTenant<MockTenant>((tenantApp, tenantCtx) => {
                        counters[tenantCtx.Tenant.Identifier]++;
                    });
                }
            ))
            {
                var client = server.CreateClient();
                await client.GetStringAsync("http://tenant1/");
                await client.GetStringAsync("http://tenant2/");
                await client.GetStringAsync("http://tenant1/");
                await client.GetStringAsync("http://tenant2/");
            }

            // Assert
            Assert.Equal(1, counters["tenant1"]);
            Assert.Equal(1, counters["tenant2"]);
        }

        [Fact]
        public async void ShouldConfigureTenantPipelineOnDemand()
        {
            // Arrange
            var counters = new Dictionary<string, int>()
            {
                { "tenant1", 0 },
                { "tenant2", 0 }
            };

            // Act
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
                (app) => {
                    app.ConfigureTenant<MockTenant>((tenantApp, tenantCtx) => {
                        counters[tenantCtx.Tenant.Identifier]++;
                    });
                }
            ))
            {
                var client = server.CreateClient();
                await client.GetStringAsync("http://tenant1/");
            }

            // Assert
            Assert.Equal(1, counters["tenant1"]);
            Assert.Equal(0, counters["tenant2"]);
        }

        [Fact]
        public async void ApplicationServicesShouldReferenceTenantServices()
        {
            // Arrange
            var tenantServices = new Dictionary<string, IServiceProvider>();
            var applicationServices = new Dictionary<string, IServiceProvider>();

            Func<MockTenant, TenantContext<MockTenant>> tenantContextFactory = (tenant) =>
            {
                var serviceProvider = new ServiceCollection()
                    .AddSingleton(new MockStatefullService() { State = tenant.Identifier })
                    .BuildServiceProvider();
                tenantServices[tenant.Identifier] = serviceProvider;
                return new TenantContext<MockTenant>(tenant, serviceProvider);
            };

            // Act
            using (var server = CreateTestServer(
                new MockITenantResolver(
                    new[] { new MockTenant("tenant1"), new MockTenant("tenant2") },
                    tenantContextFactory
                ),
                (app) => {
                    app.ConfigureTenant<MockTenant>((tenantApp, tenantCtx) => {
                        applicationServices[tenantCtx.Tenant.Identifier] = tenantApp.ApplicationServices;
                    });
                }
            ))
            {
                var client = server.CreateClient();
                await client.GetStringAsync("http://tenant1/");
                await client.GetStringAsync("http://tenant2/");
            }

            // Assert
            Assert.NotSame(applicationServices["tenant1"], applicationServices["tenant2"]);
            Assert.Same(tenantServices["tenant1"], applicationServices["tenant1"]);
            Assert.Same(tenantServices["tenant2"], applicationServices["tenant2"]);
        }

        #region Helper functions
        private static TestServer CreateTestServer(ITenantResolver<MockTenant> tenantResolver, Action<IApplicationBuilder> additionalConfiguration = null)
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
                            if (additionalConfiguration != null)
                            {
                                additionalConfiguration.Invoke(app);
                            }
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
