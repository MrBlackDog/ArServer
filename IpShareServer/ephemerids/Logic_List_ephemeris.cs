using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpShareServer
{
    static class Logic_List_ephemeris
    {
        static public string End_Method(string[] Str, int count)
        {
            string newStr = "";
            for (int i = count; i < Str.Length; i++)
            {
                newStr = newStr + Str[i] + '\n';
            }
            return newStr;
        }
        static public string NormString(string[] Str)
        {
            string nextStr = "";
            for (int i = 0; i < Str.Length; i++)
            {
                if (i != Str.Length - 1)
                {
                    if (Str[i] != "")
                    {
                        nextStr = nextStr + Str[i] + " ";
                    }
                }
                else
                {
                    if (Str[i] != "")
                    {
                        nextStr = nextStr + Str[i];
                    }
                }
            }
            return nextStr;
        }
        static public string[] noSpace(string[] ArrayInfo)
        {
            var info = NormString(ArrayInfo);
            var newInfo = info.Split(' ');
            return newInfo;
        }
        static public DateTime Get_Data(string[] nextStr)
        {
            int chas = 0;
            int god = Convert.ToInt32(nextStr[1]) + 2000;
            int mesyac = Convert.ToInt32(nextStr[2]);

            int den = Convert.ToInt32(nextStr[3]);
            int minute = Convert.ToInt32(nextStr[5]);
            if (minute == 59)
            {
                chas = Convert.ToInt32(nextStr[4]) + 1;
                minute = 0;
            }
            else
            { chas = Convert.ToInt32(nextStr[4]); }
            var data = new DateTime(god, mesyac, den, chas, minute, 0);
            return data;
        }
        static public List<Sputnik> CreateFirst_SateliteList(string[] Str)
        {
            var sputnik = new List<Sputnik>();
            int k = 0;
            int number = 0;
            DateTime data = new DateTime();
            string infor = "";
            for (int j = 0; j < Str.Length; j++)
            {
                var newStr = Str[j].Split(' ');
                if (newStr.Length > 1)
                {
                    var nextStr = noSpace(newStr);
                    if (k < 1)
                    {
                        number = Convert.ToInt32(nextStr[0]);
                        data = Get_Data(nextStr);
                        for (int i = 6; i < nextStr.Length; i++)
                        {
                            if (i != nextStr.Length - 1)
                                infor = infor + nextStr[i] + " ";
                            else infor = infor + nextStr[i] + '\n';
                        }
                    }
                    else
                    {
                        for (int i = 0; i < nextStr.Length; i++)
                        {
                            if (i != nextStr.Length - 1)
                                infor = infor + nextStr[i] + " ";
                            else infor = infor + nextStr[i] + '\n';
                        }
                    }
                    k++;
                    if (k == 8)
                    {
                        var sput = new Sputnik(number, data, infor);
                        sputnik.Add(sput);
                        infor = "";
                        number = 0;
                        k = 0;
                    }
                }
            }
            return sputnik;
        }
        private static string[] formationArraystring(string[]Str,int count)
        {
            string OnenumberSatelite = End_Method(Str, count);
            var newStr = OnenumberSatelite.Split('\n');
            return newStr;
        }
        private static List<Sputnik> GropBay_List(List<Sputnik> satelite,int i)
        {
            List<Sputnik> finish = new List<Sputnik>();
            var groupBya = satelite.GroupBy(item => item.data);
            var spSat = new Sputnik();
            foreach (var group in groupBya)
            {
                foreach (var item in group)
                {                   
                    switch (groupBya.Count())
                    {
                        case 1:
                            finish.Add(spSat.GetSputnik(item.number, item.data, item.ephemerisInfo));
                            break;
                        case 2:
                            if (i == 1)
                            {
                                finish.Add(spSat.GetSputnik(item.number, item.data, item.ephemerisInfo));
                            }
                            break;
                        case 3:
                            if (i == 2)
                            {
                                finish.Add(spSat.GetSputnik(item.number, item.data, item.ephemerisInfo));
            
                            }
                            break;
                        default:
                            Console.WriteLine("Разбиений нет");
                            break;
                    }
                }
                i++;
            }
            return finish;
        }
        public static IEnumerable<Sputnik> StartList(string BRDC_string )
        {
            var comparer = new BoxEqualityComparer();
            var Str = BRDC_string.Split('\n');
            int count = Str.Length - 258;
            int count_2 = count - 256;
            var End_arrayStr = formationArraystring(Str, count);
            var End_arrayStr2 = formationArraystring(Str, count_2);

            var satelite =CreateFirst_SateliteList(End_arrayStr);
            var satelite_2 = CreateFirst_SateliteList(End_arrayStr2);

            var finish = GropBay_List(satelite,0);
            var finish2 = GropBay_List(satelite_2,1);
            var union = finish.Union(finish2,comparer).OrderBy(item=>item.number);
            foreach(Sputnik sputnik in union)
            {
                sputnik.CalculatePositionNew(sputnik._ephemeris.Toe + 60);
                sputnik.CalculatePosition(sputnik._ephemeris.Toe + 60);
            }
            Console.WriteLine($"Количество спутников :{union.Count()}");
            return union;
        }
    }
}
