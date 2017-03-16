using System;
using Xunit;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Yeast.Mvc.Tests.Mocks;
using System.Reflection;

namespace Yeast.Mvc.Tests
{
    public class MvcCoreBuilderExtensionsTests
    {
        [Fact]
        public void AddsEnabledMvcModulesToTheListOfApplicationParts()
        {
            // Arrange
            var manager = new ApplicationPartManager();
            var builder = new MvcCoreBuilder(new ServiceCollection(), manager);
            var assembly = typeof(MockMvcModule).GetTypeInfo().Assembly;

            var featureManager = new MockFeatureManager
            {
                EnabledFeatures = new [] { new MockMvcModule() }
            };

            // Act
            var result = builder.AddMvcModules(featureManager);

            // Assert
            Assert.Same(result, builder);
            var part = Assert.Single(builder.PartManager.ApplicationParts);
            var assemblyPart = Assert.IsType<AssemblyPart>(part);
            Assert.Equal(assembly, assemblyPart.Assembly);
        }
    }
}
