using FishKing.DataTypes;
using FishKing.Entities;
using FishKing.Extensions;
using FishKing.UtilityClasses;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FishKing.Enums.WaterTypes;

namespace FishKing
{
    static class FishGenerator
    {
        public static DistributedRandomGenerator drng = new DistributedRandomGenerator();

        public static int MaximumLengthMM
        {
            get
            {
                return GlobalContent.Fish_Types.Values.Max(v => v.MaxMM);
            }
        }

        public static int MinimumLengthMM
        {
            get
            {
                return GlobalContent.Fish_Types.Values.Min(v => v.AvgMM);
            }
        }

        static FishGenerator()
        {
            drng.AddValue(1, 0.1);
            drng.AddValue(2, 0.2);
            drng.AddValue(3, 0.4);
            drng.AddValue(4, 0.2);
            drng.AddValue(5, 0.1);
        }

        public static Fish CreateFish(WaterType waterType)
        {
            List<Tuple<Fish_Types,int>> availableFish;

            switch (waterType)
            {
                case WaterType.River:
                    availableFish = GlobalContent.Fish_Types.Values.Where(ftv => ftv.InRiver > 0).Select(ft => Tuple.Create(ft, ft.InRiver)).ToList(); break;
                case WaterType.Ocean:
                    availableFish = GlobalContent.Fish_Types.Values.Where(ftv => ftv.InOcean > 0).Select(ft => Tuple.Create(ft, ft.InOcean)).ToList(); break;
                case WaterType.DeepOcean:
                    availableFish = GlobalContent.Fish_Types.Values.Where(ftv => ftv.InDeepOcean > 0).Select(ft => Tuple.Create(ft, ft.InDeepOcean)).ToList(); break;
                case WaterType.Pond:
                    availableFish = GlobalContent.Fish_Types.Values.Where(ftv => ftv.InPond > 0).Select(ft => Tuple.Create(ft, ft.InPond)).ToList(); break;
                case WaterType.Lake:
                    availableFish = GlobalContent.Fish_Types.Values.Where(ftv => ftv.InLake > 0).Select(ft => Tuple.Create(ft, ft.InLake)).ToList(); break;
                case WaterType.CaveLake:
                    availableFish = GlobalContent.Fish_Types.Values.Where(ftv => ftv.InCaveLake > 0).Select(ft => Tuple.Create(ft, ft.InCaveLake)).ToList(); break;
                case WaterType.Waterfall:
                    availableFish = GlobalContent.Fish_Types.Values.Where(ftv => ftv.InWaterfall > 0).Select(ft => Tuple.Create(ft, ft.InWaterfall)).ToList(); break;
                default:
                    throw new Exception("Water type doesn't exist: " + waterType.ToString());
            }
            Fish_Types fishType;
#if DEBUG
            if (DebuggingVariables.ForceAFishType && DebuggingVariables.ForcedFishType != null)
            {
                fishType = DebuggingVariables.ForcedFishType;
            }
            else
            {
                fishType = availableFish.GetRandomFish();
            }
#else
            fishType = availableFish.GetRandomFish();
#endif
            var fishTypeName = fishType.Name.ToString().Replace(" ","");


            var fish = Factories.FishFactory.CreateNew();
            fish.FishType = fishType;

            return fish;
        }
    }
}
