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
                    availableFish = GlobalContent.Fish_Types.Values.Where(ftv => ftv.InRiver).ToList(); break;
                case "ocean":
                    availableFish = GlobalContent.Fish_Types.Values.Where(ftv => ftv.InOcean).ToList(); break;
                case "deepocean":
                    availableFish = GlobalContent.Fish_Types.Values.Where(ftv => ftv.InDeepOcean).ToList(); break;
                case "pond":
                    availableFish = GlobalContent.Fish_Types.Values.Where(ftv => ftv.InPond).ToList(); break;
                case "lake":
                    availableFish = GlobalContent.Fish_Types.Values.Where(ftv => ftv.InLake).ToList(); break;
                case "waterfall":
                    availableFish = GlobalContent.Fish_Types.Values.Where(ftv => ftv.InWaterfall).ToList(); break;
                default:
                    availableFish = GlobalContent.Fish_Types.Values.ToList(); break;
            }
            var fishType = availableFish.RandomElement();
            var fishTypeName = fishType.Name.ToString().Replace(" ","");

            decimal gramsPerMM = (decimal)fishType.MaxGrams / (decimal)fishType.MaxMM;

            var fishLengthMM = randomSeed.Next(fishType.AvgMM, fishType.MaxMM);
            var fishGrams = (int)(fishLengthMM * gramsPerMM);
            
            var fish = Factories.FishFactory.CreateNew();
            fish.FishType = fishType;
            fish.Grams = fishGrams;
            fish.LengthMM = fishLengthMM;
            fish.SpriteInstanceTexture = (Texture2D)GlobalContent.GetFile(fishTypeName);
            fish.SpriteInstanceTextureScale = Math.Max(0.1f, (float)Decimal.Divide(fishLengthMM, 1000));
            fish.IsSmall = fish.SpriteInstanceTextureScale < 0.2;

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
