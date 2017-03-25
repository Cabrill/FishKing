using FishKing.Entities;
using FlatRedBall.Glue.StateInterpolation;
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

        private float MaxAlignmentY { get; set; }

        private float MaxFishY { get; set; }

        private float MaxFishX { get; set; }

        public Fish AttachedFish { get; set;  }

        private int FishTop { get { return (int)UnknownFishInstance.Y; } }

        private int FishBottom { get { return FishTop + (int)UnknownFishInstance.Height; } }

        private int FishMiddle { get { return (int)((FishTop + FishBottom) / 2); } }

        private int FishFight { get; set; }

        private int AlignmentTop { get { return (int)AlignmentBarInstance.Y; } }

        private int AlignmentBottom { get { return AlignmentTop + (int)AlignmentBarInstance.Height; } }

        private int FishSpeed { get; set; }

        public bool IsAligned { get { return this.AlignmentBarInstance.CurrentAlignmentState == AlignmentBarRuntime.Alignment.Aligned;  } }

        public void Update()
        {
            AnimateUnknownFish();
            LowerAlignmentBar();
            UpdateAlignmentBarStatus();

            if (!fishIsMoving)
            {
                MoveFish();
            }
        }

        public void Reset()
        {
            AttachedFish = null;
            AlignmentBarInstance.Y = 90;
        }

        public void AttachFish(Fish fish)
        {
            this.AttachedFish = fish;

            SetLocalVariables();
        }

        private void SetLocalVariables()
        {
            FishFight = GlobalContent.Fish_Types[AttachedFish.FishType.Name].Fight;
            FishSpeed = GlobalContent.Fish_Types[AttachedFish.FishType.Name].Speed;
            
            MaxAlignmentY = 100 - AlignmentBarInstance.Height; 

            UnknownFishInstance.Width = DetermineUnknownFishWidth(AttachedFish.LengthMM);
            UnknownFishInstance.Height = UnknownFishInstance.Width * UnknownFishWidthHeightRatio;

            MaxFishY = UnknownFishInstance.Height;
            MaxFishX = 100 - UnknownFishInstance.Width;

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

        public void MoveFish()
        {
            fishIsMoving = true;

            var tweenHolder = new TweenerHolder();
            tweenHolder.Caller = UnknownFishInstance;

            double tweenDuration = (double)FishSpeed / 4;
            double chanceOfFight = (double)FishFight / 200;
            double chanceOfLuck = .9 - chanceOfFight;

            float destX = 0f;
            float destY = 0f;

            var fishIsOnRightHalf = UnknownFishInstance.X + (UnknownFishInstance.Width/2) > 50;

            if (fishIsOnRightHalf)
            {
                destX = randomSeed.Next(0, (int)UnknownFishInstance.X);
                UnknownFishInstance.FlipHorizontal = false;
            }
            else
            {
                destX = randomSeed.Next((int)UnknownFishInstance.X + 1, (int)MaxFishX);
                UnknownFishInstance.FlipHorizontal = true;
            }

            fishTweener = tweenHolder.Tween("X").To(destX).During(tweenDuration).Using(FlatRedBall.Glue.StateInterpolation.InterpolationType.Cubic, FlatRedBall.Glue.StateInterpolation.Easing.InOut);

            var fightOrLuck = randomSeed.NextDouble();

            if (chanceOfFight > fightOrLuck)
            {
                destY = UnknownFishInstance.Y + (float)(randomSeed.Next(5, 15));
                destY = Math.Min(destY, MaxFishY);
                tweenHolder.Tween("Y").To(destY).During(tweenDuration).Using(FlatRedBall.Glue.StateInterpolation.InterpolationType.Cubic, FlatRedBall.Glue.StateInterpolation.Easing.InOut);
            }
            else if (chanceOfLuck > fightOrLuck)
            {
                destY = UnknownFishInstance.Y - (float)(randomSeed.Next(5, 15));
                destY = Math.Max(destY, 0);
                tweenHolder.Tween("Y").To(destY).During(tweenDuration).Using(FlatRedBall.Glue.StateInterpolation.InterpolationType.Cubic, FlatRedBall.Glue.StateInterpolation.Easing.InOut);
            }

            fishTweener.Ended += () => { fishIsMoving = false; };
        }

        public void AnimateUnknownFish()
        {
            if (Visible)
            {
                if (!UnknownFishInstance.UnknownFishSwimAnimation.IsPlaying())
                {
                    UnknownFishInstance.UnknownFishSwimAnimation.Play();
                }
            }
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
    }
}
