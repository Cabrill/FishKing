using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.UtilityClasses
{
    public static class InfoToString
    {
        public static string Weight(int grams)
        {
            var kg = (grams / 1000);


            if (kg >= 1)
            {
                return String.Format("{0:0.00}", decimal.Divide(grams, 1000)) + "kg";
            }
            else
            {
                return $"{grams}g";
            }

        }

        public static string Length(int mm)
        {
            var meters = (mm / 1000);
            if (meters >= 1)
            {
                return String.Format("{0:0.00}", decimal.Divide(mm, 1000)) + "m";
            }
            else
            {
                var cm = (mm / 10);
                if (cm >= 1)
                {
                    return String.Format("{0:0.00}", decimal.Divide(mm, 100)) + "cm";
                }
                else
                {
                    return $"{mm}mm";
                }
            }
        }

        public static string Time(TimeSpan timeSpan)
        {
            return String.Format("{0} H {1} M {2} S", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
        }

        public static string Date(DateTime dateTime)
        {
            return String.Format(dateTime.ToString("MMM dd{0}, YYYY"), AddOrdinal(dateTime.Day));
        }

        public static string AddOrdinal(int num)
        {
            if (num <= 0) return num.ToString();

            switch (num % 100)
            {
                case 11:
                case 12:
                case 13:
                    return num + "th";
            }

            switch (num % 10)
            {
                case 1:
                    return num + "st";
                case 2:
                    return num + "nd";
                case 3:
                    return num + "rd";
                default:
                    return num + "th";
            }

        }
    }
}
