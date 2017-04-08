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
using FlatRedBall.Glue.StateInterpolation;
using Microsoft.Xna.Framework;
using StateInterpolationPlugin;

namespace FishKing.Entities
{
	public partial class Fish
	{
        private Random random = new Random();
        private float xAcceleration;
        private double outOfWaterTime;
        private Direction directionTraveling;
        private float originalTextureScale;
        SpriteList waterDropParticles = new SpriteList();
        float minWaterDropHitY;
        float maxWaterDropHitY;
        Tweener distanceTweener;
        Tweener verticalTweener;

        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
		private void CustomInitialize()
		{
            ShadowInstance.Visible = false;
            ShadowInstance.SpriteInstanceAlpha = 0.5f;
            ShadowInstance.RelativeZ = -0.5f;
        }

		private void CustomActivity()
		{
            if (Visible)
            {
                if (Visible && ShadowInstance.Visible)
                {
                    if (directionTraveling == Direction.Left || directionTraveling == Direction.Right)
                    {
                        ShadowInstance.RelativeY = -8 + -RelativeY;
                        ShadowInstance.SpriteInstanceWidth = this.SpriteInstance.Width * (1 - (RelativeY + 8) / 64);
                        ShadowInstance.SpriteInstanceAlpha = 0.5f * (1 - (RelativeY + 8) / 50);
                    }
                    else
                    {
                        ShadowInstance.RelativeY = (-20 * (1 - (originalTextureScale / SpriteInstanceTextureScale)));
                        ShadowInstance.SpriteInstanceWidth = this.SpriteInstance.Width * (originalTextureScale / SpriteInstanceTextureScale);
                        ShadowInstance.SpriteInstanceAlpha = 0.5f * (originalTextureScale / SpriteInstanceTextureScale);
                    }
                }
                this.WaterDropEmitter.TimedEmit(waterDropParticles);
            }
            if (waterDropParticles.Count > 0)
            {
                UpdateWaterParticles();
            }
		}

        private void UpdateWaterParticles()
        {
            if (distanceTweener.Running)
            {
                WaterDropEmitter.XAcceleration = xAcceleration;
            }
            else
            {
                WaterDropEmitter.XAcceleration = 0;
            }

            var timeOutOfWater = FlatRedBall.TimeManager.CurrentTime - outOfWaterTime;

            WaterDropEmitter.SecondFrequency = (float)(2 * Math.Min(1,(timeOutOfWater / 20)));
            WaterDropEmitter.NumberPerEmission = Math.Max(0, 6 - (int)timeOutOfWater);

            Sprite sprite;
            for (int i = waterDropParticles.Count - 1; i > -1; i--)
            {
                sprite = waterDropParticles[i];
                sprite.Alpha = 1 - sprite.TextureScale;
                if (sprite.Y <= minWaterDropHitY && sprite.YVelocity < 0 && sprite.Y > maxWaterDropHitY)
                {
                    if (random.NextDouble() > 0.9)
                    {
                        sprite.YVelocity = 0;
                        sprite.CurrentChainName = "Splash";
                        sprite.Alpha = 0.3f;
                    }
                }
                else if (sprite.Y <= maxWaterDropHitY)
                {
                    sprite.YVelocity = 0;
                    sprite.CurrentChainName = "Splash";
                    sprite.Alpha = 0.3f;
                }

                if (sprite.CurrentChainName == "Splash" && sprite.JustCycled)
                {
                    SpriteManager.RemoveSprite(sprite);
                }
            }
        }


        private void CustomDestroy()
		{
            for (int i = waterDropParticles.Count - 1; i > -1; i--)
            {
                SpriteManager.RemoveSprite(waterDropParticles[i]);
            }
        }

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }

        public void PullInAndLand(Vector3 targetPosition, Direction direction, Action afterAction)
        {
            outOfWaterTime = FlatRedBall.TimeManager.CurrentTime;
            directionTraveling = direction;
            Visible = true;
            ShadowInstance.Visible = true;
            this.Position = targetPosition;
            this.RelativeZ = -0.5f;
            SetRelativeFromAbsolute();

            


            GlobalContent.SplashOut.Play();
            WaterSplashInstance.Play();

            double tweenDuration = 0;
            originalTextureScale = SpriteInstanceTextureScale;
            
            var wasCastHorizontally = direction == Direction.Left || direction == Direction.Right;
            if (wasCastHorizontally)
            {
                var destY = -8;
                SpriteInstanceFlipHorizontal = (direction == Direction.Left);
                tweenDuration = Math.Abs(RelativeX / 96);
                distanceTweener = this.Tween("RelativeX").To(0).During(tweenDuration).Using(InterpolationType.Sinusoidal, Easing.Out);
                verticalTweener = this.Tween("RelativeY").To(20).During(tweenDuration / 2).Using(InterpolationType.Quadratic, Easing.Out);
                verticalTweener.Ended += () => {
                    var lastUpdate = RelativeY;
                    var updateBeforeLast = lastUpdate;
                    var downTween = this.Tween("RelativeY").To(destY).During(tweenDuration / 2).Using(InterpolationType.Bounce, Easing.Out);
                    downTween.PositionChanged += (a) =>
                    {
                        if (a > lastUpdate && updateBeforeLast > lastUpdate)
                        {
                            GlobalContent.FishSplat.Play();
                        }
                        updateBeforeLast = lastUpdate;
                        lastUpdate = a;
                    };
                    downTween.Start();
                };
            }
            else
            {
                tweenDuration = Math.Abs(RelativeY / 96);
                distanceTweener = this.Tween("RelativeY").To(-8).During(tweenDuration).Using(InterpolationType.Sinusoidal, Easing.Out);
                this.Tween("RelativeX").To(0).During(tweenDuration).Using(InterpolationType.Linear, Easing.InOut).Start();

                var newScale = originalTextureScale * 2f;

                verticalTweener = this.Tween("SpriteInstanceTextureScale").To(newScale).During(tweenDuration / 2).Using(InterpolationType.Quadratic, Easing.Out);
                verticalTweener.Ended += () => {
                    var lastUpdate = SpriteInstanceTextureScale;
                    var updateBeforeLast = lastUpdate;
                    var downTween = this.Tween("SpriteInstanceTextureScale").To(originalTextureScale).During(tweenDuration / 2).Using(InterpolationType.Bounce, Easing.Out);

                    downTween.PositionChanged += (a) =>
                    {
                        if (a > lastUpdate && updateBeforeLast > lastUpdate)
                        {
                            GlobalContent.FishSplat.Play();
                        }
                        updateBeforeLast = lastUpdate;
                        lastUpdate = a;
                    };
                    downTween.Start();
                };
            }
            distanceTweener.Ended += () => { ShadowInstance.Visible = false; };
            distanceTweener.Ended += afterAction;
            distanceTweener.Start();
            verticalTweener.Start();

            //Set waterdrop variables
            minWaterDropHitY = this.Y;
            maxWaterDropHitY = minWaterDropHitY - SpriteInstance.Height / 2;
            xAcceleration = this.RelativeX / (float)tweenDuration;
        }
	}
}
