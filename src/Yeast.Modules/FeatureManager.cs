using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using Yeast.Modules.Abstractions;

namespace Yeast.Modules
{
    public class FeatureManager : IFeatureManager
    {
        private readonly List<FeatureInfo> _enabledFeatures;
        private readonly ILogger _logger;
        private readonly IModuleLoader _moduleLoader;

        public FeatureManager(IModuleLoader moduleLoader) : this(moduleLoader, null) { }

        public FeatureManager(IModuleLoader moduleLoader, ILoggerFactory loggerFactory)
        {
            _moduleLoader = moduleLoader ?? throw new ArgumentNullException(nameof(moduleLoader));

            _logger = (ILogger)loggerFactory?.CreateLogger<FeatureManager>() ?? NullLogger.Instance;

            _enabledFeatures = new List<FeatureInfo>();
        }

        /// <inheritdoc />
        public IEnumerable<FeatureInfo> EnabledFeatures {
            get { return _enabledFeatures.AsEnumerable(); }
        }
    }
}
