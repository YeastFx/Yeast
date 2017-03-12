using System;
using System.IO;
using Xunit;

namespace Yeast.Modules.Tests
{
    public class ModuleLoaderTests
    {
        private readonly string _modulesBasePath;

        public ModuleLoaderTests()
        {
            _modulesBasePath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Modules");
        }

        [Fact]
        public void CanLoadAllModulesFromModulesDirectory()
        {
            // Arrange
            var moduleLoader = new ModuleLoader();
            var moduleDirectoryPath = Path.GetFullPath(_modulesBasePath);

            // Act
            moduleLoader.LoadModules(moduleDirectoryPath);

            // Assert
            Assert.True(moduleLoader.IsLoaded("ModuleA"));
            Assert.True(moduleLoader.IsLoaded("ModuleB"));
        }

        [Fact]
        public void CanLoadSingleModuleFromItsDirectory()
        {
            // Arrange
            var moduleLoader = new ModuleLoader();
            var moduleDirectoryPath = Path.GetFullPath(Path.Combine(_modulesBasePath, "ModuleA"));

            // Act
            moduleLoader.LoadModule(moduleDirectoryPath);

            // Assert
            Assert.True(moduleLoader.IsLoaded("ModuleA"));
            Assert.False(moduleLoader.IsLoaded("ModuleB"));
        }

        [Theory]
        [InlineData("ModuleA", "ClassA")]
        [InlineData("ModuleB", "ClassB")]
        public void CanRetrieveModuleExportedTypes(string moduleName, string typeName)
        {
            // Arrange
            var moduleLoader = new ModuleLoader();
            var moduleDirectoryPath = Path.GetFullPath(Path.Combine(_modulesBasePath, moduleName));

            // Act
            moduleLoader.LoadModule(moduleDirectoryPath);
            var module = moduleLoader.GetLoadedModuleByName(moduleName);
            var moduleTypes = module.Assembly.GetExportedTypes();

            // Assert
            Assert.Contains(moduleTypes, type => type.Name == typeName);
        }
    }
}
