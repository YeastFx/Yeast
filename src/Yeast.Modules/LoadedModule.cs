using System.Reflection;

namespace Yeast.Modules
{
    public class LoadedModule
    {
        private readonly string _modulePath;
        private readonly Assembly _assembly;

        internal LoadedModule(string modulePath, Assembly assembly)
        {
            _modulePath = modulePath;
            _assembly = assembly;
        }

        public string Path {
            get { return _modulePath; }
        }

        public string Name {
            get { return _assembly.GetName().Name; }
        }

        public Assembly Assembly {
            get { return _assembly; }
        }
    }
}
