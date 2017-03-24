using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.GumRuntimes
{
    partial class SpinningReelRuntime
    {
        int handleRotationRate = 1;
        int foregroundRotationRate = 2;
        int backgroundRotationRate = -4;


        public void Spin()
        {
            ReelHandleRotation = (ReelHandleRotation + handleRotationRate) % 360;
            ReelForegroundRotation = (ReelForegroundRotation + foregroundRotationRate) % 360;
            ReelBackgroundRotation = (ReelBackgroundRotation + backgroundRotationRate) % 360;
        }
    }
}
