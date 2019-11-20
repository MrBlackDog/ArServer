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
        public const double odote = 7.2921151467e-5;
        public List<float> ephemerids = new List<float>();
        #endregion

        #region Приватные поля
        //
        //вторая строка
        public double IODE;//Issue of Data Ephemeris
        public double Crs;//sine term, radius
        public double delta_n;//mean motion difference
        public double M0; //mean anomaly at reference time
        //третья строка
        public double Cuc;//cosine term, arg. of latitude
        public double e;//eccentricity
        public double Cus;//sine term, arg. of latitude
        public double A0;//sqrt(A)  where A is semimajor axis
        //четвертая строка
        public double Toe;//time of ephemeris
        public double Cic;//cosine term, inclination
        public double OMEGA;//LoAN at weekly epoch
        public double Cis;//sine term, inclination
        //четвертая строка
        public double i0;//inclination at reference time
        public double Crc;//cosine term, radius
        public double omega;//argument of perigee
        public double OMEGA_DOT;//rate of right ascension 
        //пятая строка
        public double i0_dot;//rate of inclination angle
        public double L2_codes;
        public double GPS_week;//GPS week
        public double L2_dataflag;
        //шестая строка
        public double SV_acc;
        public double SV_health;
        public double TGD;
        public double IODC;
        //седьмая строка
        public double msg_trans_t;
        public double fit_int;
        #endregion
        public Ephemeris(string Eph)
        {
            var inf = Eph.Substring(3);
            var xd2 = System.Text.RegularExpressions.Regex.Replace(inf, @"[^De]-", " -").Replace("\n", " ").Split(' ').
                        Where(e => !string.IsNullOrEmpty(e)).Select(e => e.Replace("\r", "").Replace('D', 'e').Replace('E', 'e')).ToArray();
            //.ToList().ForEach(e => ephemerids.Add(float.Parse(e)));
            //var EphInf = ephemerids.ToArray();
            //var xd = inf[0];
            IODE = Double.Parse(xd2[3]);
            Crs = Double.Parse(xd2[4]);
            delta_n = Double.Parse(xd2[5]);
            M0 = Double.Parse(xd2[6]);
            Cuc = Double.Parse(xd2[7]);
            e = Double.Parse(xd2[8]);
            Cus = Double.Parse(xd2[9]);
            A0 = Math.Pow(Double.Parse(xd2[10]) , 2);
            Toe = Double.Parse(xd2[11]);
            Cic = Double.Parse(xd2[12]);
            OMEGA = Double.Parse(xd2[13]);
            Cis = Double.Parse(xd2 [14]);
            i0 = Double.Parse(xd2[15]);
            Crc = Double.Parse(xd2 [16]);
            omega = Double.Parse(xd2[17]);
            OMEGA_DOT = Double.Parse(xd2[18]);
            i0_dot = Double.Parse(xd2[19]);

        }
    }
}
