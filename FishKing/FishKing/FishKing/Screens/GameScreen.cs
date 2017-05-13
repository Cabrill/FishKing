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
using Microsoft.Xna.Framework.Media;
using FlatRedBall.Math;
using FlatRedBall.Gui;
using FishKing.Managers;

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
        static string levelToLoad = "ForestPond";
        static string startPointName = "FirstSpawn";
        private bool wasFishing = false;
        private bool shouldUpdateCamera;

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
            InitializePauseMenuButtons();

            if (!MusicManager.PlayingSong)
            {
                List<Song> playList = new List<Song>() { Audionautix_AcousticGuitar1, Audionautix_OneFineDay, Audionautix_Serenity };
                MusicManager.PlayList = playList;
                MusicManager.Volume = 0.15f;
                MusicManager.PlaySong();
            }

            if (!TournamentManager.TournamentHasStarted
#if DEBUG
                && !DebuggingVariables.MapDebugMode
#endif
                )
            {
                TournamentManager.StartTournament();
                levelToLoad = TournamentManager.CurrentTournament.MapName;
            }

            LoadLevel(levelToLoad);
            
            InitializeCharacter();

            InitializeCamera();

            TournamentStatusInstance.Setup(this);

#if DEBUG
            if (DebuggingVariables.MapDebugMode)
            {
                RestartVariables.Add("this.CharacterInstance.X");
                RestartVariables.Add("this.CharacterInstance.Y");
            }
