using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using Xunit;
using Yeast.Modules.Abstractions;
using Yeast.Modules.Tests.Mocks;

namespace Yeast.Modules.Tests
{
    public class ModuleManagerTests
    {
        [Fact]
        public void TrowsOnNullModuleLoader()
        {
            // Arrange

            // Act
            Action instanciateModuleManager = () => new ModuleManager(null);

            // Assert
            Assert.Throws<ArgumentNullException>(instanciateModuleManager);
        }

        [Fact]
        public void CanEnableLoadedModules()
        {
            // Arrange
            var moduleLoader = new MockIModuleLoader(new ILoadedModule[] {
                new MockILoadedModule("Module1"),
                new MockILoadedModule("Module2")
            });
            var moduleManager = new ModuleManager(moduleLoader);

            // Act
            moduleManager.EnableModules("Module1", "Module2");

            // Assert
            Assert.Contains(moduleManager.EnabledModules, moduleName => moduleName == "Module1");
            Assert.Contains(moduleManager.EnabledModules, moduleName => moduleName == "Module2");
        }

        [Fact]
        public void CanEnableModuleMultipleTimes()
        {
            // Arrange
            var moduleLoader = new MockIModuleLoader(
                new MockILoadedModule("Module1"),
                new MockILoadedModule("Module2")
            );
            var moduleManager = new ModuleManager(moduleLoader);

            // Act
            moduleManager.EnableModules("Module1");
            moduleManager.EnableModules("Module1", "Module2");

            // Assert
            Assert.Equal(2, moduleManager.EnabledModules.Count());
            Assert.Contains(moduleManager.EnabledModules, moduleName => moduleName == "Module1");
            Assert.Contains(moduleManager.EnabledModules, moduleName => moduleName == "Module2");
        }

        [Fact]
        public void ConfigureModulesServicesCallsModulesIStartups()
        {
            // Arrange
            var startup1 = new MockStartup();
            var startup2 = new MockStartup();
            var moduleLoader = new MockIModuleLoader(
                new MockILoadedModule("Module1")
                {
                    Startups = new IStartup[]
                    {
                        startup1
                    }
                },
                new MockILoadedModule("Module2")
                {
                    Startups = new IStartup[]
                    {
                        startup2
                    }
                }
            );
            var moduleManager = new ModuleManager(moduleLoader);
            moduleManager.EnableModules("Module1", "Module2");

            // Act
            moduleManager.ConfigureModulesServices(new ServiceCollection());

            // Assert
            Assert.Equal(1, startup1.ConfigureServicesCallsCount);
            Assert.Equal(1, startup2.ConfigureServicesCallsCount);
        }

        [Fact]
        public void ConfigureServicesCallsModulesIStartups()
        {
            // Arrange
            var startup1 = new MockStartup();
            var startup2 = new MockStartup();
            var moduleLoader = new MockIModuleLoader(
                new MockILoadedModule("Module1")
                {
                    Startups = new IStartup[]
                    {
                        startup1
                    }
                },
                new MockILoadedModule("Module2")
                {
                    Startups = new IStartup[]
                    {
                        startup2
                    }
                }
            );
            var moduleManager = new ModuleManager(moduleLoader);
            moduleManager.EnableModules("Module1", "Module2");

            // Act
            moduleManager.ConfigureModules(new MockIApplicationBuilder());

            // Assert
            Assert.Equal(1, startup1.ConfigureCallsCount);
            Assert.Equal(1, startup2.ConfigureCallsCount);
        }
    }
}
