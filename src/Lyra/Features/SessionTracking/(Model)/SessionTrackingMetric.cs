using System;
using System.Collections.Generic;

namespace Lyra.Features.SessionTracking
{
    public class SessionTrackingMetric
    {
        public DateTime LogTimestamp { get; set; } = DateTime.UtcNow;

        public string Metric { get; set; }

        public Dictionary<string, object> Tags { get; set; } = new();
    }
}