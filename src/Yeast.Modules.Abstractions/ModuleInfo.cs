namespace Yeast.Modules.Abstractions
{
    public abstract class ModuleInfo
    {
        public string Name {
            get {
                return GetType().Name;
            }
        }
    }
}