#endif
        }

        private void InitializePauseMenuButtons()
        {
            PauseMenuInstance.ResumeGameButtonClick += (IWindow window) => { UnpauseThisScreen(); PauseMenuInstance.Visible = false; };
            PauseMenuInstance.HelpButtonClick += (IWindow window) => { HelpScreenInstance.Visible = true; };
            PauseMenuInstance.ExitButtonClick += (IWindow window) => {
                PopupMessageInstance.CurrentPopupDisplayState = GumRuntimes.PopupMessageRuntime.PopupDisplay.OKCancel;
                PopupMessageInstance.TitleText = "Confirm Quit";
                PopupMessageInstance.PopupText = "Return to the main menu and abandon this tournament?";
                PopupMessageInstance.CancelButtonClick += (IWindow win) => { PopupMessageInstance.Visible = false; };
                PopupMessageInstance.OKButtonClick += (IWindow win) =>
                {
                    SaveGameManager.CurrentSaveData.StopPlaySession();
                    SaveGameManager.SaveCurrentData();
                    LoadingScreen.TransitionToScreen(typeof(MainMenu).FullName);
                };
                PopupMessageInstance.Visible = true;
            };
        }


        private void InitializeCamera()
        {
            var camera = Camera.Main;
            camera.ClearBorders();
            camera.ClearMinimumsAndMaximums();
            if (CurrentTileMap.Width > camera.OrthogonalWidth || CurrentTileMap.Height > camera.OrthogonalHeight)
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
#else
            this.SolidCollisions.Visible = false;
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

            var gamePad = InputManager.Xbox360GamePads[0];

            var movementInputs = new Multiple2DInputs();
            movementInputs.Inputs.Add(InputManager.Keyboard.Get2DInput(
                Keys.A, Keys.D, Keys.W, Keys.S));
            movementInputs.Inputs.Add(InputManager.Keyboard.Get2DInput(
                Keys.Left, Keys.Right, Keys.Up, Keys.Down));
            if (InputManager.NumberOfConnectedGamePads > 0)
            {
                movementInputs.Inputs.Add(gamePad.DPad);
                movementInputs.Inputs.Add(gamePad.LeftStick);
            }

            var actionInputs = new MultiplePressableInputs();
            actionInputs.Inputs.Add(InputManager.Keyboard.GetKey(Keys.Space));
            if (InputManager.NumberOfConnectedGamePads > 0)
            {
                actionInputs.Inputs.Add(InputManager.Xbox360GamePads[0].GetButton(Xbox360GamePad.Button.A));
                actionInputs.Inputs.Add(InputManager.Xbox360GamePads[0].GetButton(Xbox360GamePad.Button.Y));
                actionInputs.Inputs.Add(InputManager.Xbox360GamePads[0].GetButton(Xbox360GamePad.Button.X));
                actionInputs.Inputs.Add(InputManager.Xbox360GamePads[0].GetButton(Xbox360GamePad.Button.B));
            }

            var alignmentInputs = new Multiple1DInputs();
            var leftMouseButton = new PressableTo1DInput(InputManager.Mouse.GetButton(Mouse.MouseButtons.LeftButton));
            alignmentInputs.Inputs.Add(leftMouseButton);
            if (InputManager.NumberOfConnectedGamePads > 0)
            {
                var rightTrigger = new AnalogButtonTo1DInput(gamePad.RightTrigger);
                alignmentInputs.Inputs.Add(rightTrigger);
            }

            var reelingInputs = new Multiple1DInputs();
            var leftShift = new PressableTo1DInput(InputManager.Keyboard.GetKey(Keys.LeftShift));
            reelingInputs.Inputs.Add(leftShift);
            var rightShift = new PressableTo1DInput(InputManager.Keyboard.GetKey(Keys.RightShift));
            reelingInputs.Inputs.Add(rightShift);
            if (InputManager.NumberOfConnectedGamePads > 0)
            {
                var leftTrigger = new AnalogButtonTo1DInput(gamePad.LeftTrigger);
                reelingInputs.Inputs.Add(leftTrigger);
            }

            var escapeInputs = new MultiplePressableInputs();
            escapeInputs.Inputs.Add(InputManager.Keyboard.GetKey(Keys.Escape));
            if (InputManager.NumberOfConnectedGamePads > 0)
            {
                escapeInputs.Inputs.Add(gamePad.GetButton(Xbox360GamePad.Button.Start));
            }

            var helpInputs = new MultiplePressableInputs();
            helpInputs.Inputs.Add(InputManager.Keyboard.GetKey(Keys.F1));
            if (InputManager.NumberOfConnectedGamePads > 0)
            {
                helpInputs.Inputs.Add(gamePad.GetButton(Xbox360GamePad.Button.Back));
            }

            this.CharacterInstance.MovementInput = movementInputs;
            this.CharacterInstance.ActionInput = actionInputs;
            this.CharacterInstance.FishingAlignmentInput = alignmentInputs;
            this.CharacterInstance.ReelingInput = reelingInputs;
            this.CharacterInstance.EscapeInput = escapeInputs;
            this.CharacterInstance.HelpInput = helpInputs;

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
            if (FlatRedBall.Input.InputManager.Keyboard.GetKey(Keys.F5).WasJustPressed)
            {
                RestartScreen(reloadContent: true);
            }
#endif
            MusicManager.Update();
            if (CharacterInstance.EscapeInput.WasJustPressed && !TournamentStatusInstance.HasStartedCelebration)
            {
                if (HelpScreenInstance.Visible)
                {
                    HelpScreenInstance.Visible = false;
                }
                else
                {
                    PauseMenuInstance.Visible = !PauseMenuInstance.Visible;

                    if (PauseMenuInstance.Visible)
                    {
                        PauseThisScreen();
                    }
                    else
                    {
                        UnpauseThisScreen();
                    }
                }
            }

            FlatRedBallServices.Game.IsMouseVisible = IsPaused;
            if (IsPaused)
            {
                HandlePauseInput();
            }
            else
            { 
#if DEBUG
                if (DebuggingVariables.TournamentScoresYUIOP)
                {
                    if (InputManager.Keyboard.GetKey(Keys.Y).WasJustPressed)
                    {
                        TournamentManager.CurrentScores.AddToNonPlayerScore(1, 10);
                    }
                    if (InputManager.Keyboard.GetKey(Keys.U).WasJustPressed)
                    {
                        TournamentManager.CurrentScores.AddToNonPlayerScore(2, 10);
                    }
                    if (InputManager.Keyboard.GetKey(Keys.I).WasJustPressed)
                    {
                        TournamentManager.CurrentScores.AddToNonPlayerScore(3, 10);
                    }
                    if (InputManager.Keyboard.GetKey(Keys.O).WasJustPressed)
                    {
                        TournamentManager.CurrentScores.AddToNonPlayerScore(4, 10);
                    }
                    if (InputManager.Keyboard.GetKey(Keys.P).WasJustPressed)
                    {
                        TournamentManager.CurrentScores.AddToNonPlayerScore(5, 10);
                    }
                    if (InputManager.Keyboard.GetKey(Keys.T).WasJustPressed)
                    {
                        TournamentManager.CurrentScores.AddToPlayerScore(10);
                    }
                }
#endif

#if DEBUG
                if (DebuggingVariables.MapDebugMode)
                {
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
                }
                else
                {
#endif
                    if (TournamentManager.CurrentScores.HasPlayerFinished)
                    {
                        if (!ResultsDisplayInstance.Visible)
                        {
                            TournamentStatusInstance.UpdateFishPlaceMarkers(TournamentManager.CurrentScores.AsArray);
                            TournamentManager.EndTournament();

                            ResultsDisplayInstance.DisplayResults(TournamentManager.CurrentTournamentResults);
                            ResultsDisplayInstance.OKButtonClick += ResultsDisplayOkClick;
                            ResultsDisplayInstance.Visible = true;

                            TournamentStatusInstance.StartCelebration();
                        }
                        else if (CharacterInstance.EscapeInput.WasJustPressed || CharacterInstance.ActionInput.WasJustPressed)
                        {
                            ResultsDisplayInstance.ClickOK();
                        }
                    }
                    else
                    {
#if DEBUG
                        if (DebuggingVariables.SimulateTournamentScores)
                        {
                            TournamentManager.CurrentScores.SimulateTournament();
                        }
#endif
                        if (TournamentManager.CurrentScores.HasScoreChanged)
                        {
                            TournamentStatusInstance.UpdateFishPlaceMarkers(TournamentManager.CurrentScores.AsArray);
                            TournamentManager.CurrentScores.MarkScoreReviewed();
                        }
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
                    }
#if DEBUG
                }
#endif
            }
        }

        private void ResultsDisplayOkClick(IWindow window)
        {
            SaveGameManager.CurrentSaveData.AddTournamentResult(TournamentManager.CurrentTournamentResults);
            SaveGameManager.SaveCurrentData();
            LoadingScreen.TransitionToScreen(typeof(MainMenu).FullName);
        }

        private void HandlePauseInput()
        {
            if (PopupMessageInstance.Visible)
            {

                if (CharacterInstance.ActionInput.WasJustPressed)
                {
                    PopupMessageInstance.HandleSelection();
                }
                else
                {
                    PopupMessageInstance.HandleMovement(CardinalTimedDirection.GetDesiredDirection(CharacterInstance.MovementInput));
                }
            }
            else
            {
                PauseMenuInstance.HandleMovement(CharacterInstance.MovementInput);
                PauseMenuInstance.HandleSelection(CharacterInstance.ActionInput);
            }
        }
        

        const float offset = .5f;
        const float roundingValue = 1;
        private void UpdateCamera()
        {
            Camera.Main.X = MathFunctions.RoundFloat(this.CharacterInstance.X, roundingValue, offset);
            Camera.Main.Y = MathFunctions.RoundFloat(this.CharacterInstance.Y, roundingValue, offset);
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
                                TournamentManager.CurrentScores.AddToPlayerScore(CharacterInstance.FishOnTheLine.Points);
                                SaveGameManager.CurrentSaveData.AddCaughtFish(CharacterInstance.FishOnTheLine);
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
                                    FishCatchingInterfaceInstance.RaiseAlignmentBar(CharacterInstance.AlignAmount);
                                }
                                if (CharacterInstance.IsHoldingReelButton)
                                {
                                    FishCatchingInterfaceInstance.SpinReel(CharacterInstance.ReelAmount);
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
