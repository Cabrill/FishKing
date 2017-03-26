using FishKing.Entities;
using FlatRedBall.Glue.StateInterpolation;
using Microsoft.Xna.Framework;
using RenderingLibrary;
using StateInterpolationPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.GumRuntimes
{
    partial class FramedCatchingBackgroundRuntime
    {
        private const int MaxUnknownFishSpriteWidth = 70;
        private const int MinUnknownFishSpriteWidth = 15;
        private const float UnknownFishWidthHeightRatio = 1f / 3.5f;
        private bool fishIsMoving = false;
        private Tweener fishTweener;
        private static Random randomSeed = new Random();

        private float reelInRate = 0.1f;
        public float CurrentReelSpeed { get; set; }

        public Fish AttachedFish { get; set; }
        private int FishFight { get; set; }
        private int FishSpeed { get; set; }
        public bool IsFishCaught { get { return FishTop <= 0; } }

        private float MaxAlignmentY { get; set; }
        private float MaxFishY { get; set; }
        private float MaxFishX { get { return 100 - (UnknownFishInstance.XOrigin == RenderingLibrary.Graphics.HorizontalAlignment.Left ? UnknownFishInstance.Width : 0); } }
        private float MinFishX { get { return 0 + (UnknownFishInstance.XOrigin == RenderingLibrary.Graphics.HorizontalAlignment.Left ? 0 : UnknownFishInstance.Width); } }

        private float FishTop { get { return UnknownFishInstance.Y + (UnknownFishInstance.Height/2); } }
        private float FishBottom { get { return UnknownFishInstance.Y - (UnknownFishInstance.Height / 2); } }
        private float FishMiddle { get { return UnknownFishInstance.Y; } }

        private int AlignmentTop { get { return (int)AlignmentBarInstance.Y; } }
        private int AlignmentBottom { get { return AlignmentTop + (int)AlignmentBarInstance.Height; } }
        public bool IsAligned { get { return this.AlignmentBarInstance.CurrentAlignmentState == AlignmentBarRuntime.Alignment.Aligned;  } }

        public void AttachFish(Fish fish)
        {
            Reset();
            this.AttachedFish = fish;

            SetLocalVariables();
        }

        private void SetLocalVariables()
        {
            FishFight = GlobalContent.Fish_Types[AttachedFish.FishType.Name].Fight;
            FishSpeed = GlobalContent.Fish_Types[AttachedFish.FishType.Name].Speed;

            UnknownFishInstance.XOrigin = RenderingLibrary.Graphics.HorizontalAlignment.Left;
            UnknownFishInstance.YOrigin = RenderingLibrary.Graphics.VerticalAlignment.Center;

            MaxAlignmentY = 100 - AlignmentBarInstance.Height;

            UnknownFishInstance.Width = DetermineUnknownFishWidth(AttachedFish.LengthMM);
            UnknownFishInstance.Height = UnknownFishInstance.Width * UnknownFishWidthHeightRatio;

            MaxFishY = 100 - UnknownFishInstance.Height;

            UnknownFishInstance.X = MaxFishX / 2;
            UnknownFishInstance.Y = MaxFishY;
        }

        private float DetermineUnknownFishWidth(int fishLengthMM)
        {
            var normalizedLength = Decimal.Divide(fishLengthMM, FishGenerator.MaximumLengthMM);
            var fishWidth = Math.Max(MinUnknownFishSpriteWidth,
                (float)(normalizedLength * MaxUnknownFishSpriteWidth));

            return fishWidth;
        }

        public void Update()
        {
            if (Visible)
            {
                AnimateUnknownFish();
                if (!fishIsMoving && CurrentReelSpeed == 0)
                {
                    MoveFish();
                }

                LowerAlignmentBar();
                UpdateAlignmentBarStatus();
                ReelInFish();
                UpdateFishingLine();                
            }
        }

        public void ReelInFish()
        {
            if (CurrentReelSpeed > 0)
            {
                if (TweenerManager.Self.IsObjectReferencedByTweeners(UnknownFishInstance))
                {
                    TweenerManager.Self.StopAllTweenersOwnedBy(UnknownFishInstance);
                    fishIsMoving = false;
                }
                if (UnknownFishInstance.FlipHorizontal)
                {
                    FlipFishHorizontally();
                }
            }
            var rotationFlip = UnknownFishInstance.FlipHorizontal ? 1 : -1;
            UnknownFishInstance.Y -= reelInRate * CurrentReelSpeed;
            UnknownFishInstance.Rotation = 45 * rotationFlip * (CurrentReelSpeed / 3);
        }

        private void MoveFish()
        {
            fishIsMoving = true;

            var tweenHolder = new TweenerHolder();
            tweenHolder.Caller = UnknownFishInstance;

            double tweenDuration = (double)FishSpeed / 4;
            double chanceOfFight = (double)FishFight / 200;
            double chanceOfLuck = .9 - chanceOfFight;

            float destX = 0f;
            float destY = 0f;

            var fishIsOnRightHalf = UnknownFishInstance.X > (MaxFishX/2);
            FlipFishHorizontally();

            if (fishIsOnRightHalf)
            {   
                destX = randomSeed.Next((int)MinFishX, (int)UnknownFishInstance.X);
            }
            else
            {
                destX = randomSeed.Next((int)UnknownFishInstance.X, (int)MaxFishX);
            }

            fishTweener = tweenHolder.Tween("X").To(destX).During(tweenDuration).Using(FlatRedBall.Glue.StateInterpolation.InterpolationType.Cubic, FlatRedBall.Glue.StateInterpolation.Easing.InOut);

            var fightOrLuck = randomSeed.NextDouble();

            //if (chanceOfFight > fightOrLuck)
            //{
            //    destY = UnknownFishInstance.Y + (float)(randomSeed.Next(5, 15));
            //    destY = Math.Min(destY, MaxFishY);
            //    tweenHolder.Tween("Y").To(destY).During(tweenDuration).Using(FlatRedBall.Glue.StateInterpolation.InterpolationType.Cubic, FlatRedBall.Glue.StateInterpolation.Easing.InOut);
            //}
            //else if (chanceOfLuck > fightOrLuck)
            //{
            //    destY = UnknownFishInstance.Y - (float)(randomSeed.Next(5, 15));
            //    destY = Math.Max(destY, 0);
            //    tweenHolder.Tween("Y").To(destY).During(tweenDuration).Using(FlatRedBall.Glue.StateInterpolation.InterpolationType.Cubic, FlatRedBall.Glue.StateInterpolation.Easing.InOut);
            //}

            fishTweener.Ended += () => { fishIsMoving = false; };
        }

        private void FlipFishHorizontally()
        {
            UnknownFishInstance.FlipHorizontal = !UnknownFishInstance.FlipHorizontal;

            if (UnknownFishInstance.FlipHorizontal)
            {
                UnknownFishInstance.X += UnknownFishInstance.Width;
                UnknownFishInstance.XOrigin = RenderingLibrary.Graphics.HorizontalAlignment.Right;
            }
            else
            {
                UnknownFishInstance.X -= UnknownFishInstance.Width;
                UnknownFishInstance.XOrigin = RenderingLibrary.Graphics.HorizontalAlignment.Left;
            }
        }

        private void AnimateUnknownFish()
        {
            if (Visible)
            {
                if (!UnknownFishInstance.UnknownFishSwimAnimation.IsPlaying())
                {
                    UnknownFishInstance.UnknownFishSwimAnimation.Play();
                }
            }
        }

        private void UpdateFishingLine()
        {
            var fishAsPositionedSizedObject = UnknownFishInstance as IPositionedSizedObject;
            var x1 = UnknownFishInstance.AbsoluteX;
            var x2 = WaterBoxFishingLine.AbsoluteX;
            var y1 = UnknownFishInstance.AbsoluteY;
            var y2 = WaterBoxFishingLine.AbsoluteY;

            if (UnknownFishInstance.FlipHorizontal)
            {
                x1 -= fishAsPositionedSizedObject.Width * 0.15f;
            }
            else
            {
                x1 += fishAsPositionedSizedObject.Width * 0.15f;
            }

            var fishingLineAngle = Math.PI - Math.Atan2(y2 - y1, x2 - x1);
            var fishingLineDegrees = MathHelper.ToDegrees((float)fishingLineAngle);
            WaterBoxFishingLine.Rotation = 90 + fishingLineDegrees;

            var fishingLineLength = (float)Math.Sqrt(((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)));
            WaterBoxFishingLine.Height = fishingLineLength;
        }

        public void LowerAlignmentBar()
        {
            if (AlignmentBarInstance.Y < MaxAlignmentY)
            {
                AlignmentBarInstance.Y += 1;
            }
            if (AlignmentBarInstance.Y > MaxAlignmentY)
            {
                AlignmentBarInstance.Y = MaxAlignmentY;
            }
        }

        public void RaiseAlignmentBar()
        {
            if (AlignmentBarInstance.Y > 0)
            {
                this.AlignmentBarInstance.Y -= 2;
            }

            if (AlignmentBarInstance.Y < 0)
            {
                AlignmentBarInstance.Y = 0;
            }
        }

        private void UpdateAlignmentBarStatus()
        {
            if (AlignmentTop < FishMiddle && AlignmentBottom > FishMiddle)
            {
                this.AlignmentBarInstance.CurrentAlignmentState = AlignmentBarRuntime.Alignment.Aligned;
            }
            else
            {
                this.AlignmentBarInstance.CurrentAlignmentState = AlignmentBarRuntime.Alignment.NotAligned;
            }
        }

        public void Reset()
        {
            AttachedFish = null;
            AlignmentBarInstance.Y = 90;
        }
    }
}
