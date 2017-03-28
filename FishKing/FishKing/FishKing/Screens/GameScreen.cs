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

using Cursor = FlatRedBall.Gui.Cursor;
using GuiManager = FlatRedBall.Gui.GuiManager;
using FlatRedBall.Localization;
using Microsoft.Xna.Framework;
using FishKing.Entities;
using FishKing.DataTypes;
using System.Collections.Specialized;
using System.Linq;
using FlatRedBall.TileCollisions;

#if FRB_XNA || SILVERLIGHT
using Keys = Microsoft.Xna.Framework.Input.Keys;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using Texture2D = Microsoft.Xna.Framework.Graphics.Texture2D;
#endif
#endregion

namespace FishKing.Screens
{
	public partial class GameScreen
	{
        static string levelToLoad = "BasicIsland";
        static string startPointName = "FirstSpawn";

        bool CanMoveCharacter
        {
            get
            {
                return DialogDisplayInstance.Visible == false && 
                    !CharacterInstance.IsPullingInCatch && 
                    (!CharacterInstance.IsDisplayingCatch || CharacterInstance.HasFinishedDisplayingCatch);
            }
        }

		void CustomInitialize()
        {
            LoadLevel(levelToLoad);

            InitializeCharacter();
        }

        private void AddCollisions()
        {
            var collisions = new List<String>() { "Water", "Cliffside", "CliffsideLeft", "CliffsideRight" };
            SolidCollisions.AddCollisionFrom(BasicIsland, collisions);
        }

        private void HandleNewNpc(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach(var item in e.NewItems)
            {
                (item as Character).ReactToReposition();
            }
        }

        void LoadLevel(string levelToLoad)
        {
            InitializeLevel(levelToLoad);
            AddCollisions();
            AdjustCamera();
            AdjustNpcs();

#if DEBUG
            this.SolidCollisions.Visible =
                DebuggingVariables.ShowShapes;
#endif
        }

        private void AdjustNpcs()
        {
            foreach(var character in NpcCharacterList)
            {
                character.ReactToReposition();
            }
        }

        private void AdjustCamera()
        {
            Camera.Main.MinimumX -= .5f;
            Camera.Main.MaximumX += .5f;
            Camera.Main.MinimumY -= .5f;
            Camera.Main.MaximumY += .5f;
            


            Camera.Main.X += .25f;
            Camera.Main.Y += .25f;
        }

        private void InitializeCharacter()
        {
            var foundStartPoint = this.StartPointList.FirstOrDefault(item => item.Name == startPointName);
            if(foundStartPoint == null)
            {
                throw new Exception($"Could not find start point with a name of {startPointName}");
            }
            this.CharacterInstance.X = foundStartPoint.X;
            this.CharacterInstance.Y = foundStartPoint.Y;

            this.CharacterInstance.ReactToReposition();

            this.CharacterInstance.MovementInput = InputManager.Keyboard.Get2DInput(
                Keys.A, Keys.D, Keys.W, Keys.S);

            this.CharacterInstance.ActionInput = InputManager.Keyboard.GetKey(Keys.Space);
            this.CharacterInstance.FishingAlignmentInput = InputManager.Mouse.GetButton(Mouse.MouseButtons.LeftButton);

        }

        void CustomActivity(bool firstTimeCalled)
		{
            DialogActivity();
            FishingActivity();

            bool characterMoved = false;
            if (CanMoveCharacter)
            {
                characterMoved = this.CharacterInstance.PerformMovementActivity(this.SolidCollisions, NpcCharacterList);
            }

            CharacterInstance.UpdateFishingStatus(characterMoved);
            this.CharacterInstance.SetSpriteOffset();

            CollisionActivity();
		}

        private void DialogActivity()
        {
            if (CharacterInstance.IsAttemptingAction)
            {
                if (this.DialogDisplayInstance.Visible)
                {
                    this.DialogDisplayInstance.Visible = false;
                }
                else
                {
                    Character npcTalkingTo = null;
                    foreach (var npc in this.NpcCharacterList)
                    {
                        if (CharacterInstance.ActionCollision.CollideAgainst(npc.BackwardCollision))
                        {
                            npcTalkingTo = npc;
                            break;
                        }
                    }

                    if(npcTalkingTo != null)
                    {
                        ShowDialog(npcTalkingTo.Dialog);
                    }
                }
            }
            CharacterInstance.IsInDialog = DialogDisplayInstance.Visible;
        }

