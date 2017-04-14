using FishKing.DataTypes;
using FishKing.Entities;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing
{
    static class FishGenerator
    {
        private static Random randomSeed = new Random();

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

        public static Fish CreateFish(string waterType = "")
        {
            List<Fish_Types> availableFish;

            switch (waterType.ToLower())
            {
                case "river":
                    availableFish = GlobalContent.Fish_Types.Values.Where(ftv => ftv.InRiver > 0).ToList(); break;
                case "ocean":
                    availableFish = GlobalContent.Fish_Types.Values.Where(ftv => ftv.InOcean > 0).ToList(); break;
                case "deepocean":
                    availableFish = GlobalContent.Fish_Types.Values.Where(ftv => ftv.InDeepOcean > 0).ToList(); break;
                case "pond":
                    availableFish = GlobalContent.Fish_Types.Values.Where(ftv => ftv.InPond > 0).ToList(); break;
                case "lake":
                    availableFish = GlobalContent.Fish_Types.Values.Where(ftv => ftv.InLake > 0).ToList(); break;
                case "waterfall":
                    availableFish = GlobalContent.Fish_Types.Values.Where(ftv => ftv.InWaterfall > 0).ToList(); break;
                default:
                    availableFish = GlobalContent.Fish_Types.Values.ToList(); break;
            }
            Fish_Types fishType;
#if DEBUG
            if (DebuggingVariables.ForceAFishType && DebuggingVariables.ForcedFishType != null)
            {
                fishType = DebuggingVariables.ForcedFishType;
            }
            else
            {
                fishType = availableFish.RandomElement();
            }
#else
            fishType = availableFish.RandomElement();
#endif
            var fishTypeName = fishType.Name.ToString().Replace(" ","");


            var fish = Factories.FishFactory.CreateNew();
            fish.FishType = fishType;

            return fish;
        }

        private static T RandomElement<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.RandomElementUsing<T>(randomSeed);
        }

        private static T RandomElementUsing<T>(this IEnumerable<T> enumerable, Random rand)
        {
            int index = rand.Next(0, enumerable.Count());
            return enumerable.ElementAt(index);
        }
    }
}
