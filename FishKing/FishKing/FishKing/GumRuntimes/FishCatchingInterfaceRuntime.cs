using FishKing.Entities;
using FlatRedBall.Glue.StateInterpolation;
using Microsoft.Xna.Framework.Audio;
using StateInterpolationPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FishKing.Enums.WaterTypes;

namespace FishKing.GumRuntimes
{
    public partial class FishCatchingInterfaceRuntime
    {
        SoundEffectInstance underwaterSound;
        public bool HasAttachedFish { get { return FramedCatchingBackgroundInstance.AttachedFish != null; } }
        public bool FishHasEscaped { get { return FramedCatchingBackgroundInstance.FishHasEscaped; } }
        public bool LineHasSnapped { get { return FishingLineStatusInstance.LineHasSnapped;  } }
        public bool FishIsCaught { get { return (
#if DEBUG
                DebuggingVariables.ImmediatelyCatchFish ||
#endif
                FramedCatchingBackgroundInstance.IsFishCaught); } }


        partial void CustomInitialize()
        {
            underwaterSound = GlobalContent.UnderwaterSound.CreateInstance();
            underwaterSound.IsLooped = true;
            underwaterSound.Volume = 0.25f;
        }

        public void AttachFish(Fish fish, WaterType waterType)
        {
            Reset();
            FramedCatchingBackgroundInstance.AttachFish(fish);
            FramedCatchingBackgroundInstance.SetBackgroundFrom(waterType);
        }

        public void Update()
        {
            if (Visible)
            {
                if (underwaterSound.State != SoundState.Playing)
                {
                    underwaterSound.Play();
                }
                SpinningReelInstance.Update();
                FishingLineStatusInstance.Update();

                if (FishingLineStatusInstance.LineHasSnapped)
                {
                    FramedCatchingBackgroundInstance.LineHasSnapped = true;
                }
                else
                {
                    FramedCatchingBackgroundInstance.CurrentReelSpeed = SpinningReelInstance.ReelSpeed;
                }
                FramedCatchingBackgroundInstance.Update();

                if (LineHasSnapped || FishIsCaught)
                {
                    SpinningReelInstance.Stop();
                }
#if DEBUG
                if (DebuggingVariables.FishShouldImmediatelyEscape) FishingLineStatusInstance.LineStress = 200;
#endif
            }
            else
            {
                Stop();
            }
        }

        public void RaiseAlignmentBar(float percentPressure = 1f)
        {
            FramedCatchingBackgroundInstance.RaiseAlignmentBar(percentPressure);
        }

        public void SpinReel(float percentPressure = 1f)
        {
            if (!LineHasSnapped && !FishIsCaught)
            {
                if (FramedCatchingBackgroundInstance.IsAligned)
                {
                    SpinningReelInstance.SpinReel(percentPressure);
                }
                else
                {
                    SpinningReelInstance.JamReel();
                    FishingLineStatusInstance.IncreaseStress();
                }
            }
        }

        public void Reset()
        {
            FishingLineStatusInstance.Reset();
            FramedCatchingBackgroundInstance.Reset();
            SpinningReelInstance.Reset();
        }

        public void Stop()
        {
            SpinningReelInstance.Stop();
            underwaterSound.Stop();
        }
    }
}
