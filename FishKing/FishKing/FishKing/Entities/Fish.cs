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
        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
		private void CustomInitialize()
		{

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

        public void PullInAndLand(Vector3 targetPosition, Direction direction, Action afterAction)
        {
            Visible = true;
            this.Position = targetPosition;
            SetRelativeFromAbsolute();
                        
            //FishOnTheLine.WaterSplashInstance.Position = TargetPosition;
            GlobalContent.SplashOut.Play();
            WaterSplashInstance.Play();

            Tweener distanceTweener;
            Tweener verticalTweener;
            double tweenDuration = 0;
            var currentScale = SpriteInstanceTextureScale;

            var wasCastHorizontally = direction == Direction.Left || direction == Direction.Right;
            if (wasCastHorizontally)
            {
                var destY = -SpriteInstance.Height / 16;
                SpriteInstanceFlipHorizontal = (direction == Direction.Left);
                tweenDuration = Math.Abs(RelativeX / 112);
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
                distanceTweener = this.Tween("RelativeY").To(-SpriteInstance.Height / 16).During(tweenDuration).Using(InterpolationType.Sinusoidal, Easing.Out);
                this.Tween("RelativeX").To(0).During(tweenDuration).Using(InterpolationType.Linear, Easing.InOut).Start();

                var newScale = currentScale * 1.5f;

                verticalTweener = this.Tween("SpriteInstanceTextureScale").To(newScale).During(tweenDuration / 2).Using(InterpolationType.Quadratic, Easing.Out);
                verticalTweener.Ended += () => {
                    var lastUpdate = SpriteInstanceTextureScale;
                    var updateBeforeLast = lastUpdate;
                    var downTween = this.Tween("SpriteInstanceTextureScale").To(currentScale).During(tweenDuration / 2).Using(InterpolationType.Bounce, Easing.In);

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
            distanceTweener.Ended += afterAction;
            distanceTweener.Start();
            verticalTweener.Start();
        }
	}
}
