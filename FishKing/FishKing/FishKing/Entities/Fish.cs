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
        private Direction directionFrom;
        private float originalTextureScale;
        SpriteList waterDropParticles = new SpriteList();
        float minWaterDropHitY;
        float maxWaterDropHitY;
        Tweener distanceTweener;
        Tweener verticalTweener;
        private bool hasLanded = false;
        private Line fishingLine;
        private double timeToLive
        {
            get { return 0.5f * (1-(originalTextureScale/ SpriteInstanceTextureScale));  }
        }

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

            WaterDropEmitter.RelativeZ = -0.5f;
        }

		private void CustomActivity()
		{
            if (Visible)
            {
                if (Visible && ShadowInstance.Visible)
                {
                    if (hasLanded)
                    {
                        ShadowInstance.Detach();
                    }
                    else
                    {
                        if (directionFrom == Direction.Left || directionFrom == Direction.Right)
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
            if (distanceTweener != null && distanceTweener.Running)
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
            if (hasLanded || directionFrom == Direction.Left || directionFrom == Direction.Right)
            {
                for (int i = waterDropParticles.Count - 1; i > -1; i--)
                {
                    sprite = waterDropParticles[i];
                    if (sprite.Alpha == 1)
                    {
                        sprite.Alpha = 1 - sprite.TextureScale;
                    }
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
            else
            {
                var currentTime = FlatRedBall.TimeManager.CurrentTime;
                double timeAlive;
                double maxTimeToLive = 1;
                for (int i = waterDropParticles.Count - 1; i > -1; i--)
                {
                    sprite = waterDropParticles[i];
                    timeAlive = currentTime - sprite.TimeCreated;

                    if (sprite.Alpha == 1)
                    {
                        sprite.Alpha = 1 - sprite.TextureScale;
                    }
                    if (timeAlive >= timeToLive && sprite.YVelocity < 0 && timeAlive < maxTimeToLive)
                    {
                        if (random.NextDouble() > 0.9)
                        {
                            sprite.YVelocity = 0;
                            sprite.CurrentChainName = "Splash";
                            sprite.Alpha = 0.3f;
                        }
                    }
                    else if (timeAlive > maxTimeToLive)
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
            directionFrom = direction;
            Visible = true;
            ShadowInstance.Visible = true;
            this.Position = targetPosition;
            this.RelativeZ = -0.5f;
            SetRelativeFromAbsolute();

            //Create fishing line
            fishingLine = ShapeManager.AddLine();
            fishingLine.Color = Color.GhostWhite;
            fishingLine.Position = this.Position;
            fishingLine.AttachTo(this, true);
            switch(directionFrom)
            {
                case Direction.Left: fishingLine.RelativePoint2 = new Point3D(-RelativeX - 36, -RelativeY + 4); break;
                case Direction.Right: fishingLine.RelativePoint2 = new Point3D(RelativeX + 36, -RelativeY + 4); break;
                case Direction.Up: fishingLine.RelativePoint2 = new Point3D(-RelativeX, -RelativeY + 4); break;
                case Direction.Down: fishingLine.RelativePoint2 = new Point3D(-RelativeX, -RelativeY + 4); break;
            }

            GlobalContent.SplashOut.Play();
            WaterSplashInstance.Play();

            double tweenDuration = 0;
            originalTextureScale = SpriteInstanceTextureScale;
            
            var wasCastHorizontally = direction == Direction.Left || direction == Direction.Right;
            if (wasCastHorizontally)
            {
                if (direction == Direction.Left)
                {
                    fishingLine.RelativePoint1 = new Point3D(SpriteInstance.Width/2, 0);
                    fishingLine.RelativePoint2 = new Point3D(-RelativeX-36, -RelativeY+4);
                }
                else
                {
                    fishingLine.RelativePoint1 = new Point3D(-SpriteInstance.Width / 2, 0);
                    fishingLine.RelativePoint2 = new Point3D(-RelativeX, -RelativeY);
                }

                var destY = -8;
                SpriteInstanceFlipHorizontal = (direction == Direction.Left);
                tweenDuration = Math.Abs(RelativeX / 96);
                distanceTweener = this.Tween("RelativeX").To(0).During(tweenDuration).Using(InterpolationType.Sinusoidal, Easing.Out);
                distanceTweener.PositionChanged += (a) => {
                    var timeElapse = FlatRedBall.TimeManager.CurrentTime - outOfWaterTime;
                    if (timeElapse <= 0.4)
                    {
                        switch (directionFrom)
                        {
                            case Direction.Left: fishingLine.RelativePoint2 = new Point3D(-RelativeX - 36, -RelativeY + 4); break;
                            case Direction.Right: fishingLine.RelativePoint2 = new Point3D(-RelativeX + 36, -RelativeY + 4); break;
                        }
                    }
                    else if (timeElapse <= 0.5)
                    {
                        switch (directionFrom)
                        {
                            case Direction.Left: fishingLine.RelativePoint2 = new Point3D(-RelativeX - 36, -RelativeY + 12); break;
                            case Direction.Right: fishingLine.RelativePoint2 = new Point3D(-RelativeX + 36, -RelativeY + 12); break;
                        }
                        
                    }
                    else
                    {
                        switch (directionFrom)
                        {
                            case Direction.Left: fishingLine.RelativePoint2 = new Point3D(-RelativeX - 44, -RelativeY + 30); break;
                            case Direction.Right: fishingLine.RelativePoint2 = new Point3D(-RelativeX + 44, -RelativeY + 30); break;
                        }   
                    }
                };

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
                Position.X += SpriteInstance.Width / 2;
                SetRelativeFromAbsolute();

                fishingLine.RelativePoint1 = new Point3D(-SpriteInstance.Width / 2, SpriteInstance.Height / 2);
                if (direction == Direction.Up)
                {
                    fishingLine.RelativePoint2 = new Point3D(-RelativeX - 8, -RelativeY + 36);
                }
                else
                {
                    fishingLine.RelativePoint2 = new Point3D(-RelativeX-12, -RelativeY-20);
                }

                tweenDuration = Math.Abs(RelativeY / 96);
                distanceTweener = this.Tween("RelativeY").To(-8).During(tweenDuration).Using(InterpolationType.Sinusoidal, Easing.Out);
                distanceTweener.PositionChanged += (a) => {
                    var timeElapse = FlatRedBall.TimeManager.CurrentTime - outOfWaterTime;
                    if (timeElapse <= 0.4)
                    {
                        switch (directionFrom)
                        {
                            case Direction.Up: fishingLine.RelativePoint2 = new Point3D(-RelativeX-8, -RelativeY+36); break;
                            case Direction.Down: fishingLine.RelativePoint2 = new Point3D(-RelativeX-8, -RelativeY - 12); break;
                        }
                    }
                    else if (timeElapse <= 0.5)
                    {
                        switch (directionFrom)
                        {
                            case Direction.Up: fishingLine.RelativePoint2 = new Point3D(-RelativeX-8, -RelativeY + 36); break;
                            case Direction.Down: fishingLine.RelativePoint2 = new Point3D(-RelativeX-12, -RelativeY -15); break;
                        }

                    }
                    else
                    {
                        switch (directionFrom)
                        {
                            case Direction.Up: fishingLine.RelativePoint2 = new Point3D(-RelativeX-8, -RelativeY + 35); break;
                            case Direction.Down: fishingLine.RelativePoint2 = new Point3D(-RelativeX-8, -RelativeY -20); break;
                        }
                    }
                };
                this.Tween("RelativeX").To(0).During(tweenDuration).Using(InterpolationType.Linear, Easing.InOut).Start();

                var newScale = originalTextureScale * 2f;

                verticalTweener = this.Tween("SpriteInstanceTextureScale").To(newScale).During(tweenDuration / 2).Using(InterpolationType.Quadratic, Easing.Out);
                verticalTweener.PositionChanged += (currentScale) => {
                    fishingLine.RelativePoint1 = new Point3D(-SpriteInstance.Width / 2, 0);
                };
                verticalTweener.Ended += () => {
                    var lastUpdate = SpriteInstanceTextureScale;
                    var updateBeforeLast = lastUpdate;
                    var downTween = this.Tween("SpriteInstanceTextureScale").To(originalTextureScale).During(tweenDuration / 2).Using(InterpolationType.Bounce, Easing.Out);

                    downTween.PositionChanged += (a) =>
                    {
                        fishingLine.RelativePoint1 = new Point3D(-SpriteInstance.Width / 2, 0);
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
            distanceTweener.Ended += () => { ShapeManager.Remove(fishingLine); hasLanded = true; SetGroundLocation(); };
            distanceTweener.Ended += afterAction;
            distanceTweener.Start();
            verticalTweener.Start();

            //Set waterdrop variables
            SetGroundLocation();
            xAcceleration = -this.RelativeX / (float)tweenDuration;
        }

        private void SetGroundLocation()
        {
            minWaterDropHitY = this.Y;
            maxWaterDropHitY = minWaterDropHitY - SpriteInstance.Height / 2;
        }
	}
}
