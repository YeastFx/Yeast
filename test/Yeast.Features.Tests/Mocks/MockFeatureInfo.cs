﻿using Microsoft.Extensions.DependencyInjection;
using System;
using Yeast.Features.Abstractions;

namespace Yeast.Features.Tests.Mocks
{
    public class MockFeatureInfo : FeatureInfo
    {
        private readonly string _featureName;
        private readonly int _priority;

        public MockFeatureInfo(string featureName, int priority = 0)
        {
            _featureName = featureName;
            _priority = priority;
        }

        public override string Name => _featureName;

        public override int Priority => _priority;

        public Action<IServiceCollection, IFeatureManager> ConfigureAction { get; set; }

        public override void ConfigureServices(IServiceCollection services, IFeatureManager featureManager)
        {
            ConfigureAction?.Invoke(services, featureManager);
        }
    }

    public class MockInheritedFeatureInfo : MockFeatureInfo
    {
        public MockInheritedFeatureInfo(string featureName) : base(featureName) { }
    }

    public interface IMockInheritedFeatureInfo { }

    public class MockInheritedFeatureInfoWithInterface : MockFeatureInfo, IMockInheritedFeatureInfo
    {
        public MockInheritedFeatureInfoWithInterface(string featureName) : base(featureName) { }
    }
}
