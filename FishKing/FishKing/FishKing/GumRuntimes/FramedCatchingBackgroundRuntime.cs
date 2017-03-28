﻿using FishKing.Entities;
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

        private float alignmentVelocity = 0f;
        private float alignmentVelocityAttritionRate = 0.02f;
        private float alignmentVelocityIncrementRate = 0.05f;
        private float maxAlignmentVelocy = 1.3f;

        private float fightOrLuckVelocity = 0f;
        private float fightOrLuckMaxVelocity = 0.5f;
        private float fightOrLuckMinVelocity = -0.5f;
        private float fightVelocityIncrementRate = 0.08f;
        private float luckVelocityIncrementRate = 0.1f;
        private float fightOrLuckVelocityAttritionRate = 0.001f;

        private float DefaultFishingLineX;
        private float DefaultFishingLineY;

        private float reelInRate = 0.15f;
        public float CurrentReelSpeed { get; set; }

        public Fish AttachedFish { get; set; }
        private float FishFight { get; set; }
        private float FishSpeed { get; set; }
        
        private double ChanceOfFight { get; set; }
        private double ChanceOfLuck { get; set; }

        private float FightModifier { get; set; }
        private float SpeedModifier { get; set; }

        private double FishHorizontalMovementTweenDuration { get; set; }
        private TweenerHolder FishMovementTweenerHolder = new TweenerHolder();

        public bool IsFishCaught { get { return FishTop <= 0; } }
        private bool FishIsEscaping { get; set; }
        public bool FishHasEscaped { get; set; }
        public bool LineHasSnapped { get; set; }

        private float MaxAlignmentY { get; set; }
        private float MinAlignmentY { get { return 0; } }
        private float MaxFishY { get; set; }
        private float MaxFishX { get { return 100 - (UnknownFishInstance.XOrigin == RenderingLibrary.Graphics.HorizontalAlignment.Left ? UnknownFishInstance.Width : 0); } }
        private float MinFishX { get { return 0 + (UnknownFishInstance.XOrigin == RenderingLibrary.Graphics.HorizontalAlignment.Left ? 0 : UnknownFishInstance.Width); } }

        private IPositionedSizedObject FishAsPositionedSizedObject;

        private float FishTop { get { return UnknownFishInstance.Y - (UnknownFishInstance.Height / 2); } }
        private float FishBottom { get { return UnknownFishInstance.Y + (UnknownFishInstance.Height / 2); } }
        private float FishMiddle { get { return UnknownFishInstance.Y; } }

        private int AlignmentTop { get { return (int)AlignmentBarInstance.Y; } }
        private int AlignmentBottom { get { return AlignmentTop + (int)AlignmentBarInstance.Height; } }
        public bool IsAligned { get { return this.AlignmentBarInstance.CurrentAlignmentState == AlignmentBarRuntime.Alignment.Aligned; } }

        partial void CustomInitialize()
        {
            DefaultFishingLineX = WaterBoxFishingLine.X;
            DefaultFishingLineY = WaterBoxFishingLine.Y;
            FishAsPositionedSizedObject =  UnknownFishInstance as IPositionedSizedObject;
        }

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

            SpeedModifier = 0.25f + ((float)FishSpeed / 50);
            FightModifier = 1f + ((float)FishFight / 50);

            ChanceOfFight = (double)FishFight / 2000;
            ChanceOfLuck = 0.001;

            FishHorizontalMovementTweenDuration = (double)3 - (FishSpeed / 35);

            UnknownFishInstance.XOrigin = RenderingLibrary.Graphics.HorizontalAlignment.Left;
            UnknownFishInstance.YOrigin = RenderingLibrary.Graphics.VerticalAlignment.Center;

            MaxAlignmentY = 100 - AlignmentBarInstance.Height;

            UnknownFishInstance.Width = DetermineUnknownFishWidth(AttachedFish.LengthMM);
            UnknownFishInstance.Height = UnknownFishInstance.Width * UnknownFishWidthHeightRatio;

            MaxFishY = 100 - (UnknownFishInstance.Height/2);

            UnknownFishInstance.X = MaxFishX / 2;
            UnknownFishInstance.Y = MaxFishY;

            FishMovementTweenerHolder = new TweenerHolder();
            FishMovementTweenerHolder.Caller = UnknownFishInstance;
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
                if (LineHasSnapped)
                {
                    if (!FishIsEscaping)
                    {
                        ShowFishEscape();
                    }
                }
                else
                {
                    AnimateUnknownFish();
                    if (!TweenerManager.Self.IsObjectReferencedByTweeners(UnknownFishInstance))
                    {
                        fishIsMoving = false;
                    }
                    if (!fishIsMoving && CurrentReelSpeed == 0)
                    {
                        MoveFish();
                    }
                    UpdateAlignmentBarStatus();
                    UpdateFishFightOrLuck();
                    ReelInFish();
                    UpdateFishingLine();
                }
            }
        }

        private void ReelInFish()
        {
            if (CurrentReelSpeed > 0)
            {
                if (TweenerManager.Self.IsObjectReferencedByTweeners(UnknownFishInstance) && UnknownFishInstance.FlipHorizontal)
                {
                    TweenerManager.Self.StopAllTweenersOwnedBy(UnknownFishInstance);
                }
                if (UnknownFishInstance.FlipHorizontal)
                {
                    FlipFishHorizontally();
                }
            }

            float changeInY;
            if (UnknownFishInstance.Y == MaxFishY && CurrentReelSpeed == 0 && fightOrLuckVelocity > 0)
            {
                fightOrLuckVelocity = 0;
                changeInY = 0;
            }
            else
            {
                changeInY = (-reelInRate * CurrentReelSpeed) + fightOrLuckVelocity;
            }
            UnknownFishInstance.Y = Math.Min(MaxFishY, UnknownFishInstance.Y + changeInY);

            var flipModifier = UnknownFishInstance.XOrigin == RenderingLibrary.Graphics.HorizontalAlignment.Left ? 1 : -1;
            
            var newRotation = flipModifier * 45 * (changeInY / 1.3f);
            UnknownFishInstance.Rotation = newRotation;

#if DEBUG
            if (DebuggingVariables.ShowFishDebugText) UpdateDebugText(changeInY, newRotation);
#endif
        }

