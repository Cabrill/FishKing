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
using Microsoft.Xna.Framework.Audio;
using FishKing.UtilityClasses;

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
        private bool wasFishing = false;
        private bool shouldUpdateCamera;
        private static TournamentScores TournamentScore;

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
            TournamentStatusInstance.gameScreen = this;
            LoadLevel(levelToLoad);
            
            InitializeCharacter();

            InitializeCamera();

            if (TournamentScore == null)
            {
                TournamentScore = new TournamentScores();
                TournamentStatusInstance.GoalScore = 200;
                TournamentStatusInstance.PlayerFishNumber = 4;
            }
        }

        private void InitializeCamera()
        {
            var camera = Camera.Main;
            camera.ClearBorders();
            camera.ClearMinimumsAndMaximums();
            if (CurrentTileMap.Width > camera.OrthogonalWidth && CurrentTileMap.Height > camera.OrthogonalHeight)
            {
                shouldUpdateCamera = true;

                camera.MinimumX = (camera.OrthogonalWidth / 2);
                camera.MaximumX = (CurrentTileMap.Width) - (camera.OrthogonalWidth / 2);
                camera.MinimumY = -CurrentTileMap.Height + camera.OrthogonalHeight / 2;
                camera.MaximumY = -(camera.OrthogonalHeight / 2) + TournamentStatusInstance.AbsoluteHeight;
                camera.X = this.CharacterInstance.X;
                camera.Y = this.CharacterInstance.Y;
            }
            else
            {
                shouldUpdateCamera = false;
                camera.X = CurrentTileMap.Width / 2;
                camera.Y = -CurrentTileMap.Height / 2;
            }
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
            AdjustNpcs();

#if DEBUG
            this.SolidCollisions.Visible =
                DebuggingVariables.ShowShapes;
#endif
            
            if (levelToLoad.Contains("Cave"))
            {
                CaveLightConeSprite.Position = Camera.Main.Position;
                CaveLightConeSprite.Visible = true;
                CaveLightConeSprite.Z = -1;
            }
            else
            {
                CaveLightConeSprite.Visible = false;
            }
            AmbientAudioManager.CurrentTileMap = CurrentTileMap;
        }

        private void AdjustNpcs()
        {
            foreach(var character in NpcCharacterList)
            {
                character.ReactToReposition();
            }
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

            var overlayLayer = CurrentTileMap.MapLayers.FindByName("Overlay");
            if (overlayLayer != null)
            {
                this.CharacterInstance.Position.Z = overlayLayer.Z - 0.5f;
            }
            else
            {
                this.CharacterInstance.Position.Z = CurrentTileMap.MapLayers.Count - 2;
            }
            AmbientAudioManager.CharacterInstance = CharacterInstance;
            AmbientAudioManager.UpdateAmbientSoundSources();
        }

        void CustomActivity(bool firstTimeCalled)
		{

#if DEBUG
            if (DebuggingVariables.TournamentScoresYUIOP)
            {
                if (InputManager.Keyboard.GetKey(Keys.Y).WasJustPressed)
                {
                    TournamentScore.AddToNonPlayerScore(1, 10);
                }
                if (InputManager.Keyboard.GetKey(Keys.U).WasJustPressed)
                {
                    TournamentScore.AddToNonPlayerScore(2, 10);
                }
                if (InputManager.Keyboard.GetKey(Keys.I).WasJustPressed)
                {
                    TournamentScore.AddToNonPlayerScore(3, 10);
                }
                if (InputManager.Keyboard.GetKey(Keys.O).WasJustPressed)
                {
                    TournamentScore.AddToNonPlayerScore(4, 10);
                }
                if (InputManager.Keyboard.GetKey(Keys.P).WasJustPressed)
                {
                    TournamentScore.AddToNonPlayerScore(5, 10);
                }
                if (InputManager.Keyboard.GetKey(Keys.T).WasJustPressed)
                {
                    TournamentScore.AddToPlayerScore(10);
                }
            }
#endif

            if (shouldUpdateCamera) UpdateCamera();
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
            if (CharacterInstance.IsMoving)
            {
                AmbientAudioManager.UpdateAmbientSoundSources();
            }
#if DEBUG
            if (DebuggingVariables.SimulateTournamentScores)
            {
                TournamentScore.SimulateTournament();
            }
#endif
            if (TournamentScore.HasScoreChanged)
            {
                TournamentStatusInstance.UpdateFishPlaceMarkers(TournamentScore.Scores);
                TournamentScore.MarkScoreReviewed();
            }
        }

        private void UpdateCamera()
        {
            Camera.Main.X = this.CharacterInstance.X;
            Camera.Main.Y = this.CharacterInstance.Y;
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

            var tileSize = (float)CurrentTileMap.WidthPerTile;

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
                                TournamentScore.AddToPlayerScore(CharacterInstance.FishOnTheLine.Points);
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
                    var catchChance = 0.0025;
                    var catchRoll = rnd.NextDouble();

                    if (
#if DEBUG
                    DebuggingVariables.ImmediatelyCatchFish ||
#endif
                        catchRoll <= catchChance)
                    {
                        var fish = FishGenerator.CreateFish(GetWaterType());
                        CharacterInstance.FishOnTheLine = fish;
                    }
                }
            }

            FishCatchingInterfaceInstance.Visible = CharacterInstance.HasInitiatedCatching && !FishCatchingInterfaceInstance.FishIsCaught;
            ProgressBarInstance.Visible = CharacterInstance.IsOnWindUp;
            TargetingSpriteInstance.Visible = CharacterInstance.IsOnWindUp;
            FishCatchDisplayInstance.Visible = (CharacterInstance.IsDisplayingCatch && CharacterInstance.IsOnFinalFrameOfAnimationChain);

            if (wasFishing  && !CharacterInstance.IsFishing)
            {
                FishCatchingInterfaceInstance.Stop();
            }

            wasFishing = CharacterInstance.IsFishing;
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
            var waterNames = new List<string>() { "IsOcean", "IsDeepOcean", "IsLake", "IsCaveLake", "IsRiver", "IsPond", "IsDeepOcean", "IsWaterfall" };
            WaterTiles.AddWaterFrom(CurrentTileMap, (List =>
                List.Any(item => waterNames.Contains(item.Name))
                ));
            WaterTiles.Visible = false;
        }

        private void RemoveBlockeddWaterTiles()
        {
            var nonWaterLayers = new System.Collections.Generic.List<string>() { "Bridge", "Boat" };
            WaterTiles.RemoveCollisionsFromLayer(CurrentTileMap, nonWaterLayers);
        }

        private void AddCollisions()
        {
            SolidCollisions.AddCollisionFrom(CurrentTileMap,
                 (list =>
                 list.Any(item => item.Name == "HasCollision")));

            var collisionLayers = new System.Collections.Generic.List<string>() { "Walls", "Water" };
            SolidCollisions.AddCollisionFromLayer(CurrentTileMap, collisionLayers);
        }

        private void RemoveBridgedCollisions()
        {
            var nonCollisionLayers = new System.Collections.Generic.List<string>() { "Bridge", "Boat" };
            SolidCollisions.RemoveCollisionsFromLayer(CurrentTileMap, nonCollisionLayers);
            //SolidCollisions.RemoveCollisionFrom(CurrentTileMap,
            //    (List => 
            //    List.Any(item => item.Name == "IsBridge")
            //    ));
        }

        void CustomDestroy()
        {
            AmbientAudioManager.RemoveAmbientAudioSources();
        }

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

	}
}
