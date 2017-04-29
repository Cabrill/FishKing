using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.UtilityClasses
{
    public static class FishInfoToString
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

}
}
