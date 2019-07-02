using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IpShareServer.Models
{
    public struct Measurment
    {

        public int Svid;
        public int State;
        public int ConstellationType;
        public long ReceivedSvTimeNanos;

        public Measurment(int svid, int constellationtype, int state, long receivedsvtimenanos)
        {
            Svid = svid;
            ConstellationType = constellationtype;
            State = state;
            ReceivedSvTimeNanos = receivedsvtimenanos;
        }
    }
}
