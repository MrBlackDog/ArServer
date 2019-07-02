using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IpShareServer.Models
{
    public struct GNSSClock
    {
        public long TimeNanos;
        public long FullBiasNanos;
        public double BiasNanos;

        public GNSSClock(long timenanos, long fullbiasnanos, double biasnanos)
        {
            TimeNanos = timenanos;
            FullBiasNanos = fullbiasnanos;
            BiasNanos = biasnanos;
        }
    }
}