        private void FishingActivity()
        {
#if DEBUG
            if (DebuggingVariables.ImmediatelyStartFishing && !CharacterInstance.IsFishing && !CharacterInstance.IsMoving)
            {
                FishCatchingInterfaceInstance.Reset();
                CharacterInstance.ResetFishingStatus();
                CharacterInstance.IsFishing = true;
                var fish = FishGenerator.CreateFish();
                CharacterInstance.FishOnTheLine = fish;
                CharacterInstance.HasInitiatedCatching = true;
            }
#endif
            if ((CharacterInstance.IsMoving && FishCatchingInterfaceInstance.HasAttachedFish) || 
                (CharacterInstance.IsAttemptingAction && CharacterInstance.HasFinishedDisplayingCatch))
            {
                FishCatchingInterfaceInstance.Reset();
                CharacterInstance.ResetFishingStatus();
            }
            
            var characterJustReleased = CharacterInstance.IsCastingRod && CharacterInstance.ActionInput.WasJustReleased;

            var characterJustStartedfishing = CharacterInstance.IsAttemptingAction && !CharacterInstance.IsInDialog &&
                !CharacterInstance.IsFishing && !CharacterInstance.IsCastingRod && !CharacterInstance.IsMoving && 
                !CharacterInstance.IsAttemptingMovement;

            var tileSize = (float)BasicIsland.WidthPerTile;

            if (characterJustStartedfishing)
            {
                CharacterInstance.HasInitiatedCatching = false;
                CharacterInstance.IsCastingRod = true;

                ProgressBarInstance.ResetProgress();
                ProgressBarInstance.PositionProgressBarOver(CharacterInstance.Position);

                var targetStartX = CharacterInstance.Position.X;
                var targetStartY = CharacterInstance.Position.Y;
                switch (CharacterInstance.DirectionFacing)
                {
                    case Direction.Left: targetStartX -= tileSize*2.5f; break;
                    case Direction.Right: targetStartX += tileSize * 2.5f; break;
                    case Direction.Up: targetStartY += tileSize * 2.5f; break;
                    case Direction.Down: targetStartY -= tileSize * 2.5f; break;
                }

                TargetingSpriteInstance.Position = new Vector3(targetStartX, targetStartY, 1);
                CharacterInstance.TargetPosition = TargetingSpriteInstance.Position;
            }
            else if (CharacterInstance.IsOnWindUp)
            {
                ProgressBarInstance.Update();

                var percentPower = (float)Decimal.Divide(ProgressBarInstance.Progress, 100);
                var maxDistance = CharacterInstance.MaxDistanceTileCast * tileSize;
                var effectiveDistance = (tileSize*2.5f) + (maxDistance * percentPower);

                var targetNewX = CharacterInstance.Position.X;
                var targetNewY = CharacterInstance.Position.Y;

                switch (CharacterInstance.DirectionFacing)
                {
                    case Direction.Left: targetNewX += -effectiveDistance; break;
                    case Direction.Right: targetNewX += effectiveDistance; break;
                    case Direction.Up: targetNewY += effectiveDistance; break;
                    case Direction.Down: targetNewY += -effectiveDistance; break;
                }
                TargetingSpriteInstance.Position = new Vector3(targetNewX, targetNewY, 1);
                CharacterInstance.TargetPosition = TargetingSpriteInstance.Position;
            }
            else if (CharacterInstance.IsFishing)
            {
                if (CharacterInstance.HasFishOnTheLine)
                {
                    if (CharacterInstance.IsHoldingAlignButton)
                    {
                        FishCatchingInterfaceInstance.RaiseAlignmentBar();
                    }
                    if (CharacterInstance.IsHoldingAction)
                    {
                        FishCatchingInterfaceInstance.SpinReel();
                    }
                    if (CharacterInstance.HasInitiatedCatching)
                    {
                        if (!FishCatchingInterfaceInstance.HasAttachedFish)
                        {
                             FishCatchingInterfaceInstance.AttachFish(CharacterInstance.FishOnTheLine);
                        }
                        if (FishCatchingInterfaceInstance.IsFishCaught)
                        {
                            if (!CharacterInstance.IsPullingInCatch && !CharacterInstance.IsDisplayingCatch)
                            {
                                CharacterInstance.HandleFishCaught();
                            } 
                        }
                        else
                        {
                            FishCatchingInterfaceInstance.Update();
                        }
                    }
                }
                else
                {
                    var rnd = new Random();
                    var catchChance = 0.003;
                    var catchRoll = rnd.NextDouble();

                    if (
#if DEBUG
                    DebuggingVariables.ImmediatelyCatchFish ||
#endif
                        catchRoll <= catchChance)
                    {
                        var fish = FishGenerator.CreateFish();
                        CharacterInstance.FishOnTheLine = fish;
                    }
                }
            }

            FishCatchingInterfaceInstance.Visible = CharacterInstance.HasInitiatedCatching && !FishCatchingInterfaceInstance.IsFishCaught;
            ProgressBarInstance.Visible = CharacterInstance.IsOnWindUp;
            TargetingSpriteInstance.Visible = CharacterInstance.IsOnWindUp;
        }

        private void ShowDialog(string stringId)
        {
            this.DialogDisplayInstance.Visible = true;
            this.DialogDisplayInstance.Text = LocalizationManager.Translate(stringId);
        }

        private void CollisionActivity()
        {
            foreach(var trigger in MapNavigationTriggerList)
            {
                if(CharacterInstance.BackwardCollision.CollideAgainst(trigger.Collision))
                {
                    levelToLoad = trigger.TargetMap;
                    startPointName = trigger.StartPointName;

                    if(string.IsNullOrEmpty(startPointName))
                    {
                        throw new Exception("Trigger has an empty StartPointName");
                    }

                    RestartScreen(reloadContent: false);
                }
            }

        }

        void CustomDestroy()
		{
            
		}

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

	}
}
