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
using FlatRedBall.Math.Geometry;
using System.Collections.Generic;
using FishKing.UtilityClasses;

namespace FishKing.Entities
{
	public partial class Fish
	{
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

            SpriteInstance.TopTexturePixel = textureRow * textureHeight;
            SpriteInstance.BottomTexturePixel = (textureRow + 1) * textureHeight;
            SpriteInstance.LeftTexturePixel = textureCol * textureWidth;
            SpriteInstance.RightTexturePixel = (textureCol + 1) * textureWidth;

            var randomSeed = RandomNumbers.Random;

            double gramsPerMM = (double)decimal.Divide(fishType.MaxGrams, fishType.MaxMM);

            var randomBonus = randomSeed.NextDouble() * ((int)FishGenerator.drng.GetDistributedRandomValue(randomSeed.NextDouble()) / 5f);

            var fishLengthMM = (int)(fishType.AvgMM + (randomBonus *  (fishType.MaxMM- fishType.AvgMM)));

            randomBonus = randomSeed.NextDouble() * ((int)FishGenerator.drng.GetDistributedRandomValue(randomSeed.NextDouble()) / 5f);

            var fishGrams = (int)Math.Min(fishType.MaxGrams, ((randomBonus * (maxGramBonus - minGramBonus)) + minGramBonus) * (fishLengthMM * gramsPerMM));

            var minPoints = 1;
            var maxPoints = fishType.MaxPoints;
            var pointBonus = (((double)fishLengthMM / (double)fishType.MaxMM) + ((double)fishGrams / fishType.MaxGrams)) / 2;

            var fishPoints = (int)(fishType.MaxPoints * (0.15 + pointBonus));
            fishPoints = Math.Min(fishPoints, maxPoints);
            fishPoints = Math.Max(fishPoints, minPoints);

            fish.Name = fishType.Name;
            fish.Grams = fishGrams;
            fish.WeightDisplay = InfoToString.Weight(fishGrams);
            fish.LengthMM = fishLengthMM;
            fish.LengthDisplay = InfoToString.Length(fishLengthMM);
            fish.Points = fishPoints;
            SpriteInstance.TextureScale = Math.Max(0.1f, (float)Decimal.Divide(fishLengthMM, 3000));
            fish.IsSmall = SpriteInstance.TextureScale < 0.15;

            if (fishType.Name == "Swordfish" || fishType.Name == "Sailfish")
            {
                ShadowInstance.SpriteInstanceWidth = this.SpriteInstance.Width * 0.6f;
                ShadowInstance.SpriteInstanceHeight *= 0.5f;
            }
            else
            {
                ShadowInstance.SpriteInstanceWidth = this.SpriteInstance.Width;
            }

            WaterDropEmitter.EmissionSettings.TextureScale = Math.Min(SpriteInstance.TextureScale, 0.8f);
            WaterDropEmitter.ScaleX = SpriteInstance.Width / 3;
            WaterDropEmitter.ScaleY = SpriteInstance.Height / 6;
            
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
