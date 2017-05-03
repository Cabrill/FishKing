using FlatRedBall.Audio;
using Microsoft.Xna.Framework.Audio;
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
        private Random random = new Random();

        SoundEffectInstance reelSlowClick;
        SoundEffectInstance reelMediumClick;
        SoundEffectInstance reelFastClick;

        int handleRotationRate = -3;
        int foregroundRotationRate = 4;
        int backgroundRotationRate = -5;

        float spinVelocity = 0;
        float velocityIncrementRate = 0.1f;
        float maxVelocity = 4;
        float velocityAttritionRate = 0.025f;

        partial void CustomInitialize()
        {
            reelFastClick = GlobalContent.ReelFastClick.CreateInstance();
            reelSlowClick = GlobalContent.ReelSlowClick.CreateInstance();
            reelMediumClick = GlobalContent.ReelMediumClick.CreateInstance();
            reelSlowClick.IsLooped = true;
            reelMediumClick.IsLooped = true;
            reelFastClick.IsLooped = true;
        }

        
        public void SpinReel(float percent)
        {
            spinVelocity = Math.Min(maxVelocity, spinVelocity + (velocityIncrementRate*percent));
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

            PlayReelClick();
        }

        private void PlayReelClick()
        {
            var pitch = -0.030625f + (spinVelocity / (16*maxVelocity));

            if (spinVelocity < 0.5)
            {
                reelFastClick.Stop();
                reelMediumClick.Stop();
                reelSlowClick.Stop();
            }
            else if (spinVelocity < 1.5)
            {
                reelFastClick.Stop();
                reelMediumClick.Stop();

                reelSlowClick.Pitch = pitch;
                if (reelSlowClick.State != SoundState.Playing)
                {   
                    reelSlowClick.Play();
                }
            }
            else if (spinVelocity < 3)
            {
                reelSlowClick.Stop();
                reelFastClick.Stop();

                reelMediumClick.Pitch = pitch;
                if (reelMediumClick.State != SoundState.Playing)
                {
                    reelMediumClick.Play();
                }
            }
            else if (spinVelocity >= 3)
            {
                reelMediumClick.Stop();
                reelSlowClick.Stop();

                reelFastClick.Pitch = pitch;
                if (reelFastClick.State != SoundState.Playing)
                {   
                    reelFastClick.Play();
                }
            }
        }

        public void Reset()
        {
            //Maintaing dimension ratios of a square
            
            IPositionedSizedObject spinningReel = this as IPositionedSizedObject;
            this.Height = spinningReel.Width;

            spinVelocity = 0;
            ReelHandleRotation = ReelForegroundRotation = ReelBackgroundRotation = 0;
        }

        public void Stop()
        {
            reelFastClick.Stop();
            reelMediumClick.Stop();
            reelSlowClick.Stop();
        }
    }
}
