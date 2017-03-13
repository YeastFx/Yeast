using System.Collections.Generic;
using System.Reflection;
using Yeast.Modules.Abstractions;

namespace Yeast.Modules
{
    public interface ILoadedModule
    {
        Assembly Assembly { get; }
        ModuleInfo Infos { get; }
        string Name { get; }
        string Path { get; }
        IEnumerable<FeatureInfo> Features { get; }
    }
}