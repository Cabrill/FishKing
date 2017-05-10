using FishKing.DataTypes;
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
            return String.Format(dateTime.ToString("MMM {0}, yyyy"), AddOrdinal(dateTime.Day));
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

        public static string PlacesCaught(Fish_Types fishType)
        {
            var placesString = "";
            if (fishType.InDeepOcean > 0)
            {
                placesString += "Ocean,";
            }
            if (fishType.InRiver > 0)
            {
                placesString += " River,";
            }
            if (fishType.InLake > 0)
            {
                placesString += " Lake,";
            }
            if (fishType.InPond > 0)
            {
                placesString += " Pond,";
            }
            if (fishType.InWaterfall > 0)
            {
                placesString += " Waterfall,";
            }
            if (fishType.InOcean > 0)
            {
                placesString += " Beach,";
            }
            if (fishType.InCaveLake > 0)
            {
                placesString += " Cave,";
            }

            placesString = placesString.Trim();
            placesString = placesString.Substring(0, placesString.Length - 1);
            return placesString;
        }
    }
}
