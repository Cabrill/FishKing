using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.Enums
{
    public static class WaterTypes
    {
        public enum WaterType { None, Ocean, Lake, River, Pond, Waterfall, DeepOcean };

        public static List<string> AsStringList()
        {
            return Enum.GetNames(typeof(WaterType)).ToList();
        }

        public static WaterType WaterTypeNameToEnum(string name)
        {
            switch (name)
            {
                case "IsOcean": return WaterType.Ocean;
                case "IsLake": return WaterType.Lake; 
                case "IsRiver": return WaterType.River;
                case "IsPond": return WaterType.Pond;
                case "IsWaterfall": return WaterType.Waterfall; 
                case "IsDeepOcean": return WaterType.DeepOcean;
                default: return WaterType.None;
            }
        }
    }
}
