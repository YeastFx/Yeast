using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Generic;
using System.Linq;
using Yeast.Features.Abstractions;

namespace Yeast.Features
{
    public class FeatureManager : IFeatureManager
    {
        private readonly List<FeatureInfo> _enabledFeatures;
        private readonly ILogger _logger;

        public FeatureManager(ILoggerFactory loggerFactory)
        {
            _logger = (ILogger)loggerFactory?.CreateLogger<FeatureManager>() ?? NullLogger.Instance;

            _enabledFeatures = new List<FeatureInfo>();
        }

        /// <inheritdoc />
        public IEnumerable<FeatureInfo> EnabledFeatures {
            get { return _enabledFeatures.AsEnumerable(); }
        }
    }
}
