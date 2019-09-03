using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpShareServer
{
    class Sputnik
    {
        public int number;
        public DateTime data;
        public string efemeridInfo;

        public Sputnik(int number,DateTime data,string efemeridInfo)
        {
            this.number = number;
            this.data = data;
            this.efemeridInfo = efemeridInfo;
        }
        public Sputnik()
        {

        }
         public Sputnik GetSputnik(int number, DateTime data, string efemeridInfo)
        {
            return new Sputnik(number, data, efemeridInfo);
        }
    }
    class BoxEqualityComparer : IEqualityComparer<Sputnik>
    {
        public bool Equals(Sputnik b1, Sputnik b2)
        {
            if (b2 == null && b1 == null)
                return true;
            else if (b1 == null || b2 == null)
                return false;
            else if (b1.number == b2.number)
                return true;
            else
                return false;
        }
        public int GetHashCode(Sputnik bx)
        {
            var r = new Random();
            
            int hCode = bx.number;
            return hCode.GetHashCode();
        }
    }

}
