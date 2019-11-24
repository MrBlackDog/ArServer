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
            //Convert.ToInt32(nextStr[6].Remove(nextStr[6].Length - 19 * 2, nextStr[6].Length).Split(".")[1])

            var rawStringToRemove = Eph.Split("\n")[0];
            var StringToRemove ="";
            try
            {
                StringToRemove = rawStringToRemove.Remove((rawStringToRemove.Length - 19 * 3 - 1), 57).Replace("\r", "");
            }
            catch
            {
                StringToRemove = "";
            }
                var xd = Eph.Replace("\r", "");
            var xdnew = xd.Remove(0, StringToRemove.Length);
            String[] ParsedString = new String[31];
            int j = 0;
            var xdxd = xdnew.Split("\n");
            for (int x = 0; x < 6; x++)
            {
                int SkipedCrars = 0;
                string currstr = xdxd[x];
                if (xdxd[x].Length % 19 != 0)
                {
                    currstr = currstr.Insert(0, " ");
                }
                for (int i = 0; i < currstr.Length; i += 19)
                {
                    //var xd3 = xd.Remove(0, SkipedCrars).Substring(0, 19);
                    //if (str.Length % 19 != 0 && flag)
                    //{
                    //   ParsedString[j] = str.Remove(0, SkipedCrars).Substring(0, 18).Replace('D', 'e');
                    //}
                    try
                    {
                        ParsedString[j] = currstr.Remove(0, SkipedCrars).Substring(0, 19).Replace('D', 'e');
                    }
                    //ParsedString[j] = ParsedString[j].Replace(" ", "").Replace('D', 'e');
                    catch
                    {
                        ParsedString[j] = "0";
                    }
                    finally
                    {
                        j++;
                        SkipedCrars += 19;
                    }
                }
            }
            try
            {
                IODE = Double.Parse(ParsedString[3].Replace("\n", ""));
                Crs = Double.Parse(ParsedString[4].Replace("\n", ""));
                delta_n = Double.Parse(ParsedString[5].Replace("\n", ""));
                M0 = Double.Parse(ParsedString[6].Replace("\n", ""));
                Cuc = Double.Parse(ParsedString[7].Replace("\n", ""));
                e = Double.Parse(ParsedString[8].Replace("\n", ""));
                Cus = Double.Parse(ParsedString[9].Replace("\n", ""));
                A0 = Math.Pow(Double.Parse(ParsedString[10].Replace("\n", "")), 2);
                Toe = Double.Parse(ParsedString[11].Replace("\n", ""));
                Cic = Double.Parse(ParsedString[12].Replace("\n", ""));
                OMEGA = Double.Parse(ParsedString[13].Replace("\n", ""));
                Cis = Double.Parse(ParsedString[14].Replace("\n", ""));
                i0 = Double.Parse(ParsedString[15].Replace("\n", ""));
                Crc = Double.Parse(ParsedString[16].Replace("\n", ""));
                omega = Double.Parse(ParsedString[17].Replace("\n", ""));
                OMEGA_DOT = Double.Parse(ParsedString[18].Replace("\n", ""));
                i0_dot = Double.Parse(ParsedString[19].Replace("\n", ""));
            }
            catch
            {
                Console.WriteLine("Error");
                IODE = 0;
                Crs = 0;
                delta_n = 0;
                M0 = 0;
                Cuc = 0;
                e = 0;
                Cus = 0;
                A0 = 0;
                Toe = 0;
                Cic = 0;
                OMEGA = 0;
                Cis = 0;
                i0 = 0;
                Crc = 0;
                omega = 0;
                OMEGA_DOT = 0;
                i0_dot = 0;
            }

            /*IODE = Double.Parse(ParsedString[0]);
            Crs = Double.Parse(ParsedString[1]);
            delta_n = Double.Parse(ParsedString[2]);
            M0 = Double.Parse(ParsedString[3]);
            Cuc = Double.Parse(ParsedString[4]);
            e = Double.Parse(ParsedString[5]);
            Cus = Double.Parse(ParsedString[6]);
            A0 = Math.Pow(Double.Parse(ParsedString[7]), 2);
            Toe = Double.Parse(ParsedString[8]);
            Cic = Double.Parse(ParsedString[9]);
            OMEGA = Double.Parse(ParsedString[10]);
            Cis = Double.Parse(ParsedString[11]);
            i0 = Double.Parse(ParsedString[12]);
            Crc = Double.Parse(ParsedString[13]);
            omega = Double.Parse(ParsedString[14]);
            OMEGA_DOT = Double.Parse(ParsedString[15]);
            i0_dot = Double.Parse(ParsedString[16]);*/
        }
    }
}
