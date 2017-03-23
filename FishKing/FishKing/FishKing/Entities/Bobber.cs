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
using FlatRedBall.Math.Splines;

namespace FishKing.Entities
{
    public partial class Bobber
	{
        SoundEffectInstance bobberSoundInstance;
        AudioListener listener;
        AudioEmitter emitter;

        public bool IsMoving
        {
            get; private set;
        }

        private double splineTime = 0.1;

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

            FishingLineSpline.PathColor = Color.White;
            FishingLineSpline.SplinePointRadiusInPixels = 1;
            
            IsMoving = false;
        }

		private void CustomActivity()
		{
            if (IsMoving)
            {
                splineTime += 0.1;
                var splinePoint = new SplinePoint(this.X, this.Y, 2, splineTime);
                FishingLineSpline.Add(splinePoint);
                FishingLineSpline.UpdateShapes();
            }
            FishingLineSpline.Visible = this.Visible;
        }

		private void CustomDestroy()
		{

		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }

        public void AnimateTo(Vector3 relativeDestination, int tileSize)
        {
            IsMoving = true;
            this.RelativePosition = Vector3.Zero;
            this.Visible = true;
            FishingLineSpline.Clear();
            FishingLineSpline.Visible = true;
            splineTime = 0.1;

            Tweener distanceTweener;
            Tweener verticalTweener;
            double tweenDuration = 0.75;

            var isMovingHorizontal = relativeDestination.X != this.RelativeX;
            if (isMovingHorizontal)
            {
                this.RelativeY += tileSize * 2;
                if (relativeDestination.X > this.RelativeX)
                {
                    this.RelativeX += tileSize*2.5f;
                    
                }
                else
                {
                    this.RelativeX -= tileSize*2.5f;
                }
                distanceTweener = this.Tween("RelativeX").To(relativeDestination.X).During(tweenDuration).Using(InterpolationType.Sinusoidal, Easing.Out);

                verticalTweener = this.Tween("RelativeY").To(RelativeY*1.5f).During(tweenDuration/2).Using(InterpolationType.Sinusoidal, Easing.Out);
                verticalTweener.Ended += () => {
                    this.Tween("RelativeY").To(0).During(tweenDuration / 2).Using(InterpolationType.Quadratic, Easing.In).Start();
                };
            }
            else
            {
                var castingUp = relativeDestination.Y > this.RelativeY;

                if (castingUp)
                {
                    this.RelativeY += tileSize*2.4f;
                    this.RelativeX += tileSize*0.4f;
                }

                distanceTweener = this.Tween("RelativeY").To(relativeDestination.Y).During(tweenDuration).Using(InterpolationType.Sinusoidal, Easing.Out);

                if (castingUp)
                {
                    this.Tween("RelativeX").To(relativeDestination.X - tileSize*0.5f).During(tweenDuration).Using(InterpolationType.Linear, Easing.InOut).Start();
                }

                var currentScale = this.BobberSpriteInstance.TextureScale;
                var newScale = currentScale * 1.5f;

                verticalTweener = this.BobberSpriteInstance.Tween("TextureScale").To(newScale).During(tweenDuration / 2).Using(InterpolationType.Sinusoidal, Easing.Out);
                verticalTweener.Ended += () => {
                    this.BobberSpriteInstance.Tween("TextureScale").To(currentScale).During(tweenDuration / 2).Using(InterpolationType.Quadratic, Easing.In).Start();
                };
            }

            //Determine positional sound of bobber when it hits
            emitter.Position = new Vector3(relativeDestination.X / tileSize, relativeDestination.Y / tileSize, 0);
            bobberSoundInstance.Apply3D(listener, emitter);

            //Start movement
            distanceTweener.Ended += () => {
                bobberSoundInstance.Play();
                IsMoving = false;
            };
            distanceTweener.Start();
            verticalTweener.Start();
        }
	}
}
