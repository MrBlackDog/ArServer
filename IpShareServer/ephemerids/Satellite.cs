using System;
using System.Collections.Generic;
using System.Linq;

namespace IpShareServer.Models
{
    class Satellite
    {
        #region Константы
        public const double GM = 3.986005e14;
        public const double С = 2.99792458e8;
        public const double omegae = 7.2921151467e-5;

        #endregion

        #region Глобальные свойства
        /// <summary>
        /// Широта
        /// </summary>
        public double Lat { get; private set; }

        /// <summary>
        /// Радиус
        /// </summary>
        public double Rad { get; private set; }

        /// <summary>
        /// Наклон
        /// </summary>
        public double Alt { get; private set; }
        public List<float> ephemerids = new List<float>();
        public List<float> IONAlpha = new List<float>();
        #endregion

        #region Приватные поля
        /// <summary>
        /// Среднее движение
        /// </summary>
        private double n0;

        /// <summary>
        /// Время эферемид
        /// </summary>
        private double Tk;

        /// <summary>
        /// Корректировка времени
        /// </summary>
        private double Tsv;
        private double Tknew;

        /// <summary>
        /// Корректировка cреднего движения 
        /// </summary>
        private double Na;

        /// <summary>
        /// Средняя аномалия
        /// </summary>
        private double Ma;

        /// <summary>
        /// Уравнение Кеплера для эксцентричной аномалии
        /// </summary>
        private double Mk;

        /// <summary>
        /// Истинная аномалия
        /// </summary>
        private double Vk;

        /// <summary>
        /// Эксцентрическая аномалия
        /// </summary>
        private double Ek;

        /// <summary>
        /// Аргумент широты
        /// </summary>
        private double Fk;

        /// <summary>
        /// Коррекция широты, радиуса и наклона
        /// </summary>
        /// Не знаю,нужно ли нам будет переводить ширину наклон и радиус в XYZ
        private double deltaUk;
        private double deltaRk;
        private double deltaIk;
         
        /// <summary>
        /// Координаты в ECEF
        /// </summary>
        private double Xk;
        private double Yk;
        private double Zk;
        #endregion
        //8.3820D-09 -7.4510D-09 -5.9600D-08  5.9600D-08          ION ALPHA
        public Satellite(string Info, string Header)
        {
            var inf = Info.Substring(22);
            System.Text.RegularExpressions.Regex.Replace(inf, @"[^De]-", " -").Split(' ').
                        Where(e => !string.IsNullOrEmpty(e)).Select(e => e.Replace("\r", "").Replace('D', 'e')).
                        ToList().ForEach(e => ephemerids.Add(float.Parse(e)));
            var header = Header.Substring(229);
            System.Text.RegularExpressions.Regex.Replace(header, @"[^De]-", " -").Split(' ').
                        Where(e => !string.IsNullOrEmpty(e)).Take(3).Select(e => e.Replace("\r", "").Replace('D', 'e')).
                        ToList().ForEach(e => IONAlpha.Add(float.Parse(e)));
            Console.WriteLine("Спутник добавлен");
        }

        public void CalculatePosition(int t)
        {
        /*    Tk = t - toe;//t- время,на которое хотим посчитать эфимериды, указывать в секундах.
            n0 = Math.Sqrt(GM / Alt(Math.Pow(A0, 3)));
            Na = n0 + deltaNa;
            Mk = M0 + Na * Tk;
            Ek = Kepler(0, En);
            Vk = (Math.Acos(Math.Cos(Ek) - En)) / (1 - En * Math.Cos(Ek));
            Fk = Vk + omega;
            deltaUk = Cuc * Math.Cos(2 * Fk) + Cus * Math.Sin(2 * Fk);
            deltaRk = Crc * Math.Cos(2 * Fk) + Crs * Math.Sin(2 * Fk);
            deltaIk = Cic * Math.Cos(2 * Fk) + Cis * Math.Sin(2 * Fk);
            var Uk = Fk + deltaUk;
           var  Rk = A * (1 - En * Math.Cos(Ek)) + deltaRk;
           var Ik = i0 + Itk + deltaIk;
           var Xshtk = Rk * Math.Cos(Uk);
          var  Yshtk = Rk * Math.Sin(Uk);
          var  Omegak = Omega0 + (OMEGA_Dot - omegae) * Tk - omegae * T0e;
            //ECEF
            Xk = Xshtk * Math.Cos(Omegak) - Yshtk * Math.Sin(Omegak) * Math.Cos(Ik);
            Xk = Xshtk * Math.Sin(Omegak) + Yshtk * Math.Cos(Omegak) * Math.Cos(Ik);
            Zk = Yshtk * Math.Cos(Ik);*/
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
        }
    }
}