using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.GumRuntimes
{
    partial class SpinningReelRuntime
    {
        public float ReelSpeed { get { return spinVelocity; } }
        public float ReelResistance { get; set; } = 0;

        int handleRotationRate = -3;
        int foregroundRotationRate = 4;
        int backgroundRotationRate = -5;

        float spinVelocity = 0;
        float velocityIncrementRate = 0.3f;
        float maxVelocity = 4;
        float velocityAttritionRate = 0.1f;
        
        public void Spin()
        {
            spinVelocity = Math.Min(maxVelocity, spinVelocity + velocityIncrementRate);
        }

        public void Update()
        {
            spinVelocity = Math.Max(0, spinVelocity- velocityAttritionRate);

            ReelHandleRotation = (ReelHandleRotation + (handleRotationRate * spinVelocity)) % 360;
            ReelForegroundRotation = (ReelForegroundRotation + (foregroundRotationRate * spinVelocity)) % 360;
            ReelBackgroundRotation = (ReelBackgroundRotation + (backgroundRotationRate * spinVelocity)) % -360;
        }

        public void Reset()
        {
            ReelResistance = 0;
            spinVelocity = 0;
            ReelHandleRotation = ReelForegroundRotation = ReelBackgroundRotation = 0;
        }
    }
}
