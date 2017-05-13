using FishKing.DataTypes;
using FishKing.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FishKing.Enums.WaterTypes;

namespace FishKing.Extensions
{
    static class FishExtensions
    {

        public static Fish_Types GetRandomFish(this List<Tuple<Fish_Types, int>> list)
        {
            var probabilitySum = list.Sum(ft => ft.Item2);
            Fish_Types fishType;
            var ftdrng = new DistributedRandomGenerator();

            foreach (Tuple<Fish_Types, int> ftt in list)
            {
                ftdrng.AddValue(ftt.Item1, (double)Decimal.Divide(ftt.Item2, probabilitySum));
            }

            fishType = (Fish_Types)ftdrng.GetDistributedRandomValue(RandomNumbers.Random.NextDouble());
            return fishType;
        }

        public static DataTypes.Fish_Types GetValue<Fish_Types>(string value)
        {
            return GlobalContent.Fish_Types[value];
        }
    }
}
