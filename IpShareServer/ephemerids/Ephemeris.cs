using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IpShareServer.ephemerids
{
    public class Ephemeris
    {
        #region Константы
        public const double GM = 3.986005e14;
        public const double С = 2.99792458e8;
        public const double Omegae = 7.2921151467e-5;
        public List<float> ephemerids = new List<float>();
        #endregion

        #region Приватные поля
        private double IODE;
        private double crs;
        private double delta_n;
        private double M0;
        private double Cuc;
        private double e;
        private double Cus;
        private double A;
        private double Toe;
        private double Cic;
        private double OMEGA;
        private double Cis;
        private double i0;
        private double Crc;
        private double omega;
        private double OMEGA_DOT;
        #endregion
        public Ephemeris(string Eph)
        {
            var inf = Eph.Substring(3);
            var xd2 = System.Text.RegularExpressions.Regex.Replace(inf, @"[^De]-", " -").Replace("\n", " ").Split(' ').
                        Where(e => !string.IsNullOrEmpty(e)).Select(e => e.Replace("\r", "").Replace('D', 'e')).ToArray();
            //.ToList().ForEach(e => ephemerids.Add(float.Parse(e)));
            //var EphInf = ephemerids.ToArray();
            var xd = inf[0];
            //IODE = inf[0];
        }
    }
}
