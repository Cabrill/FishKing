#region Usings

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
using FlatRedBall.Math.Splines;
using BitmapFont = FlatRedBall.Graphics.BitmapFont;
using Cursor = FlatRedBall.Gui.Cursor;
using GuiManager = FlatRedBall.Gui.GuiManager;
using FlatRedBall.TileCollisions;
using Microsoft.Xna.Framework;
using FlatRedBall.Math;
using FlatRedBall.Glue.StateInterpolation;
using StateInterpolationPlugin;

#if FRB_XNA || SILVERLIGHT
using Keys = Microsoft.Xna.Framework.Input.Keys;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using Texture2D = Microsoft.Xna.Framework.Graphics.Texture2D;

#endif
#endregion

namespace FishKing.Entities
{
    public enum Direction
    {
        None,
        Up,
        Down,
        Left,
        Right
    }

    public partial class Character
    {
        const int tileSize = 32;
        public Direction DirectionFacing { get; private set; }
        public int MaxDistanceTileCast { get; set; }
        
        public string Dialog { get; set; }
        public string Animation { set { this.SpriteInstance.AnimationChains = GetFile(value) as AnimationChainList;} }

        public I2DInput MovementInput { get; set; }
        public IPressableInput FishingAlignmentInput { get; set; }
        public IPressableInput ActionInput { get; set; }

        private bool isMovingToTile = false;
        public bool IsMoving { get { return SpriteInstance.CurrentChainName.Substring(0, 4) == "Walk"; } }

        public bool IsAttemptingMovement { get { return MovementInput != null && (MovementInput.X != 0 || MovementInput.Y != 0 || MovementInput.XVelocity != 0 || MovementInput.YVelocity != 0); } }
        public bool IsAttemptingAction { get; private set; }
        public bool IsAttemptingReelIn { get; private set; }
        public bool IsInDialog { get; set; }

        public Vector3 TargetPosition { get; set; } 

        public bool IsHoldingAction { get; private set; }
        public bool IsHoldingAlignButton { get; private set; }

        private int WindUpAnimationFrame { get { return (DirectionFacing == Direction.Left || DirectionFacing == Direction.Right) ? 2 : 3; } }
        public bool IsOnFinalFrameOfAnimationChain { get { return SpriteInstance.CurrentFrameIndex == SpriteInstance.CurrentChain.Count - 1; } }

        public bool IsFishing { get; set; } = false;
        public bool HasFishOnTheLine { get { return FishOnTheLine != null; } }

        public bool IsCastingRod { get; set; } = false;
        public bool JustReleasedCast { get; set; } = false;
        public bool IsOnWindUp { get { return (IsCastingRod && IsHoldingAction && SpriteInstance.CurrentFrameIndex == WindUpAnimationFrame); } }
        public bool IsBeforeWindUp { get { return IsCastingRod && (SpriteInstance.CurrentFrameIndex < WindUpAnimationFrame); } }
        public bool IsAfterWindUp { get { return (IsCastingRod && SpriteInstance.CurrentFrameIndex > WindUpAnimationFrame); } }
        
        public bool HasInitiatedCatching { get; set; }
        public bool IsPullingInCatch { get { return SpriteInstance.CurrentChainName.Substring(0, 3) == "Tug"; } }
        public bool IsDisplayingCatch { get; set; } = false;
        public bool HasFinishedDisplayingCatch { get; set; } = false;

        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
        private void CustomInitialize()
        {
            InitializeCollision();
            DirectionFacing = Direction.Down;
            SpriteInstance.CurrentChainName = "StandDown";
            MaxDistanceTileCast = 3;
        }

        private void InitializeCollision()
        {
            ForwardCollision.Position = Position;
            BackwardCollision.Position = this.Position;
            UpdateActionCollision();

#if DEBUG
            ForwardCollision.Visible = BackwardCollision.Visible =
                ActionCollision.Visible = DebuggingVariables.ShowShapes;
            ForwardCollision.Color = Color.Green;
            BackwardCollision.Color = Color.Red;
#endif
        }

