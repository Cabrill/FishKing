using System;
using System.Collections.Generic;
using System.Text;
using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using FlatRedBall.AI.Pathfinding;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Graphics.Particle;
using FlatRedBall.Math.Geometry;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using StateInterpolationPlugin;
using FlatRedBall.Glue.StateInterpolation;

namespace FishKing.Entities
{
    public partial class Bobber
	{
        SoundEffectInstance bobberSoundInstance;
        AudioListener listener;
        AudioEmitter emitter;

        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
		private void CustomInitialize()
		{
            bobberSoundInstance = BobberDrop.CreateInstance();

            listener = new AudioListener();
            listener.Position = Vector3.Zero;
            emitter = new AudioEmitter();
		}

		private void CustomActivity()
		{


		}

		private void CustomDestroy()
		{


		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }

        public void AnimateTo(Vector3 relativeDestination, int tileSize)
        {
            this.RelativePosition = Vector3.Zero;
            this.Visible = true;

            Tweener distanceTweener;
            Tweener verticalTweener;
            double tweenDuration = 0.75;

            var isMovingHorizontal = relativeDestination.X != this.RelativeX;
            if (isMovingHorizontal)
            {
                distanceTweener = this.Tween("RelativeX").To(relativeDestination.X).During(tweenDuration).Using(FlatRedBall.Glue.StateInterpolation.InterpolationType.Sinusoidal, FlatRedBall.Glue.StateInterpolation.Easing.Out);

                verticalTweener = this.Tween("RelativeY").To(30).During(tweenDuration/2).Using(InterpolationType.Sinusoidal, Easing.Out);
                verticalTweener.Ended += () => {
                    this.Tween("RelativeY").To(0).During(tweenDuration / 2).Using(InterpolationType.Quadratic, Easing.In).Start();
                };
            }
            else
            {
                distanceTweener = this.Tween("RelativeY").To(relativeDestination.Y).During(tweenDuration).Using(FlatRedBall.Glue.StateInterpolation.InterpolationType.Sinusoidal, FlatRedBall.Glue.StateInterpolation.Easing.Out);

                var currentScale = this.BobberSpriteInstance.TextureScale;
                var newScale = currentScale * 1.5f;

                verticalTweener = this.BobberSpriteInstance.Tween("TextureScale").To(newScale).During(tweenDuration / 2).Using(InterpolationType.Sinusoidal, Easing.Out);
                verticalTweener.Ended += () => {
                    this.BobberSpriteInstance.Tween("TextureScale").To(currentScale).During(tweenDuration / 2).Using(InterpolationType.Quadratic, Easing.In).Start();
                };
            }

            //Determine sound of bobber when it hits
            emitter.Position = new Vector3(relativeDestination.X / tileSize, relativeDestination.Y / tileSize, 0);
            bobberSoundInstance.Apply3D(listener, emitter);

            //Start movement
            distanceTweener.Ended += () => { bobberSoundInstance.Play(); };
            distanceTweener.Start();
            verticalTweener.Start();
        }

        public void Hide()
        {
            this.Visible = false;
        }
	}
}
