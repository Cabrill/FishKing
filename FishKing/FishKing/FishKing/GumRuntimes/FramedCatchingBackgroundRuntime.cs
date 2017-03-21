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
        private const int MaxFishWidth = 70;
        private const int MinFishWidth = 15;
        private bool fishIsMoving = false;
        private Tweener fishTweener;

        private int MaxY
        {
            get { return 100 - (int)AlignmentBarInstance.Height; }
        }

        private int AlignmentTop
        {
            get { return (int)AlignmentBarInstance.Y; }
        }

        private int AlignmentBottom
        {
            get { return AlignmentTop + (int)AlignmentBarInstance.Height; }
        }

        private int FishTop
        {
            get { return (int)UnknownFishInstance.Y; }
        }

        private int FishBottom
        {
            get { return FishTop + (int)UnknownFishInstance.Height; }
        }

        private int FishMiddle
        {
            get { return (int)((FishTop + FishBottom) / 2); }
        }

        private int FishFight
        {
            get; set;
        }

        private int FishSpeed
        {
            get; set;
        }

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
            AlignmentBarInstance.Y = 90;
        }

        public void AttachFish(Fish fish)
        {
            FishFight = GlobalContent.Fish_Types[fish.FishType.Name].Fight;
            FishSpeed = GlobalContent.Fish_Types[fish.FishType.Name].Speed;


            UnknownFishInstance.Width = DetermineFishWidth(fish.LengthMM);
            UnknownFishInstance.Height = UnknownFishInstance.Width / 3.5f;
        }

        private float DetermineFishWidth(int fishLengthMM)
        {
            var minLength = FishGenerator.MinimumLengthMM;
            var fishLengthRange = FishGenerator.MaximumLengthMM - minLength;
            var normalizedLength = Decimal.Divide((fishLengthMM - minLength), fishLengthRange);

            var fishWidth = (float)(normalizedLength * MaxFishWidth);
            fishWidth = Math.Max(MinFishWidth, fishWidth);

            return fishWidth;
        }

        public void MoveFish()
        {
            if (!fishIsMoving)
            {
                fishIsMoving = true;

                var mockY = UnknownFishInstance.Y - 25;
                var mockX = UnknownFishInstance.X + 10;

                var tweenHolder = new TweenerHolder();
                tweenHolder.Caller = UnknownFishInstance;

                fishTweener = tweenHolder.Tween("Y").To(mockY).During(3).Using(FlatRedBall.Glue.StateInterpolation.InterpolationType.Cubic, FlatRedBall.Glue.StateInterpolation.Easing.InOut);
                tweenHolder.Tween("X").To(mockX).During(2).Using(FlatRedBall.Glue.StateInterpolation.InterpolationType.Cubic, FlatRedBall.Glue.StateInterpolation.Easing.InOut);
                fishTweener.Ended += () => { fishIsMoving = false; };
            }
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
            if (AlignmentBarInstance.Y < MaxY)
            {
                AlignmentBarInstance.Y += 1;
            }
            if (AlignmentBarInstance.Y > MaxY)
            {
                AlignmentBarInstance.Y = MaxY;
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
                AlignmentBarInstance.CurrentAlignmentState = AlignmentBarRuntime.Alignment.Aligned;
            }
            else
            {
                AlignmentBarInstance.CurrentAlignmentState = AlignmentBarRuntime.Alignment.NotAligned;
            }
        }
    }
}
