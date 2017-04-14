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
using FlatRedBall.Math;
using FishKing.Extensions;

namespace FishKing.Entities
{
    public partial class Bobber
	{
        private SoundEffectInstance bobberSoundInstance;
        private AudioListener listener;
        private AudioEmitter emitter;
        private float originalTextureScale;

        private bool wasCastHorizontally = false;

        public bool FrameJustChanged
        {
            get { return BobberSpriteInstance.JustChangedFrame; }
        }

        public bool IsMoving
        {
            get; private set;
        }

        public Vector3 LineOriginationPosition
        {
            get {
                UpdateDependencies(FlatRedBall.TimeManager.CurrentTime);
                return BobberSpriteInstance.GetKeyPixelPosition();
            }
        }

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

            IsMoving = false;
            ShadowInstance.SpriteInstanceWidth = this.BobberSpriteInstance.Width * 0.9f;
            ShadowInstance.SpriteInstanceAlpha = 0.5f;
        }

        /// <summary>
        /// Update the fishing line as the bobber moves through the air, and hide the splash after it has played
        /// </summary>
		private void CustomActivity()
		{
            if (Visible)
            {
                if (IsMoving)
                {
                    ShadowInstance.Visible = true;
                    ShadowInstance.RelativeY = -RelativeY;
                    if (wasCastHorizontally)
                    {
                        ShadowInstance.RelativeY = -8 + -RelativeY;
                        ShadowInstance.SpriteInstanceWidth = BobberSpriteInstance.Width * (1 - (RelativeY  / 128));
                        ShadowInstance.SpriteInstanceAlpha = 0.5f * (1 - (RelativeY / 128));
                    }
                    else
                    {
                        ShadowInstance.RelativeY = (-128 * (1 - (originalTextureScale / BobberSpriteInstance.TextureScale)));
                        ShadowInstance.SpriteInstanceWidth = BobberSpriteInstance.Width * (originalTextureScale / BobberSpriteInstance.TextureScale);
                        ShadowInstance.SpriteInstanceAlpha = 0.5f * (originalTextureScale / BobberSpriteInstance.TextureScale);
                    }
                }
                else
                {
                    ShadowInstance.Visible = false;
                }
            }
            else 
            {
                ShadowInstance.Visible = false;
                var tweenManager = TweenerManager.Self;
                if (tweenManager.IsObjectReferencedByTweeners(this))
                {
                    tweenManager.StopAllTweenersOwnedBy(this);
                }
            }
        }

		private void CustomDestroy()
		{

		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
        
        /// <summary>
        /// Move the bobber to the designated location, relative to the character
        /// </summary>
        /// <param name="relativeDestination">Designated location, relative to the caller</param>
        /// <param name="tileSize">Number of pixels per tile to determine tile distance traveled</param>
        public void TraverseTo(Vector3 relativeDestination, int tileSize)
        {
            IsMoving = true;
            originalTextureScale = BobberSpriteInstance.TextureScale;
            CurrentState = VariableState.OutOfWater;
            this.RelativePosition = Vector3.Zero;
            ShadowInstance.RelativeZ = -0.5f;
            
            this.Visible = true;

            Tweener distanceTweener;
            Tweener verticalTweener;
            double tweenDuration = 0.75;

            wasCastHorizontally = relativeDestination.X != this.RelativeX;
            if (wasCastHorizontally)
            {
                this.RelativeY += tileSize;
                if (relativeDestination.X > this.RelativeX)
                {
                    this.RelativeX += tileSize*1.3f;
                }
                else
                {
                    this.RelativeX -= tileSize*1.3f;
                }
                distanceTweener = this.Tween("RelativeX").To(relativeDestination.X).During(tweenDuration).Using(InterpolationType.Sinusoidal, Easing.Out);

                verticalTweener = this.Tween("RelativeY").To(RelativeY*1.5f).During(tweenDuration/2).Using(InterpolationType.Sinusoidal, Easing.Out);
                verticalTweener.Ended += () => {
                    this.Tween("RelativeY").To(-5).During(tweenDuration / 2).Using(InterpolationType.Quadratic, Easing.In).Start();
                };
            }
            else
            {
                var castingUp = relativeDestination.Y > this.RelativeY;

                if (castingUp)
                {
                    this.RelativeY += tileSize*1.1f;
                    this.RelativeX += tileSize*0.2f;
                }
                distanceTweener = this.Tween("RelativeY").To(relativeDestination.Y).During(tweenDuration).Using(InterpolationType.Sinusoidal, Easing.Out);

                if (castingUp)
                {
                    this.Tween("RelativeX").To(relativeDestination.X - tileSize*0.25f).During(tweenDuration).Using(InterpolationType.Linear, Easing.InOut).Start();
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
            distanceTweener.Ended += WaterTouchDown;
            distanceTweener.Start();
            verticalTweener.Start();
        }

        /// <summary>
        /// Occurs when the bobber hits the water
        /// </summary>
        private void WaterTouchDown()
        {
            if (Visible)
            {
                WaterSplashInstance.Play();
                bobberSoundInstance.Play();
                IsMoving = false;
                CurrentState = VariableState.BobInWater;
            }
        }
    }
}
