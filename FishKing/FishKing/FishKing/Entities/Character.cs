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
        Direction directionFacing = Direction.Down;

        const int tileSize = 16;

        public string Dialog { get; set; }
        public string Animation
        {
            set
            {
                var file = GetFile(value) as AnimationChainList;

                this.SpriteInstance.AnimationChains = file;
            }
        }

        public I2DInput MovementInput { get; set; }

        public IPressableInput ActionInput { get; set; }

        public bool isMovingToTile = false;

        public bool IsAttemptingAction
        {
            get;
            private set;
        }

        public bool IsHoldingAction
        {
            get;
            private set;
        }

        public bool IsCastingRod
        {
            get;
            set;
        }

        public bool IsOnWindUp
        {
            get
            {
                return (IsHoldingAction && SpriteInstance.CurrentFrameIndex == WindUpAnimationFrame);
            }
        }

        public bool IsBeforeWindUp
        {
            get
            {
                return IsCastingRod && (SpriteInstance.CurrentFrameIndex < WindUpAnimationFrame);
            }
        }

        private int WindUpAnimationFrame
        {
            get
            {
                if (directionFacing == Direction.Left || directionFacing == Direction.Right)
                {
                    return 2;
                }
                else
                {
                    return 3;
                }
            }
        }

        public bool IsFishing
        {
            get;
            set;
        }

        public bool HasFishOnTheLine
        {
            get;
            set;
        }

        public bool IsInDialog
        {
            get;
            set;
        }


        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
        private void CustomInitialize()
        {
            InitializeCollision();
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

            MoveInDirection(directionFacing, ref desiredX, ref desiredY);

            ActionCollision.X = desiredX;
            ActionCollision.Y = desiredY;
        }
        
        private void CustomActivity()
		{
            IsAttemptingAction = ActionInput != null && ActionInput.WasJustPressed && isMovingToTile == false;
            IsHoldingAction = ActionInput != null && ActionInput.IsDown;

            if (IsAttemptingAction)
            {
                IsFishing = false;
                IsCastingRod = false;
            }
        }

        public void SetSpriteOffset()
        {
            SpriteInstance.RelativeY = SpriteInstance.Height / 8;
            WoodRodSpriteInstance.RelativeY = WoodRodSpriteInstance.Height / 8;
        }

        public void PerformMovementActivity(TileShapeCollection collision, PositionedObjectList<Character> characters)
        {
            var desiredDirection = GetDesiredDirection();

            if(desiredDirection != Direction.None)
            {
                directionFacing = desiredDirection;
                IsCastingRod = false;
                IsFishing = false;
            }

            bool startedMoving = ApplyDesiredDirectionToMovement(desiredDirection, collision, characters);

            ApplyDesiredDirectionToAnimation(desiredDirection, startedMoving);

            TryUpdateActionCollision(desiredDirection, startedMoving);
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

        public void UpdateFishingStatus()
        {
            if (IsCastingRod && !IsFishing)
            {
                var shouldStartCastingAnimation = ActionInput.WasJustPressed;
                if (shouldStartCastingAnimation)
                {
                    var chainName = "";
                    switch (directionFacing)
                    {
                        case Direction.Left: chainName = "CastLeft"; break;
                        case Direction.Right: chainName = "CastRight"; break;
                        case Direction.Up: chainName = "CastUp"; break;
                        case Direction.Down: chainName = "CastDown"; break;
                    }
                    SpriteInstance.CurrentChainName =  WoodRodSpriteInstance.CurrentChainName = chainName;
                    SpriteInstance.Animate = WoodRodSpriteInstance.Animate = true;
                    SpriteInstance.CurrentFrameIndex = WoodRodSpriteInstance.CurrentFrameIndex = 0;
                }
                else if (!IsFishing)
                {
                    var lastFrameIndex = SpriteInstance.CurrentChain.Count - 1;
                    var isOnLastFrame = (WoodRodSpriteInstance.CurrentFrameIndex == lastFrameIndex);
                    var shouldHoldFrame = isOnLastFrame || IsOnWindUp;

                    SpriteInstance.Animate = !shouldHoldFrame;
                    WoodRodSpriteInstance.Animate = !shouldHoldFrame;

                    if (isOnLastFrame)
                    {
                        IsCastingRod = false;
                        IsFishing = true;
                    }
                }
            }
            WoodRodSpriteInstance.Visible = (IsCastingRod || IsFishing);
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
	}
}
