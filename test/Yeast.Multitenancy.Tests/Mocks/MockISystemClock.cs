using Microsoft.Extensions.Internal;
using System;

namespace Yeast.Multitenancy.Tests.Mocks
{
    public class MockISystemClock : ISystemClock
    {
        public MockISystemClock()
        {
            UtcNow = DateTime.UtcNow;
        }

        public DateTimeOffset UtcNow { get; set; }

        public void Add(TimeSpan elapsed) {
            UtcNow = UtcNow.Add(elapsed);
        }
    }
}
