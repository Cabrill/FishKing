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
        partial void CustomInitialize()
        {
            
            
        }

        public void AttachFish(Fish fish)
        {
            FramedCatchingBackgroundInstance.Reset();
            FramedCatchingBackgroundInstance.AttachFish(fish);
        }

        public void Update()
        {
            FramedCatchingBackgroundInstance.Update();
        }

        public void RaiseAlignmentBar()
        {
            FramedCatchingBackgroundInstance.RaiseAlignmentBar();
        }
    }
}