        /// <summary>
        /// Places the ActionCollision in front of where
        /// the character is facing. The action collision
        /// will always be in front of the BackwardCollision.
        /// </summary>
        private void UpdateActionCollision()
        {

            float desiredX = this.X;
            float desiredY = this.Y;

            MoveInDirection(DirectionFacing, ref desiredX, ref desiredY);

            ActionCollision.X = desiredX;
            ActionCollision.Y = desiredY;
        }
        
        private void CustomActivity()
		{
            IsAttemptingAction = ActionInput != null && ActionInput.WasJustPressed && isMovingToTile == false;
            IsHoldingAction = ActionInput != null && ActionInput.IsDown;

            if (IsAttemptingAction && !HasFishOnTheLine)
            {
                ResetFishingStatus();
            }
            IsHoldingAlignButton = FishingAlignmentInput != null && FishingAlignmentInput.IsDown;
        }


        public bool PerformMovementActivity(TileShapeCollection collision, PositionedObjectList<Character> characters)
        {
            var desiredDirection = GetDesiredDirection();

            if(desiredDirection == Direction.None && !isMovingToTile && SpriteInstance.CurrentChainName.Contains("Walk"))
            {
                SpriteInstance.CurrentFrameIndex = 0;
                SpriteInstance.CurrentChainName = SpriteInstance.CurrentChainName.Replace("Walk", "Stand");
            }
            else if (desiredDirection != Direction.None)
            {
                DirectionFacing = desiredDirection;
            }

            bool startedMoving = ApplyDesiredDirectionToMovement(desiredDirection, collision, characters);

            ApplyDesiredDirectionToAnimation(desiredDirection, startedMoving);

            TryUpdateActionCollision(desiredDirection, startedMoving);

            return startedMoving;
        }

        private void TryUpdateActionCollision(Direction desiredDirection, bool startedMoving)
        {
            bool shouldUpdateCollision = 
                // This means the user is facing a collision area
                (desiredDirection != Direction.None && this.isMovingToTile == false);

            if(shouldUpdateCollision)
            {
                UpdateActionCollision();
            }
        }

        private void ApplyDesiredDirectionToAnimation(Direction desiredDirection, bool startedMoving)
        {
            bool shouldFaceDirection = startedMoving ||
                // This means the user is facing a collision area
                (desiredDirection != Direction.None && this.isMovingToTile == false);
            if (shouldFaceDirection)
            {
                switch (desiredDirection)
                {
                    case Direction.Left: SpriteInstance.CurrentChainName = "WalkLeft"; break;
                    case Direction.Right: SpriteInstance.CurrentChainName = "WalkRight"; break;
                    case Direction.Up: SpriteInstance.CurrentChainName = "WalkUp"; break;
                    case Direction.Down: SpriteInstance.CurrentChainName = "WalkDown"; break;
                }
                this.SpriteInstance.Animate = isMovingToTile;
            }
            else if (!IsCastingRod && !IsFishing)
            {
                this.SpriteInstance.Animate = isMovingToTile;
            }
        }

