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
        public Ephemeris _ephemeris;
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
            this._ephemeris = new Ephemeris(ephemerisInfo);
            //CalculatePosition(_ephemeris.Toe + 60);
        }
        public Sputnik()
        {

        }
        public Sputnik GetSputnik(int number, DateTime data, string efemeridInfo)
        {
            return new Sputnik(number, data, efemeridInfo);
        }
        public void CalculatePosition(double t)
        {
            var Tk = t - _ephemeris.Toe;//t- время,на которое хотим посчитать эфимериды, указывать в секундах.
            var n0 = Math.Sqrt(Ephemeris.GM / (Math.Pow(_ephemeris.A0, 3)));//верно
            var Na = n0 + _ephemeris.delta_n;//верно
            var Mk = _ephemeris.M0 + Na * Tk;//верно
            var Ek = Kepler(Mk, _ephemeris.e);
            //var Vk = Math.Acos((Math.Cos(Ek) - _ephemeris.e) / (1 - _ephemeris.e * Math.Cos(Ek)));
            var xddd = Math.Sqrt((1 - Math.Pow(_ephemeris.e, 2)));
            var Vk = Math.Atan2((Math.Pow(1 - Math.Pow(_ephemeris.e, 2), 0.5) * Math.Sin(Ek)) / (1 - _ephemeris.e * Math.Cos(Ek)), (Math.Cos(Ek) - _ephemeris.e) / (1 - _ephemeris.e * Math.Cos(Ek)));
            var Fk = Vk + _ephemeris.omega;
            var deltaUk = _ephemeris.Cuc * Math.Cos(2 * Fk) + _ephemeris.Cus * Math.Sin(2 * Fk);
            var deltaRk = _ephemeris.Crc * Math.Cos(2 * Fk) + _ephemeris.Crs * Math.Sin(2 * Fk);
            var deltaIk = _ephemeris.Cic * Math.Cos(2 * Fk) + _ephemeris.Cis * Math.Sin(2 * Fk);
            var Uk = Fk + deltaUk;
            var Rk = _ephemeris.A0 * (1 - _ephemeris.e * Math.Cos(Ek)) + deltaRk;
            var Ik = _ephemeris.i0 + Tk + deltaIk;
            var Xshtk = Rk * Math.Cos(Uk);
            var Yshtk = Rk * Math.Sin(Uk);
            var Omegak = _ephemeris.OMEGA + (_ephemeris.OMEGA_DOT - Ephemeris.odote) * Tk - Ephemeris.odote * _ephemeris.Toe;
            //ECEF
            Xk = Xshtk * Math.Cos(Omegak) - Yshtk * Math.Sin(Omegak) * Math.Cos(Ik);
            Yk = Xshtk * Math.Sin(Omegak) + Yshtk * Math.Cos(Omegak) * Math.Cos(Ik);
            Zk = Yshtk * Math.Cos(Ik);
            Console.WriteLine(Xk + " " + Yk + " " + Zk);
        }
        public double Kepler(double Mk, double En)
        {
            /* int m = 0;
             double[] E1 = new double[100];
             E1[1] = 0;
             while (true)
             {
                 E1[m + 1] = Mk + En * Math.Sin(E1[m]);
                 if (Math.Abs(E1[m + 1] - E1[m]) < 1e-8)
                     break;
                 m = m + 1;
             }
             return E1[m];*/
            double Ek = 0;
            double Ek0 = Mk;
            Ek = Mk + En * Math.Sin(Ek);
            while (Math.Abs(Ek - Ek0) > 1e8)
            {
                Ek0 = Ek;
                Ek = Mk + En * Math.Sin(Ek0);
            }
            return Ek;
        }
        public void CalculatePositionNew(double tsat)
        {
            var n0 = Math.Sqrt(Ephemeris.GM / (Math.Pow(_ephemeris.A0, 3)));//верно
            var t = tsat - _ephemeris.Toe;
            var n = n0 + _ephemeris.delta_n;//верно
            var m = _ephemeris.M0 + n * t;//верно

            var m_dot = n;

            var E = Kepler2(m, _ephemeris.e);

            // var Clock_Correction = (_ephemeris.alf);

            var E_dot = m_dot / (1 - _ephemeris.e * Math.Cos(E));

            var v = Math.Atan2(Math.Sqrt(1 - Math.Pow(_ephemeris.e, 2))
                * Math.Sin(E), Math.Cos(E) - _ephemeris.e);

            var v_dot = Math.Sin(E) * E_dot * (1 + _ephemeris.e * Math.Cos(v))/
                (Math.Sin(v) * (1 - _ephemeris.e * Math.Cos(v)));
            var phi = v + _ephemeris.omega;

            var phi_dot = v_dot;

            var du = _ephemeris.Cus * Math.Sin(2*phi) + _ephemeris.Cuc * Math.Cos(2*phi);
            var dr = _ephemeris.Crs * Math.Sin(2*phi) + _ephemeris.Crc * Math.Cos(2*phi);       
            var di = _ephemeris.Cis * Math.Sin(2*phi) + _ephemeris.Cic * Math.Cos(2*phi);
        
            var du_dot = 2 *(_ephemeris.Cus * Math.Cos(2*phi) -  _ephemeris.Cuc * Math.Cos(2*phi))
        }

        private double Kepler2(double m, double e)
        {
            double E = 0;
            double E_new;
            if ((-1 * Math.PI < m) && (m < 0) || (m > Math.PI))
                E = m - e;
            else
                E = m + e;

            double check = 1;

            while (check > 10e10)
            {
                E_new = (E + (m - E + e * Math.Sin(E)) / (1 - e * Math.Cos(E)));
                check = Math.Abs(E_new - E);
                E = E_new;
            }
            return E;
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
