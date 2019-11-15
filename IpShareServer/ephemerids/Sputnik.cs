using IpShareServer.ephemerids;
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
        public string ephemerisInfo;
        public Ephemeris ephemeris;
        /// <summary>
        /// Координаты в ECEF
        /// </summary>
        private double Xk;
        private double Yk;
        private double Zk;
        public Sputnik(int number, DateTime data, string ephemerisInfo)
        {
            this.number = number;
            this.data = data;
            this.ephemerisInfo = ephemerisInfo;
            this.ephemeris = new Ephemeris(ephemerisInfo);
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
      /*  public void CalculatePosition(int t)
        {
              var Tk = t - toe;//t- время,на которое хотим посчитать эфимериды, указывать в секундах.
              var n0 = Math.Sqrt(GM / Alt(Math.Pow(A0, 3)));
              var Na = n0 + deltaNa;
              var Mk = M0 + Na * Tk;
              var Ek = Kepler(0, En);
              var Vk = (Math.Acos(Math.Cos(Ek) - En)) / (1 - En * Math.Cos(Ek));
              var Fk = Vk + omega;
              var deltaUk = Cuc * Math.Cos(2 * Fk) + Cus * Math.Sin(2 * Fk);
              var deltaRk = Crc * Math.Cos(2 * Fk) + Crs * Math.Sin(2 * Fk);
              var deltaIk = Cic * Math.Cos(2 * Fk) + Cis * Math.Sin(2 * Fk);
              var Uk = Fk + deltaUk;
              var Rk = A * (1 - En * Math.Cos(Ek)) + deltaRk;
              var Ik = i0 + Itk + deltaIk;
              var Xshtk = Rk * Math.Cos(Uk);
              var Yshtk = Rk * Math.Sin(Uk);
              var Omegak = Omega0 + (OMEGA_Dot - omegae) * Tk - omegae * T0e;
              //ECEF
              Xk = Xshtk * Math.Cos(Omegak) - Yshtk * Math.Sin(Omegak) * Math.Cos(Ik);
              Xk = Xshtk * Math.Sin(Omegak) + Yshtk * Math.Cos(Omegak) * Math.Cos(Ik);
              Zk = Yshtk * Math.Cos(Ik);
          }
          public double Kepler(double Mk, double En)
          {
              double Ek0 = Mk;
              while (Math.Abs(Ek - Ek0) > 1e8)
              {
                  Ek0 = Ek;
                  Ek = Mk + En * Math.Sin(Ek0);
              }
              return Ek;
          }*/
    }

}
