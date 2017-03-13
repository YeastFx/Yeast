using System;
using System.Linq;
using Xunit;
using Yeast.Features.Tests.Mocks;

namespace Yeast.Features.Tests
{
    public class FeatureManagerBuilderTests
    {
        [Fact]
        public void MergesProvidedFeatures()
        {
            // Arrange
            var builder = new FeatureManagerBuilder();
            var featureA = new MockFeatureInfo("FeatureA");
            var featureB = new MockFeatureInfo("FeatureB");

            // Act
            builder.AddFeatureProvider(new MockFeatureProvider
            {
                Features = new[] { featureA }
            });

            builder.AddFeatureProvider(new MockFeatureProvider
            {
                Features = new[] { featureB }
            });

            var manager = builder.Build();

            // Assert
            Assert.Equal(2, manager.AvailableFeatures.Count());
            Assert.Contains(featureA, manager.AvailableFeatures);
            Assert.Contains(featureB, manager.AvailableFeatures);
        }

        [Fact]
        public void EnablesFeaturesByName()
        {
            // Arrange
            var builder = new FeatureManagerBuilder();
            var featureA = new MockFeatureInfo("FeatureA");
            var featureB = new MockFeatureInfo("FeatureB");
            var featureC = new MockFeatureInfo("FeatureC");

            // Act
            builder.AddFeatureProvider(new MockFeatureProvider
            {
                Features = new[] { featureA, featureB, featureC }
            });

            builder.EnableFeature(featureA.Name);
            builder.EnableFeature(featureB.Name);

            var manager = builder.Build();

            // Assert
            Assert.Equal(2, manager.EnabledFeatures.Count());
            Assert.Contains(featureA, manager.EnabledFeatures);
            Assert.Contains(featureB, manager.EnabledFeatures);
            Assert.DoesNotContain(featureC, manager.EnabledFeatures);
        }

        [Fact]
        public void ThrowsOnUnavailableFeatures()
        {
            // Arrange
            var builder = new FeatureManagerBuilder();
            var featureA = new MockFeatureInfo("FeatureA");
            var featureB = new MockFeatureInfo("FeatureB");

            // Act
            builder.AddFeatureProvider(new MockFeatureProvider
            {
                Features = new[] { featureA, featureB }
            });

            builder.EnableFeature(featureA.Name);
            builder.EnableFeature("foo");

            Action buildAction = () => builder.Build();

            // Assert
            Assert.Throws<InvalidOperationException>(buildAction);
        }

        [Fact]
        public void RemovesProvidedFeaturesDuplicates()
        {
            // Arrange
            var builder = new FeatureManagerBuilder();
            var featureA = new MockFeatureInfo("FeatureA");

            // Act
            builder.AddFeatureProvider(new MockFeatureProvider
            {
                Features = new[] { featureA }
            });

            builder.AddFeatureProvider(new MockFeatureProvider
            {
                Features = new[] { featureA }
            });

            var manager = builder.Build();

            // Assert
            Assert.Equal(1, manager.AvailableFeatures.Count());
            Assert.Contains(featureA, manager.AvailableFeatures);
        }

        [Fact]
        public void DontCreateEnabledFeaturesDuplicates()
        {
            // Arrange
            var builder = new FeatureManagerBuilder();
            var featureA = new MockFeatureInfo("FeatureA");

            // Act
            builder.AddFeatureProvider(new MockFeatureProvider
            {
                Features = new[] { featureA }
            });

            builder.EnableFeature(featureA.Name);
            builder.EnableFeature(featureA.Name);

            var manager = builder.Build();

            // Assert
            Assert.Equal(1, manager.EnabledFeatures.Count());
            Assert.Same(featureA, manager.EnabledFeatures.First());
        }
    }
}