#if DEBUG
        private void UpdateDebugText(float changeInY, float rotation)
        {
            DebugContainer.Visible = true;
            textFishFight.Text = $"Fight: {FishFight}";
            textFishSpeed.Text = $"Speed: {FishSpeed}";
            textFishChangeInY.Text = $"ChangeY: {changeInY}";
            textFishRotation.Text = $"Rotation: {rotation}";
            textFightLuck.Text = $"FightLuck: {fightOrLuckVelocity}";
        }
#endif

    private void UpdateFishFightOrLuck()
        {
            var fightOrLuck = randomSeed.NextDouble();
            var totalModifier = SpeedModifier * FightModifier;

            if (ChanceOfFight > fightOrLuck)
            {
                fightOrLuckVelocity = Math.Min(fightOrLuckMaxVelocity, fightOrLuckVelocity + (fightVelocityIncrementRate * totalModifier));
            }
            else if (ChanceOfLuck > fightOrLuck)
            {
                fightOrLuckVelocity = Math.Max(fightOrLuckMinVelocity, fightOrLuckVelocity - (luckVelocityIncrementRate * totalModifier));
            }
            else 
            {
                if (fightOrLuckVelocity > 0)
                {
                    fightOrLuckVelocity = Math.Max(0,fightOrLuckVelocity - fightOrLuckVelocityAttritionRate);
                }
                else if (fightOrLuckVelocity < 0)
                {
                    fightOrLuckVelocity = Math.Min(0,fightOrLuckVelocity + fightOrLuckVelocityAttritionRate);
                }
            }
        }

        private void MoveFish()
        {
            float destX = 0f;
            var fishIsOnRightHalf = UnknownFishInstance.X > (MaxFishX / 2);
            var tweenDuration = FishHorizontalMovementTweenDuration;
            

            if (fishIsOnRightHalf)
            {   
                if (UnknownFishInstance.FlipHorizontal)
                {
                    FlipFishHorizontally();
                }
                destX = randomSeed.Next((int)MinFishX, (int)UnknownFishInstance.X);
                tweenDuration *= Math.Abs((UnknownFishInstance.X - destX)/50);
            }
            else
            {
                if (!UnknownFishInstance.FlipHorizontal)
                {
                    FlipFishHorizontally();
                }
                destX = randomSeed.Next((int)UnknownFishInstance.X, (int)MaxFishX);
                tweenDuration *= Math.Abs((UnknownFishInstance.X - destX)/50);
            }

            fishTweener = FishMovementTweenerHolder.Tween("X").To(destX).During(tweenDuration).Using(FlatRedBall.Glue.StateInterpolation.InterpolationType.Cubic, FlatRedBall.Glue.StateInterpolation.Easing.InOut);
            fishIsMoving = true;

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

        private void ShowFishEscape()
        {
            if (!UnknownFishInstance.FlipHorizontal)
            {
                FlipFishHorizontally();
            }
            UnknownFishInstance.Rotation = 0;
            TweenerManager.Self.StopAllTweenersOwnedBy(UnknownFishInstance);
            float destX = 150 + UnknownFishInstance.Width;
            var tweenDuration = FishHorizontalMovementTweenDuration;

            var fishTweener = FishMovementTweenerHolder.Tween("X").To(destX).During(tweenDuration).Using(FlatRedBall.Glue.StateInterpolation.InterpolationType.Quintic, FlatRedBall.Glue.StateInterpolation.Easing.Out);
            fishTweener.Ended += () => { FishHasEscaped = true; };

            //Set line to follow fish
            var lineTweenerHolder = new TweenerHolder();
            lineTweenerHolder.Caller = WaterBoxFishingLine;

            WaterBoxFishingLine.Height = WaterBoxFishingLine.Height / 4;
            WaterBoxFishingLine.Rotation += 180;
            WaterBoxFishingLine.X = UnknownFishInstance.X - (FishAsPositionedSizedObject.Width * 0.05f);
            WaterBoxFishingLine.Y = UnknownFishInstance.Y;

            tweenDuration = FishHorizontalMovementTweenDuration;

            var lineTweener = lineTweenerHolder.Tween("X").To(destX).During(tweenDuration).Using(FlatRedBall.Glue.StateInterpolation.InterpolationType.Quintic, FlatRedBall.Glue.StateInterpolation.Easing.Out);
            fishTweener.Start();
            lineTweener.Start();

            var lineRotationTweener = lineTweenerHolder.Tween("Rotation").To(260).During(tweenDuration).Using(FlatRedBall.Glue.StateInterpolation.InterpolationType.Circular, FlatRedBall.Glue.StateInterpolation.Easing.Out);
            lineRotationTweener.Start();

            FishIsEscaping = true;
        }

        private void UpdateFishingLine()
        {
            var x1 = UnknownFishInstance.AbsoluteX;
            var x2 = WaterBoxFishingLine.AbsoluteX;
            var y1 = UnknownFishInstance.AbsoluteY;
            var y2 = WaterBoxFishingLine.AbsoluteY;

            if (UnknownFishInstance.FlipHorizontal)
            {
                x1 -= FishAsPositionedSizedObject.Width * 0.15f;
            }
            else
            {
                x1 += FishAsPositionedSizedObject.Width * 0.15f;
            }

            x1 -= (UnknownFishInstance.Width) * (-UnknownFishInstance.Rotation / 180);

            var fishingLineAngle = Math.PI - Math.Atan2(y2 - y1, x2 - x1);
            var fishingLineDegrees = MathHelper.ToDegrees((float)fishingLineAngle);
            WaterBoxFishingLine.Rotation = (90 + fishingLineDegrees) % 360;

            var fishingLineLength = (float)Math.Sqrt(((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)));
            fishingLineLength += (UnknownFishInstance.Height) * (-UnknownFishInstance.Rotation / 45);
            WaterBoxFishingLine.Height = fishingLineLength;
        }

        public void RaiseAlignmentBar()
        {
            alignmentVelocity = Math.Min(maxAlignmentVelocy, alignmentVelocity + alignmentVelocityIncrementRate);
        }

        private void UpdateAlignmentBarStatus()
        {
            var newAlignementY = AlignmentBarInstance.Y - alignmentVelocity;
            newAlignementY = Math.Max(0, newAlignementY);
            newAlignementY = Math.Min(MaxAlignmentY, newAlignementY);

            AlignmentBarInstance.Y = newAlignementY;
            if (AlignmentBarInstance.Y == MaxAlignmentY)
            {
                alignmentVelocity = 0;
            }
            else if (AlignmentBarInstance.Y == MinAlignmentY)
            {
                alignmentVelocity = -alignmentVelocityIncrementRate;
            }
            else
            {
                alignmentVelocity = Math.Max(-2, alignmentVelocity - alignmentVelocityAttritionRate);
            }

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
            TweenerManager.Self.StopAllTweenersOwnedBy(UnknownFishInstance);
            TweenerManager.Self.StopAllTweenersOwnedBy(WaterBoxFishingLine);
            FishHasEscaped = false;
            FishIsEscaping = false;
            LineHasSnapped = false;
            fishIsMoving = false;
            UnknownFishInstance.FlipHorizontal = false;
            AttachedFish = null;
            AlignmentBarInstance.Y = 90;
            alignmentVelocity = 0;
            fightOrLuckVelocity = 0;
            WaterBoxFishingLine.X = DefaultFishingLineX;
            WaterBoxFishingLine.Y = DefaultFishingLineY;
        }
    }
}
