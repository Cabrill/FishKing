﻿using FishKing.Entities;
using FlatRedBall;
using RenderingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.GumRuntimes
{
    partial class FishCatchDisplayRuntime
    {
        public void ShowFish(Fish fish)
        {
            var ipso = this as IPositionedSizedObject;
            this.Height = 1.2f * ipso.Width;

            int maxFishTypeLength = GlobalContent.Fish_Types[fish.FishType.Name].MaxMM;
            int maxFishTypeGrams = GlobalContent.Fish_Types[fish.FishType.Name].MaxGrams;
            int maxFishTypePoints = GlobalContent.Fish_Types[fish.FishType.Name].MaxPoints;

            float lengthScale = (float)decimal.Divide(fish.LengthMM, maxFishTypeLength);
            float weightScale = (float)decimal.Divide(fish.Grams, maxFishTypeGrams);
            float pointScale = (float)decimal.Divide(fish.Points, maxFishTypePoints);

            //int fontSize = 18;
            //fontSize = (int)(decimal.Divide(FlatRedBallServices.GraphicsOptions.ResolutionHeight, 1280) * fontSize);

            //LengthWithScale.TextStatValueFontSize = fontSize;
            //WeightWithScale.TextStatValueFontSize = fontSize;
            //StarWithScale.TextStatValueFontSize = fontSize;
            //TextFishNameFontSize = fontSize;
            //FontSize = fontSize;

            LengthWithScale.InterpolateBetween(CatchStatWithScaleRuntime.StatQuality.Poor, CatchStatWithScaleRuntime.StatQuality.Best, lengthScale);
            WeightWithScale.InterpolateBetween(CatchStatWithScaleRuntime.StatQuality.Poor, CatchStatWithScaleRuntime.StatQuality.Best, weightScale);
            StarWithScale.InterpolateBetween(CatchStatWithScaleRuntime.StatQuality.Poor, CatchStatWithScaleRuntime.StatQuality.Best, pointScale);

            LengthWithScale.StatValueText = fish.LengthDisplay;
            WeightWithScale.StatValueText = fish.WeightDisplay;
            StarWithScale.StatValueText = fish.Points.ToString();
            TextFishName.Text = fish.Name;

            var fishType = fish.FishType;

            var textureRow = fishType.Row;
            var textureCol = fishType.Col;
            var textureHeight = 64;
            var textureWidth = 128;
            FishSprite.TextureLeft = textureCol * textureWidth;
            FishSprite.TextureTop = textureRow * textureHeight;
            FishSprite.TextureWidth = textureWidth;
            FishSprite.TextureHeight = textureHeight;

        }


    }
}
