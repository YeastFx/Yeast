using System;
using System.Linq;
using Xunit;
using Yeast.Features.Abstractions;
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
        public void EnablesFeaturesByInstance()
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

            builder.EnableFeature(featureA);
            builder.EnableFeature(featureB);

            var manager = builder.Build();

            // Assert
            Assert.Equal(2, manager.EnabledFeatures.Count());
            Assert.Contains(featureA, manager.EnabledFeatures);
            Assert.Contains(featureB, manager.EnabledFeatures);
            Assert.DoesNotContain(featureC, manager.EnabledFeatures);
        }

        [Fact]
        public void EnablesFeaturesByType()
        {
            // Arrange
            var builder = new FeatureManagerBuilder();
            var featureA = new MockFeatureInfo("FeatureA");
            var featureB = new MockInheritedFeatureInfo("FeatureB");
            var featureC = new MockInheritedFeatureInfoWithInterface("FeatureC");

            // Act
            builder.AddFeatureProvider(new MockFeatureProvider
            {
                Features = new[] { featureA, featureB, featureC }
            });

            builder.EnableFeature(typeof(MockInheritedFeatureInfo));

            var manager = builder.Build();

            // Assert
            Assert.Equal(1, manager.EnabledFeatures.Count());
            Assert.DoesNotContain(featureA, manager.EnabledFeatures);
            Assert.Contains(featureB, manager.EnabledFeatures);
            Assert.DoesNotContain(featureC, manager.EnabledFeatures);
        }

        [Fact]
        public void EnablesFeaturesByInterface()
        {
            // Arrange
            var builder = new FeatureManagerBuilder();
            var featureA = new MockFeatureInfo("FeatureA");
            var featureB = new MockInheritedFeatureInfo("FeatureB");
            var featureC = new MockInheritedFeatureInfoWithInterface("FeatureC");

            // Act
            builder.AddFeatureProvider(new MockFeatureProvider
            {
                Features = new[] { featureA, featureB, featureC }
            });

            builder.EnableFeature(typeof(IMockInheritedFeatureInfo));

            var manager = builder.Build();

            // Assert
            Assert.Equal(1, manager.EnabledFeatures.Count());
            Assert.DoesNotContain(featureA, manager.EnabledFeatures);
            Assert.DoesNotContain(featureB, manager.EnabledFeatures);
            Assert.Contains(featureC, manager.EnabledFeatures);
        }

        public static TheoryData<FeatureInfo[], Action<IFeatureManagerBuilder>> ThrowsOnUnavailableFeaturesData
        {
            get {
                return new TheoryData<FeatureInfo[], Action<IFeatureManagerBuilder>>()
                {
                    {
                        new []
                        {
                            new MockFeatureInfo("FeatureA"),
                            new MockFeatureInfo("FeatureB")
                        },
                        (builder) => builder.EnableFeature("Foo")
                    },
                    {
                        new []
                        {
                            new MockFeatureInfo("FeatureA"),
                            new MockFeatureInfo("FeatureB")
                        },
                        (builder) => builder.EnableFeature(new MockFeatureInfo("FeatureA"))
                    },
                    {
                        new []
                        {
                            new MockFeatureInfo("FeatureA"),
                            new MockFeatureInfo("FeatureB")
                        },
                        (builder) => builder.EnableFeature(typeof(MockInheritedFeatureInfo))
                    },
                    {
                        new []
                        {
                            new MockFeatureInfo("FeatureA"),
                            new MockFeatureInfo("FeatureB")
                        },
                        (builder) => builder.EnableFeature(typeof(IMockInheritedFeatureInfo))
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(ThrowsOnUnavailableFeaturesData))]
        public void ThrowsOnUnavailableFeatures(FeatureInfo[] availableFeatures, Action<IFeatureManagerBuilder> enableAction)
        {
            // Arrange
            var builder = new FeatureManagerBuilder();

            // Act
            builder.AddFeatureProvider(new MockFeatureProvider
            {
                Features = availableFeatures
            });

            enableAction.Invoke(builder);

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
        public void DontCreatesEnabledFeaturesDuplicates()
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

        [Fact]
        public void SortsFeaturesByPriority()
        {
            // Arrange
            var builder = new FeatureManagerBuilder();
            var featureA = new MockFeatureInfo("FeatureA", 0);
            var featureB = new MockFeatureInfo("FeatureB", 10);
            var featureC = new MockFeatureInfo("FeatureC", -10);

            // Act
            builder.AddFeatureProvider(new MockFeatureProvider
            {
                Features = new[] { featureA, featureB, featureC }
            });

            builder.EnableFeature(featureC.Name);
            builder.EnableFeature(featureB.Name);

            var manager = builder.Build();

            // Assert
            Assert.Equal(new[] { featureB, featureA, featureC }, manager.AvailableFeatures);
            Assert.Equal(new[] { featureB, featureC }, manager.EnabledFeatures);
        }
    }
}
