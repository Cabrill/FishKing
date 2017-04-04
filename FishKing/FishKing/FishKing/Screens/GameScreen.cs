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
using FishKing.Enums;
using static FishKing.Enums.WaterTypes;

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
        static string levelToLoad = "DesertIsland";
        static string startPointName = "FirstSpawn";

        bool CanMoveCharacter
        {
            get
            {
                return DialogDisplayInstance.Visible == false &&
                    !CharacterInstance.IsPullingInCatch &&
                    (!CharacterInstance.IsDisplayingCatch || CharacterInstance.HasFinishedDisplayingCatch) &&
                    (!FishCatchingInterfaceInstance.LineHasSnapped || FishCatchingInterfaceInstance.FishHasEscaped);
            }
        }

		void CustomInitialize()
        {
            LoadLevel(levelToLoad);

            InitializeCharacter();
        }

        private WaterType GetWaterType()
        {
            var tile = WaterTiles.GetTileAt(TargetingSpriteInstance.X, TargetingSpriteInstance.Y);
            if (tile == null)
            {
                return WaterType.None;
            }
            else
            {
                return WaterTypeNameToEnum(tile.Name);
            }
        }

        private void AddWaterTiles()
        {
            var waterNames = new List<string>() { "IsOcean", "IsLake", "IsRiver", "IsPond", "IsDeepOcean", "InWaterfall" };
            WaterTiles.AddWaterFrom(CurrentTileMap, (List =>
                List.Any(item => waterNames.Contains(item.Name))
                ));
            WaterTiles.Visible = false;
        }

        private void RemoveBlockeddWaterTiles()
        {
            var nonWaterLayers = new System.Collections.Generic.List<string>() { "Bridge" };
            WaterTiles.RemoveCollisionsFromLayer(CurrentTileMap, nonWaterLayers);
        }

        private void AddCollisions()
        {
            SolidCollisions.AddCollisionFrom(CurrentTileMap,
                 (list => 
                 list.Any(item => item.Name == "HasCollision") ));

            var collisionLayers = new System.Collections.Generic.List<string>() { "Walls", "Water" };
            SolidCollisions.AddCollisionFromLayer(CurrentTileMap, collisionLayers);
        }

        private void RemoveBridgedCollisions()
        {
            var nonCollisionLayers = new System.Collections.Generic.List<string>() { "Bridge" };
            SolidCollisions.RemoveCollisionsFromLayer(CurrentTileMap, nonCollisionLayers);
            //SolidCollisions.RemoveCollisionFrom(CurrentTileMap,
            //    (List => 
            //    List.Any(item => item.Name == "IsBridge")
            //    ));
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
            RemoveBridgedCollisions();
            AddWaterTiles();
            RemoveBlockeddWaterTiles();
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
            this.CharacterInstance.Position.Z = CurrentTileMap.MapLayers.Count - 3;

        }

        void CustomActivity(bool firstTimeCalled)
		{
            UpdateCamera();
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

        private void UpdateCamera()
        {
            Camera camera = Camera.Main;
            var CameraMinX = (camera.OrthogonalWidth / 2);
            var CameraMaxX = (CurrentTileMap.Width ) - (camera.OrthogonalWidth / 2);
            var CameraMinY = -CurrentTileMap.Height + camera.OrthogonalHeight / 2;
            var CameraMaxY = -(camera.OrthogonalHeight / 2);
            camera.X = this.CharacterInstance.X;
            camera.Y = this.CharacterInstance.Y;
            // assuming CameraMinX, CameraMaxX, CameraMinY, and CameraMaxY are all defined:
            camera.X = MathHelper.Clamp(camera.X, CameraMinX, CameraMaxX);
            camera.Y = MathHelper.Clamp(camera.Y, CameraMinY, CameraMaxY);
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

                TargetingSpriteInstance.Position = new Vector3(targetStartX, targetStartY, CharacterInstance.Position.Z + 1);
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
                TargetingSpriteInstance.Position = new Vector3(targetNewX, targetNewY, CharacterInstance.Position.Z + 1);
                CharacterInstance.TargetPosition = TargetingSpriteInstance.Position;
            }
            else if (CharacterInstance.JustReleasedCast)
            {
                var waterType = GetWaterType();
                if (waterType == WaterType.None)
                {
                    CharacterInstance.ResetFishingStatus();
                }
            }
            else if (CharacterInstance.IsFishing)
            {
                if (CharacterInstance.HasFishOnTheLine)
                {
                    if (CharacterInstance.HasInitiatedCatching)
                    {
                        if (!FishCatchingInterfaceInstance.HasAttachedFish)
                        {
                             FishCatchingInterfaceInstance.AttachFish(CharacterInstance.FishOnTheLine, GetWaterType());
                        }
                        if (FishCatchingInterfaceInstance.FishIsCaught)
                        {
                            FishCatchingInterfaceInstance.Stop();
                            if (!CharacterInstance.IsPullingInCatch && !CharacterInstance.IsDisplayingCatch)
                            {
                                CharacterInstance.HandleFishCaught();
                            } else if(CharacterInstance.IsDisplayingCatch && CharacterInstance.IsOnFinalFrameOfAnimationChain && !FishCatchDisplayInstance.Visible)
                            {
                                FishCatchDisplayInstance.ShowFish(CharacterInstance.FishOnTheLine);
                            }
                        }
                        else
                        {
                            if (FishCatchingInterfaceInstance.LineHasSnapped)
                            {
                                if (FishCatchingInterfaceInstance.FishHasEscaped)
                                {
                                    CharacterInstance.ResetFishingStatus();
                                    CharacterInstance.StandStill();
                                    FishCatchingInterfaceInstance.Reset();
                                    FishCatchingInterfaceInstance.Visible = false;
                                }
                            }
                            else
                            {
                                if (CharacterInstance.IsHoldingAlignButton)
                                {
                                    FishCatchingInterfaceInstance.RaiseAlignmentBar();
                                }
                                if (CharacterInstance.IsHoldingAction)
                                {
                                    FishCatchingInterfaceInstance.SpinReel();
                                }
                            }
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

            FishCatchingInterfaceInstance.Visible = CharacterInstance.HasInitiatedCatching && !FishCatchingInterfaceInstance.FishIsCaught;
            ProgressBarInstance.Visible = CharacterInstance.IsOnWindUp;
            TargetingSpriteInstance.Visible = CharacterInstance.IsOnWindUp;
            FishCatchDisplayInstance.Visible = (CharacterInstance.IsDisplayingCatch && CharacterInstance.IsOnFinalFrameOfAnimationChain);
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
