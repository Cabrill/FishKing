using System;
using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Specialized;
using FlatRedBall.Audio;
using FlatRedBall.Screens;
using FishKing.Entities;
using FishKing.Screens;
namespace FishKing.Entities
{
	public partial class Fish
	{
        static Random randomSeed = new Random();
        private static double minGramBonus = 0.8;
        private static double maxGramBonus = 1.2;

        void OnAfterFishTypeSet (object sender, EventArgs e)
        {
            var fish = this;
            var fishType = fish.FishType;

            var textureRow = fishType.Row;
            var textureCol = fishType.Col;
            var textureHeight = 64;
            var textureWidth = 128;

            SpriteInstance.TopTextureCoordinate = textureRow * textureHeight;
            SpriteInstance.BottomTextureCoordinate = (textureRow + 1) * textureHeight;
            SpriteInstance.LeftTextureCoordinate = textureCol * textureWidth;
            SpriteInstance.RightTextureCoordinate = (textureCol + 1) * textureWidth;


            double gramsPerMM = (double)decimal.Divide(fishType.MaxGrams, fishType.MaxMM);

            var fishLengthMM = randomSeed.Next(fishType.AvgMM, fishType.MaxMM + 1);

            var fishGrams = (int)Math.Min(fishType.MaxGrams, ((randomSeed.NextDouble() * (maxGramBonus - minGramBonus)) + minGramBonus) * (fishLengthMM * gramsPerMM));

            var minPoints = 1;
            var maxPoints = fishType.MaxPoints;
            var pointBonus = (((double)fishLengthMM / (double)fishType.MaxMM) + ((double)fishGrams / fishType.MaxGrams)) / 2;

            var fishPoints = (int)(fishType.MaxPoints * (0.2 + pointBonus));
            fishPoints = Math.Min(fishPoints, maxPoints);
            fishPoints = Math.Max(fishPoints, minPoints);

            fish.Name = fishType.Name;
            fish.Grams = fishGrams;
            fish.WeightDisplay = WeightToString(fishGrams);
            fish.LengthMM = fishLengthMM;
            fish.LengthDisplay = LengthToString(fishLengthMM);
            fish.Points = fishPoints;
            fish.SpriteInstanceTextureScale = Math.Max(0.1f, (float)Decimal.Divide(fishLengthMM, 1000));
            fish.IsSmall = fish.SpriteInstanceTextureScale < 0.2;
        }


        private static string WeightToString(int grams)
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

        private static string LengthToString(int mm)
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
