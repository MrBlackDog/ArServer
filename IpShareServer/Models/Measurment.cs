using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IpShareServer.Models
{
    public struct Measurment
    {
        public int State;
        public int ConstellationType;
        public long ReceivedSvTimeNanos;

        public Measurment(int constellationType, int state, long receivedSvtimeNanos)
        {
            ConstellationType = constellationType;
            State = state;
            ReceivedSvTimeNanos = receivedSvtimeNanos;
        }
    }
}
