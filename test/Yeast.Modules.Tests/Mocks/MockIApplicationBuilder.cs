﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;

namespace Yeast.Modules.Tests.Mocks
{
    public class MockIApplicationBuilder : IApplicationBuilder
    {
        public IServiceProvider ApplicationServices {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public IFeatureCollection ServerFeatures => throw new NotImplementedException();

        public IDictionary<string, object> Properties => throw new NotImplementedException();

        public RequestDelegate Build()
        {
            throw new NotImplementedException();
        }

        public IApplicationBuilder New()
        {
            throw new NotImplementedException();
        }

        public IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware)
        {
            throw new NotImplementedException();
        }
    }
}
