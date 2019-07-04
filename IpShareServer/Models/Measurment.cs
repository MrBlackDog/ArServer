using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IpShareServer.Models
{
    public struct Measurment
    {
        public int Svid;
        public int ConstellationType;
        public double TimeOffsetNanos;
        public int State;
        public long ReceivedSvTimeNanos;
        public long ReceivedSvTimeUncertaintyNanos;
        public double Cn0DbHz;
        public double PseudorangeRateMetersPerSecond;
        public double PseudorangeRateUncertaintyMetersPerSecond;
        public int AccumulatedDeltaRangeState;
        public double AccumulatedDeltaRangeMeters;
        public double AccumulatedDeltaRangeUncertaintyMeters;
        public float CarrierFrequencyHz;
        public long CarrierCycles;
        public double CarrierPhase;
        public double CarrierPhaseUncertainty;
        public int MultipathIndicator;
        public double SnrInDb;
        public double AgcDb;
        public float CarrierFreqHz;

        public Measurment(int svid, int constellationType, double timeOffsetNanos, int state, long receivedSvtimeNanos, long receivedSvTimeUncertaintyNanos, double cn0DbHz, double pseudorangeRateMetersPerSecond,
            double pseudorangeRateUncertaintyMetersPerSecond, int accumulatedDeltaRangeState, double accumulatedDeltaRangeMeters, double accumulatedDeltaRangeUncertaintyMeters, float carrierFrequencyHz, long carrierCycles, double carrierPhase,
            double carrierPhaseUncertainty, int multipathIndicator, double snrInDb, double agcDb, float carrierFreqHz)
        {
            Svid = svid;
            ConstellationType = constellationType;
            TimeOffsetNanos = timeOffsetNanos;
            State = state;
            ReceivedSvTimeNanos = receivedSvtimeNanos;
            ReceivedSvTimeUncertaintyNanos = receivedSvTimeUncertaintyNanos;
            Cn0DbHz = cn0DbHz;
            PseudorangeRateMetersPerSecond = pseudorangeRateMetersPerSecond;
            PseudorangeRateUncertaintyMetersPerSecond = pseudorangeRateUncertaintyMetersPerSecond;
            AccumulatedDeltaRangeState = accumulatedDeltaRangeState;
            AccumulatedDeltaRangeMeters = accumulatedDeltaRangeMeters;
            AccumulatedDeltaRangeUncertaintyMeters = accumulatedDeltaRangeUncertaintyMeters;
            CarrierFrequencyHz = carrierFrequencyHz;
            CarrierCycles = carrierCycles;
            CarrierPhase = carrierPhase;
            CarrierPhaseUncertainty = carrierPhaseUncertainty;
            MultipathIndicator = multipathIndicator;
            SnrInDb = snrInDb;
            AgcDb = agcDb;
            CarrierFreqHz = carrierFreqHz;
        }
    }
}
