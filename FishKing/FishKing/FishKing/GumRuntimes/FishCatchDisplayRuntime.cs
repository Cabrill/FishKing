using FishKing.Entities;
using FishKing.Managers;
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
        public void ShowFish(Fish fish, bool newCatch)
        {
            var ipso = this as IPositionedSizedObject;
            this.Height = 1.2f * ipso.Width;

            int maxFishTypeLength = GlobalContent.Fish_Types[fish.FishType.Name].MaxMM;
            int maxFishTypeGrams = GlobalContent.Fish_Types[fish.FishType.Name].MaxGrams;

            float lengthScale = (float)decimal.Divide(fish.LengthMM, maxFishTypeLength);
            float weightScale = (float)decimal.Divide(fish.Grams, maxFishTypeGrams);

            LengthWithScale.InterpolateBetween(CatchStatWithScaleRuntime.StatQuality.Poor, CatchStatWithScaleRuntime.StatQuality.Best, lengthScale);
            WeightWithScale.InterpolateBetween(CatchStatWithScaleRuntime.StatQuality.Poor, CatchStatWithScaleRuntime.StatQuality.Best, weightScale);

            LengthWithScale.StatValueText = fish.LengthDisplay;
            WeightWithScale.StatValueText = fish.WeightDisplay;

            TextFishName.Text = fish.Name;

            var textureRow = fish.FishType.Row;
            var textureCol = fish.FishType.Col;
            var textureHeight = 64;
            var textureWidth = 128;
            FishSprite.TextureLeft = textureCol * textureWidth;
            FishSprite.TextureTop = textureRow * textureHeight;
            FishSprite.TextureWidth = textureWidth;
            FishSprite.TextureHeight = textureHeight;

            if (TournamentManager.CurrentTournament.DoesFishMeetRequirements(fish))
            {
                CurrentRequirementsState = Requirements.Met;

                int maxFishTypePoints = GlobalContent.Fish_Types[fish.FishType.Name].MaxPoints;
                float pointScale = (float)decimal.Divide(fish.Points, maxFishTypePoints);

                StarWithScale.InterpolateBetween(CatchStatWithScaleRuntime.StatQuality.Poor, CatchStatWithScaleRuntime.StatQuality.Best, pointScale);
                StarWithScale.StatValueText = fish.Points.ToString();
            }
            else
            {
                if (!TournamentManager.CurrentTournament.TournamentRules.IsFishRightType(fish))
                {
                    CurrentRequirementsState = Requirements.TypeNotMet;
                }
                else if (!TournamentManager.CurrentTournament.TournamentRules.IsFishHeavyEnough(fish))
                {
                    CurrentRequirementsState = Requirements.WeightNotMet;
                }
                else if (!TournamentManager.CurrentTournament.TournamentRules.IsFishLongEnough(fish))
                {
                    CurrentRequirementsState = Requirements.LengthNotMet;
                }
            }

            if (newCatch)
            {
                CurrentPriorCatchState = PriorCatch.New;
                NewCatchAnimation.Play();
            }
            else
            {
                CurrentPriorCatchState = PriorCatch.NotNew;
                NewCatchAnimation.Stop();
            }
        }
    }
}
