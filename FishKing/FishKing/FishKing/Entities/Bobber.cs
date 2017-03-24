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
        int fishLinePointRate = 4;
        int fishLinePointCounter = 0;

        public bool IsMoving
        {
            get; private set;
        }
        private bool wasMovingLastUpdate = false;

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

            IsMoving = false;
        }

        /// <summary>
        /// Update the fishing line as the bobber moves through the air, and hide the splash after it has played
        /// </summary>
		private void CustomActivity()
		{
            UpdateFishingLine();

            if (WaterSplashSprite.Visible)
            {
                if (WaterSplashSprite.JustCycled)
                {
                    WaterSplashSprite.Visible = false;
                    WaterSplashSprite.IgnoreParentPosition = false;
                }
            }
            if (!Visible)
            {
                ResetFishingLine();
            }

            wasMovingLastUpdate = IsMoving;
        }

		private void CustomDestroy()
		{
            ResetFishingLine();
		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }

        private void UpdateFishingLine()
        {
            if (IsMoving)
            {
                if (fishLinePointCounter++ == 0)
                {
                    var newLine = ShapeManager.AddLine();
                    newLine.Color = Color.White;
                    newLine.Position = this.Position;
                    newLine.RelativePoint1 = new Point3D(0, 0, 0);
                    newLine.RelativePoint2 = new Point3D(0, 0, 0);

                    var linesSoFar = FishingLineLinesList.Count;
                    if (linesSoFar > 0)
                    {
                        var lastLine = FishingLineLinesList.Last;
                        lastLine.SetFromAbsoluteEndpoints(lastLine.Position, this.Position);
                    }

                    this.FishingLineLinesList.Add(newLine);
                }
                else if (fishLinePointCounter == fishLinePointRate)
                {
                    fishLinePointCounter = 0;
                }
            }
            else if (wasMovingLastUpdate)
            {
                //Update the final line's point
                var linesSoFar = FishingLineLinesList.Count;
                if (linesSoFar > 0)
                {
                    var lastLine = FishingLineLinesList.Last;
                    lastLine.SetFromAbsoluteEndpoints(lastLine.Position, this.Position);
                }
            }
        }

        /// <summary>
        /// Move the bobber to the designated location, relative to the character
        /// </summary>
        /// <param name="relativeDestination">Designated location, relative to the caller</param>
        /// <param name="tileSize">Number of pixels per tile to determine tile distance traveled</param>
        public void TraverseTo(Vector3 relativeDestination, int tileSize)
        {
            IsMoving = true;
            CurrentState = VariableState.OutOfWater;
            this.RelativePosition = Vector3.Zero;
            this.Visible = true;
            ResetFishingLine();
            splineTime = 0.1;

            Tweener distanceTweener;
            Tweener verticalTweener;
            double tweenDuration = 0.75;

            var isMovingHorizontal = relativeDestination.X != this.RelativeX;
            if (isMovingHorizontal)
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
                bobberSoundInstance.Play();
                IsMoving = false;
                CurrentState = VariableState.BobInWater;
                WaterSplashSprite.IgnoreParentPosition = true;
                WaterSplashSprite.Visible = true;
                WaterSplashSprite.CurrentFrameIndex = 0;
            }
        }

        private void ResetFishingLine()
        {
            if (FishingLineLinesList.Count > 0)
            {
                for (int i = FishingLineLinesList.Count; i > 0; i--)
                {
                    ShapeManager.Remove(FishingLineLinesList.Last);
                }
            }
        }
	}
}
