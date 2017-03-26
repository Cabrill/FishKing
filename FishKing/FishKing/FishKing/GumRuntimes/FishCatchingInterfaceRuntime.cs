using FishKing.Entities;
using FlatRedBall.Glue.StateInterpolation;
using StateInterpolationPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.GumRuntimes
{
    public partial class FishCatchingInterfaceRuntime
    {
        public bool HasAttachedFish { get { return FramedCatchingBackgroundInstance.AttachedFish != null; } }
        public bool IsFishCaught {  get {
                return (
#if DEBUG
                    DebuggingVariables.ImmediatelyCatchFish ||
#endif
                FramedCatchingBackgroundInstance.IsFishCaught);
            } }
        

        partial void CustomInitialize()
        {
            
            
        }

        public void AttachFish(Fish fish)
        {
            Reset();
            FramedCatchingBackgroundInstance.AttachFish(fish);
        }

        public void Update()
        {
            if (Visible)
            {
                SpinningReelInstance.Update();
                FishingLineStatusInstance.Update();
                if (!FishingLineStatusInstance.LineHasSnapped)
                {
                    FramedCatchingBackgroundInstance.CurrentReelSpeed = SpinningReelInstance.ReelSpeed;
                    FramedCatchingBackgroundInstance.Update();
                }
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
                SpinningReelInstance.Spin();
            }
            else
            {
                FishingLineStatusInstance.IncreaseStress();
            }
        }

        public void Reset()
        {
            FishingLineStatusInstance.Reset();
            FramedCatchingBackgroundInstance.Reset();
            SpinningReelInstance.Reset();
        }
    }
}
