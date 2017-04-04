using FishKing.Entities;
using FlatRedBall.Glue.StateInterpolation;
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
        }

        public void RaiseAlignmentBar()
        {
            FramedCatchingBackgroundInstance.RaiseAlignmentBar();
        }

        public void SpinReel()
        {
            if (FramedCatchingBackgroundInstance.IsAligned)
            {
                SpinningReelInstance.SpinReel();
            }
            else
            {
                SpinningReelInstance.JamReel();
                FishingLineStatusInstance.IncreaseStress();
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
        }
    }
}
