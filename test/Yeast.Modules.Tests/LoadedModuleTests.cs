using System;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using Xunit;

namespace Yeast.Modules.Tests
{
    public class LoadedModuleTests
    {
        private readonly string _modulesBasePath;

        public LoadedModuleTests()
        {
            _modulesBasePath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Modules");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ThrowsOnNullOrEmptyModulePath(string modulePath)
        {
            // Arrange
            var moduleAssemblyPath = Directory.EnumerateFiles(Path.Combine(_modulesBasePath, "ModuleA"), "ModuleA.dll", SearchOption.AllDirectories).FirstOrDefault();
            var moduleAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(moduleAssemblyPath);

            // Act
            Action instanciateLoadedModule = () => new LoadedModule(modulePath, moduleAssembly);

            // Assert
            Assert.Throws<ArgumentException>(instanciateLoadedModule);
        }

        [Fact]
        public void ThrowsOnNullModuleAssembly()
        {
            // Arrange
            var modulePath = Path.Combine(_modulesBasePath, "ModuleA");

            // Act
            Action instanciateLoadedModule = () => new LoadedModule(modulePath, null);

            // Assert
            Assert.Throws<ArgumentNullException>(instanciateLoadedModule);
        }

        [Fact]
        public void ThrowsOnNonModuleAssembly()
        {
            // Arrange
            var modulePath = Path.Combine(_modulesBasePath, "NotAModule");
            var moduleAssemblyPath = Directory.EnumerateFiles(modulePath, "NotAModule.dll", SearchOption.AllDirectories).FirstOrDefault();
            var moduleAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(moduleAssemblyPath);

            // Act
            Action instanciateLoadedModule = () => new LoadedModule(modulePath, moduleAssembly);

            // Assert
            Assert.Throws<InvalidOperationException>(instanciateLoadedModule);
        }

        [Fact]
        public void ExposesModuleStartups()
        {
            // Arrange
            var modulePath = Path.Combine(_modulesBasePath, "ModuleA");
            var moduleAssemblyPath = Directory.EnumerateFiles(modulePath, "ModuleA.dll", SearchOption.AllDirectories).FirstOrDefault();
            var moduleAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(moduleAssemblyPath);

            // Act
            var loadedModule = new LoadedModule(modulePath, moduleAssembly);

            // Assert
            Assert.Equal(1, loadedModule.Startups.Count());
            Assert.Equal("ModuleA.ModuleAStartup", loadedModule.Startups.First().GetType().FullName);
        }
    }
}