        public void UpdateFishingStatus(bool characterMoved)
        {
            if (characterMoved)
            {
                ResetFishingStatus();
            }
            if (IsCastingRod && !IsFishing)
            {
                JustReleasedCast = RodSpriteInstance.CurrentFrameIndex == WindUpAnimationFrame + 1 && RodSpriteInstance.JustChangedFrame;

                var justStartedCasting = ActionInput.WasJustPressed;
                if (justStartedCasting)
                {
                    var chainName = "";
                    switch (DirectionFacing)
                    {
                        case Direction.Left: chainName = "CastLeft"; break;
                        case Direction.Right: chainName = "CastRight"; break;
                        case Direction.Up: chainName = "CastUp"; break;
                        case Direction.Down: chainName = "CastDown"; break;
                    }
                    SpriteInstance.CurrentChainName =  RodSpriteInstance.CurrentChainName = chainName;
                    SpriteInstance.Animate = RodSpriteInstance.Animate = true;
                    SpriteInstance.CurrentFrameIndex = RodSpriteInstance.CurrentFrameIndex = 0;
                }
                else
                {
                    var shouldHoldFrame = IsOnFinalFrameOfAnimationChain || IsOnWindUp;
                    SpriteInstance.Animate = !shouldHoldFrame;
                    RodSpriteInstance.Animate = !shouldHoldFrame;

                    if (IsOnFinalFrameOfAnimationChain)
                    {
                        IsCastingRod = false;
                        IsFishing = true;
                    }
                    else if (JustReleasedCast)
                    {
                        var relativeTargetPosition = new Vector3(TargetPosition.X - Position.X, TargetPosition.Y - Position.Y, 1);
                        BobberInstance.TraverseTo(relativeTargetPosition, tileSize);
                        
                        WhooshRod.Play();
                    }
                }
            }
            else if (IsFishing)
            {
                if (HasFishOnTheLine)
                {
                    if (HasInitiatedCatching)
                    {
                        BobberInstance.BobberSpriteInstanceAnimate = false;
                        if (IsPullingInCatch)
                        {
                            if (SpriteInstance.CurrentFrameIndex == 3 && FishOnTheLine.Visible == false) //Just tugged
                            {
                                PullInFish();
                            }
                            else if (IsOnFinalFrameOfAnimationChain)
                            {
                                SpriteInstance.Animate = false;
                                RodSpriteInstance.Animate = false;
                            }
                        }
                        if (IsDisplayingCatch && !HasFinishedDisplayingCatch)
                        {
                            DisplayCaughtFish();
                        }
                    }
                    else
                    {
                        BobberInstance.CurrentState = Bobber.VariableState.Sink;
                        HasInitiatedCatching = ActionInput.IsDown;
                    }
                }
            }
            RodSpriteInstance.Visible = (IsCastingRod || IsFishing) && !IsDisplayingCatch;
            BobberInstance.Visible = (IsAfterWindUp || IsFishing) && !IsDisplayingCatch && !IsPullingInCatch;
        }

        public void ResetFishingStatus()
        {
            if (HasFishOnTheLine)
            {
                FishOnTheLine.Visible = false;
                FishOnTheLine.RemoveFromManagers();
                FishOnTheLine.Destroy();
                FishOnTheLine = null;
            }
            IsFishing = false;
            IsCastingRod = false;
            JustReleasedCast = false;
            HasInitiatedCatching = false;
            IsDisplayingCatch = false;
            HasFinishedDisplayingCatch = false;
            SpriteInstance.CurrentChainName = SpriteInstance.CurrentChainName.Replace("Cast", "Stand");
        }

        public void StandStill()
        {
            switch (DirectionFacing)
            {
                case Direction.Left: SpriteInstance.CurrentChainName = "StandLeft"; break;
                case Direction.Right: SpriteInstance.CurrentChainName = "StandRight"; break;
                case Direction.Up: SpriteInstance.CurrentChainName = "StandUp"; break;
                case Direction.Down: SpriteInstance.CurrentChainName = "StandDown"; break;
            }
            SpriteInstance.CurrentFrameIndex = 0;
        }

        public void HandleFishCaught()
        {
            var chainName = "";
            switch (DirectionFacing)
            {
                case Direction.Left: chainName = "TugLeft"; break;
                case Direction.Right: chainName = "TugRight"; break;
                case Direction.Up: chainName = "TugUp"; break;
                case Direction.Down: chainName = "TugDown"; break;
            }
            SpriteInstance.CurrentChainName = RodSpriteInstance.CurrentChainName = chainName;
            SpriteInstance.Animate = RodSpriteInstance.Animate = true;
            SpriteInstance.CurrentFrameIndex = RodSpriteInstance.CurrentFrameIndex = 0;
        }

