using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Xunit;
using Yeast.Features.Tests.Mocks;

namespace Yeast.Features.Tests
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void ThrowsOnNullServices()
        {
            // Arrange
            IServiceCollection services = null;

            // Act
            Action addFeaturesAction = () => services.AddFeatures((builder) => { });

            // Assert
            Assert.Throws<ArgumentNullException>(addFeaturesAction);
        }

        [Fact]
        public void ThrowsOnNullConfiguration()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            Action addFeaturesAction = () => services.AddFeatures(null);

            // Assert
            Assert.Throws<ArgumentNullException>(addFeaturesAction);
        }

        [Fact]
        public void ReturnsTheFeatureManager()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            var result = services.AddFeatures((builder) => { });

            // Assert
            Assert.IsType<FeatureManager>(result);
        }

        [Fact]
        public void AddsTheFeatureManagerToServices()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            var result = services.AddFeatures((builder) => { });
            var descriptor = services.First();

            // Assert
            Assert.NotNull(descriptor);
            Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
            Assert.NotNull(descriptor.ImplementationInstance);
            Assert.IsType<FeatureManager>(descriptor.ImplementationInstance);
            Assert.Same(result, descriptor.ImplementationInstance);
        }

        [Fact]
        public void ConfiguresEnabledFeaturesServices()
        {
            // Arrange
            var services = new ServiceCollection();

            var featureADescriptor = new ServiceDescriptor(typeof(MockService), new MockService());
            var featureA = new MockFeatureInfo("FeatureA")
            {
                ConfigureAction = (serviceCollection, featureManager) =>
                {
                    serviceCollection.Add(featureADescriptor);
                }
            };

            var featureBDescriptor = new ServiceDescriptor(typeof(MockService), new MockService());
            var featureB = new MockFeatureInfo("FeatureB")
            {
                ConfigureAction = (serviceCollection, featureManager) =>
                {
                    serviceCollection.Add(featureBDescriptor);
                }
            };

            var featureProvider = new MockFeatureProvider {
                Features = new [] { featureA, featureB }
            };

            // Act
            var result = services.AddFeatures(builder => {
                builder.AddFeatureProvider(featureProvider);
                builder.EnableFeature(featureA);
            });

            // Assert
            Assert.Contains(featureADescriptor, services);
            Assert.DoesNotContain(featureBDescriptor, services);
        }

    }
}
