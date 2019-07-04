using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IpShareServer.Models
{
    public struct GNSSClock
    {
        public int LeapSecond;
        public long TimeNanos;
        public double TimeUncertaintyNanos;
        public long FullBiasNanos;
        public double BiasNanos;
        public double BiasUncertaintyNanos;
        public double DriftNanosPerSecond;
        public double DriftUncertaintyNanosPerSecond;
        public int HardwareClockDiscontinuityCount;

        public GNSSClock(int leapsecond,long timenanos, double timeuncertaintynanos, long fullbiasnanos,
            double biasnanos, double biasuncertaintynanos,double driftnanospersecond, double driftuncertaintynanospersecond, int hardwareclockdiscontinuitycount)
        {
            LeapSecond = leapsecond;
            TimeNanos = timenanos;
            TimeUncertaintyNanos = timeuncertaintynanos;
            FullBiasNanos = fullbiasnanos;
            BiasNanos = biasnanos;
            BiasUncertaintyNanos = biasuncertaintynanos;
            DriftNanosPerSecond = driftnanospersecond;
            DriftUncertaintyNanosPerSecond = driftuncertaintynanospersecond;
            HardwareClockDiscontinuityCount = hardwareclockdiscontinuitycount;
        }
    }
}
