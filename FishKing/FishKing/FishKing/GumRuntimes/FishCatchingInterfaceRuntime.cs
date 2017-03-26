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
            FramedCatchingBackgroundInstance.Update();
            FishingLineStatusInstance.Update();
        }

        public void RaiseAlignmentBar()
        {
            FramedCatchingBackgroundInstance.RaiseAlignmentBar();
        }

        public void ReelIn()
        {
            if (FramedCatchingBackgroundInstance.IsAligned)
            {
                FramedCatchingBackgroundInstance.ReelInFish();
            }
            else
            {
                FishingLineStatusInstance.IncreaseStress();
            }
            FishingLineStatusInstance.Update();
        }

        private void Reset()
        {
            FishingLineStatusInstance.Reset();
            FramedCatchingBackgroundInstance.Reset();
        }
    }
}
