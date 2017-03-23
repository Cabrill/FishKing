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

        partial void CustomInitialize()
        {
            
            
        }

        public void AttachFish(Fish fish)
        {
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

            }
            else
            {
                FishingLineStatusInstance.LineStress += 1;
            }
            FishingLineStatusInstance.Update();
        }

        public void ResetStatus()
        {
            FishingLineStatusInstance.Reset();
            FramedCatchingBackgroundInstance.Reset();
        }
    }
}
