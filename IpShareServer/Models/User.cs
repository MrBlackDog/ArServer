using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace IpShareServer.Models
{
    public class User
    {
        public Measurment measurments { get; set; }
        public GNSSClock gnssClock { get; set; }
        public Vector3 Coords { get; set; }
    }
}