        private void PullInFish()
        {
            FishOnTheLine.Visible = true;
            FishOnTheLine.Position = TargetPosition;
            FishOnTheLine.AttachTo(this, true);
            //FishOnTheLine.WaterSplashInstance.Position = TargetPosition;
            GlobalContent.SplashOut.Play();
            FishOnTheLine.WaterSplashInstance.Play();

            Tweener distanceTweener;
            Tweener verticalTweener;
            double tweenDuration = 0;
            var currentScale = FishOnTheLine.SpriteInstanceTextureScale;

            var wasCastHorizontally = DirectionFacing == Direction.Left || DirectionFacing == Direction.Right;
            if (wasCastHorizontally)
            {
                var destY = -SpriteInstance.Height / 16;
                FishOnTheLine.SpriteInstanceFlipHorizontal = (DirectionFacing == Direction.Left);
                tweenDuration = Math.Abs(FishOnTheLine.RelativeX / 112);
                distanceTweener = FishOnTheLine.Tween("RelativeX").To(0).During(tweenDuration).Using(InterpolationType.Sinusoidal, Easing.Out);
                verticalTweener = FishOnTheLine.Tween("RelativeY").To(20).During(tweenDuration / 2).Using(InterpolationType.Quadratic, Easing.Out);
                verticalTweener.Ended += () => {
                    var lastUpdate = FishOnTheLine.RelativeY;
                    var updateBeforeLast = lastUpdate;
                    var downTween = FishOnTheLine.Tween("RelativeY").To(destY).During(tweenDuration / 2).Using(InterpolationType.Bounce, Easing.Out);
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
                tweenDuration = Math.Abs(FishOnTheLine.RelativeY / 96);
                distanceTweener = FishOnTheLine.Tween("RelativeY").To(-SpriteInstance.Height / 16).During(tweenDuration).Using(InterpolationType.Sinusoidal, Easing.Out);
                FishOnTheLine.Tween("RelativeX").To(0).During(tweenDuration).Using(InterpolationType.Linear, Easing.InOut).Start();

                var newScale = currentScale * 1.5f;

                verticalTweener = FishOnTheLine.Tween("SpriteInstanceTextureScale").To(newScale).During(tweenDuration / 2).Using(InterpolationType.Quadratic, Easing.Out);
                verticalTweener.Ended += () => {
                    var lastUpdate = FishOnTheLine.SpriteInstanceTextureScale;
                    var updateBeforeLast = lastUpdate;
                    var downTween = FishOnTheLine.Tween("SpriteInstanceTextureScale").To(currentScale).During(tweenDuration / 2).Using(InterpolationType.Bounce, Easing.In);

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
            distanceTweener.Ended += DisplayCaughtFish;
            distanceTweener.Start();
            verticalTweener.Start();
        }

        private void DisplayCaughtFish()
        {
            if (!IsDisplayingCatch)
            {
                IsDisplayingCatch = true;
                RodSpriteInstance.Visible = false;
                if (FishOnTheLine.IsSmall)
                {
                    SpriteInstance.CurrentChainName = "ShowSmallCatch";
                }
                else
                {
                    SpriteInstance.CurrentChainName = "ShowCatch";
                }
                SpriteInstance.CurrentFrameIndex = 0;
                SpriteInstance.Animate = true;
                FishOnTheLine.RelativeZ = 1;
                FishOnTheLine.RelativeY -= SpriteInstance.Height / 16;
            }
            else
            {
                float fishRelativeY;
                switch (SpriteInstance.CurrentFrameIndex)
                {
                    case 5:
                        fishRelativeY = 0; break;
                    case 6:
                        fishRelativeY = SpriteInstance.Height / 28; break;
                    case 7:
                        fishRelativeY = SpriteInstance.Height / 12; break;
                    case 8:
                        fishRelativeY = SpriteInstance.Height / 6;  break;
                    default:
                        fishRelativeY = -(SpriteInstance.Height / 16);break;
                }
                FishOnTheLine.RelativeY = fishRelativeY;

                if (IsOnFinalFrameOfAnimationChain)
                {
                    SpriteInstance.Animate = false;
                    HasFinishedDisplayingCatch = true;
                    GlobalContent.FishCaught.Play();
                }
            }
        }


        private bool ApplyDesiredDirectionToMovement(Direction desiredDirection, TileShapeCollection collision, 
            PositionedObjectList<Character> characters)
        {
            bool movedNewDirection = false;

            if(isMovingToTile == false && desiredDirection != Direction.None)
            {
                float desiredX = this.X;
                float desiredY = this.Y;
                bool isBlocked;
                MoveInDirection(desiredDirection, ref desiredX, ref desiredY);
                this.ForwardCollision.X = desiredX;
                this.ForwardCollision.Y = desiredY;

                isBlocked = GetIfIsBlocked(collision, characters);

                if (isBlocked)
                {
                    // move the collision back so it occupies the character's tile
                    this.ForwardCollision.Position = this.Position;
                }
                else
                {
                    float timeToTake = tileSize / MovementSpeed;

                    InstructionManager.MoveToAccurate(this, desiredX, desiredY, this.Z, timeToTake);
                    isMovingToTile = true;

                    this.Call(() =>
                    {
                        this.isMovingToTile = false;
                        BackwardCollision.Position = this.Position;
                        UpdateActionCollision();
                    }).After(timeToTake);

                    movedNewDirection = true;
                }
            }

            return movedNewDirection;
        }

        private bool GetIfIsBlocked(TileShapeCollection collision, PositionedObjectList<Character> characters)
        {
            var isBlocked = collision.CollideAgainst(ForwardCollision);

            if(!isBlocked)
            {
                // If not blocked, check against NPCs
                foreach(var npc in characters)
                {
                    // If the NPC is standing still - tests to see if where 'this' is trying to go to 
                    // is already occupied by the NPC
                    // If the NPC is walking - tests to see if where 'this' is trying to go is where 
                    // the NPC is already going.
                    
                    if(npc != this && ForwardCollision.CollideAgainst(npc.ForwardCollision))
                    {
                        isBlocked = true;
                        break;
                    }
                }
            }

            return isBlocked;
        }

        private static void MoveInDirection(Direction directionToMove, ref float x, ref float y)
        {
            switch (directionToMove)
            {
                case Direction.Left: x -= tileSize; break;
                case Direction.Right: x += tileSize; break;
                case Direction.Up: y += tileSize; break;
                case Direction.Down: y -= tileSize; break;
            }
        }

        private Direction GetDesiredDirection()
        {
            Direction desiredDirection = Direction.None;

            if (MovementInput != null)
            {
                if (MovementInput.X < 0)
                {
                    desiredDirection = Direction.Left;
                }
                else if (MovementInput.X > 0)
                {
                    desiredDirection = Direction.Right;
                }
                else if (MovementInput.Y < 0)
                {
                    desiredDirection = Direction.Down;
                }
                else if (MovementInput.Y > 0)
                {
                    desiredDirection = Direction.Up;
                }
            }

            return desiredDirection;
        }

        private void CustomDestroy()
		{
            ShapeManager.Remove(ForwardCollision);
            ShapeManager.Remove(BackwardCollision);
		}


        internal void ReactToReposition()
        {
            this.ForwardCollision.Position = this.Position;
            this.BackwardCollision.Position = this.Position;

            UpdateActionCollision();
        }
        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }

        public void SetSpriteOffset()
        {
            SpriteInstance.RelativeY = SpriteInstance.Height / 8;
            RodSpriteInstance.RelativeY = RodSpriteInstance.Height / 8;
        }
    }
}
