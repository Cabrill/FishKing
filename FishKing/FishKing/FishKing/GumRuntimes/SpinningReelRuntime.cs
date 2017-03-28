using RenderingLibrary;
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

        int handleRotationRate = -3;
        int foregroundRotationRate = 4;
        int backgroundRotationRate = -5;

        float spinVelocity = 0;
        float velocityIncrementRate = 0.3f;
        float maxVelocity = 4;
        float velocityAttritionRate = 0.1f;
        
        public void SpinReel()
        {
            spinVelocity = Math.Min(maxVelocity, spinVelocity + velocityIncrementRate);
            if (JammingAnimation.IsPlaying())
            {
                JammingAnimation.Stop();
            }
        }

        public void JamReel()
        {
            spinVelocity = Math.Max(0, spinVelocity - (velocityAttritionRate * 3));
            if (!JammingAnimation.IsPlaying())
            {
                JammingAnimation.Play();
            }
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
            //Maintaing dimension ratios of a square
            IPositionedSizedObject spinningReel = this as IPositionedSizedObject;
            this.Height = spinningReel.Width;

            spinVelocity = 0;
            ReelHandleRotation = ReelForegroundRotation = ReelBackgroundRotation = 0;
        }
    }
}
