using System;
using System.Collections.Generic;
using System.Linq;

namespace IpShareServer.Models
{
    class Satellite
    {
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
        #endregion
        //8.3820D-09 -7.4510D-09 -5.9600D-08  5.9600D-08          ION ALPHA
        public Satellite(string Info,string Header)
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
    }
}