using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using FlatRedBall.AI.Pathfinding;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Graphics.Particle;
using FlatRedBall.Math.Geometry;
using FlatRedBall.Localization;
using FishKing.GumRuntimes;
using Microsoft.Xna.Framework.Input;
using FishKing.Entities;
using FishKing.UtilityClasses;
using FishKing.GameClasses;
using FishKing.Managers;
using Microsoft.Xna.Framework.Audio;

namespace FishKing.Screens
{
	public partial class MainMenu
	{
        Multiple2DInputs _movementInput;
        MultiplePressableInputs _selectionInput;
        MultiplePressableInputs _exitInput;
        Multiple1DInputs _scrollInput;
        MultiplePressableInputs _leftShoulder;
        MultiplePressableInputs _rightShoulder;

        private SoundEffectInstance _pageTurnSound;
        private SoundEffectInstance _bookOpenSound;
        private SoundEffectInstance _bookCloseSound;

        void CustomInitialize()
        {
            InitializeInput();
            FlatRedBallServices.Game.IsMouseVisible = true;
            _pageTurnSound = GlobalContent.PageTurn.CreateInstance();
            _bookOpenSound = GlobalContent.BookOpen.CreateInstance();
            _bookCloseSound = GlobalContent.BookClose.CreateInstance();
            FishopediaInstance.PageTurnSound = _pageTurnSound;
            FishopediaInstance.BookCloseSound = _bookCloseSound;
            GoFishButton.UnhighlightButton();
        }

        private void InitializeInput()
        {
            BackgroundSprite.Enabled = false;
            
            var gamePad = InputManager.Xbox360GamePads[0];

            if (gamePad != null)
            {
                _leftShoulder = new MultiplePressableInputs();
                _leftShoulder.Inputs.Add(gamePad.GetButton(Xbox360GamePad.Button.LeftTrigger));

                _rightShoulder = new MultiplePressableInputs();
                _rightShoulder.Inputs.Add(gamePad.GetButton(Xbox360GamePad.Button.RightTrigger));
            }

            var movementInputs = new Multiple2DInputs();
            movementInputs.Inputs.Add(InputManager.Keyboard.Get2DInput(
                Keys.A, Keys.D, Keys.W, Keys.S));
            movementInputs.Inputs.Add(InputManager.Keyboard.Get2DInput(Keys.Left, Keys.Right, Keys.Up, Keys.Down));
            if (InputManager.NumberOfConnectedGamePads > 0)
            {
                movementInputs.Inputs.Add(gamePad.DPad);
                movementInputs.Inputs.Add(gamePad.LeftStick);
            }
            _movementInput = movementInputs;

            var selectionInputs = new MultiplePressableInputs();
            selectionInputs.Inputs.Add(InputManager.Keyboard.GetKey(Keys.Space));
            selectionInputs.Inputs.Add(InputManager.Keyboard.GetKey(Keys.Enter));
            if (InputManager.NumberOfConnectedGamePads > 0)
            {
                selectionInputs.Inputs.Add(InputManager.Xbox360GamePads[0].GetButton(Xbox360GamePad.Button.A));
            }
            _selectionInput = selectionInputs;

            var exitInputs = new MultiplePressableInputs();
            exitInputs.Inputs.Add(InputManager.Keyboard.GetKey(Keys.Escape));
            if (gamePad != null)
            {
                exitInputs.Inputs.Add(gamePad.GetButton(Xbox360GamePad.Button.B));
                exitInputs.Inputs.Add(gamePad.GetButton(Xbox360GamePad.Button.Back));
            }
            _exitInput = exitInputs;

            var scrollInput = new Multiple1DInputs();
            if (gamePad != null)
            {
                scrollInput.Inputs.Add(new AnalogStickTo1DInput(gamePad.RightStick));
            }
            scrollInput.Inputs.Add(InputManager.Mouse.ScrollWheel);
            _scrollInput = scrollInput;

            SaveGameManager.CurrentSaveData.StartPlaySession();
        }

        void CustomActivity(bool firstTimeCalled)
        {
            if (firstTimeCalled)
            {
                MainMenuGumRuntime.ScrollToAndHighlightFirstEligibleUnplayedTournament();
            }

            HandleMenuMovement();
            HandleMenuSelection();
            HandleExitInput();
            HandleScrollInput();
            HandleBumpers();
            if (FishopediaInstance.Visible) HandlePageFlipping();
            GoFishButton.Visible = MainMenuGumRuntime.AnyTournamentIsSelected;
        }

	    private void HandleBumpers()
	    {
	        var gamePad = InputManager.Xbox360GamePads[0];
	        if (gamePad != null && FishopediaInstance.Visible)
	        {
                if (gamePad.ButtonPushed(Xbox360GamePad.Button.RightShoulder))
                {
                    FishopediaInstance.SelectNextBookmark();
                }
                else if (gamePad.ButtonPushed((Xbox360GamePad.Button.LeftShoulder)))
                {
                    FishopediaInstance.SelectPreviousBookmark();

                }
	        }
	    }

	    private void HandlePageFlipping()
        {
            if (_leftShoulder != null && _rightShoulder != null)
            {
                if (_leftShoulder.WasJustPressed) FishopediaInstance.PreviousPage();
                else if (_rightShoulder.WasJustPressed) FishopediaInstance.NextPage();
            }
        }

        private void HandleScrollInput()
        {
            if (_scrollInput.Velocity != 0)
            {
                MainMenuGumRuntime.HandleScrollInput(_scrollInput.Velocity);
            }
        }

        private void HandleExitInput()
        {
            if (_exitInput.WasJustPressed)
            {
                if (FishopediaInstance.Visible)
                {
                    FishopediaInstance.HandleExit();
                }
                else
                {
                    BackButton.CallClick();
                }
            }
        }

        private void HandleMenuSelection()
        {
            if (_selectionInput.WasJustPressed)
            {
                if (FishopediaInstance.Visible)
                {
                    FishopediaInstance.HandleSelection();
                }
                else
                {
                    HandleMenuScreenSelection();
                }
            }
        }

        private void HandleMenuScreenSelection()
        {
            if (GoFishButton.IsHighlighted)
            {
                GoFishButton.CallClick();
            }
            else if (FishopediaButton.IsHighlighted)
            {
                FishopediaButton.CallClick();
            }
            else if (BackButton.IsHighlighted)
            {
                BackButton.CallClick();
            }
            else if (MainMenuGumRuntime.AnyTournamentIsHighlighted)
            {
                MainMenuGumRuntime.HandleSelect();
            }
        }

        private void HandleMenuMovement()
        {
            var desiredDirection = CardinalTimedDirection.GetDesiredDirection(_movementInput);

            if (desiredDirection == Direction.None)
            {
                return;
            }

            if (FishopediaInstance.Visible)
            {
                FishopediaInstance.HandleMovement(desiredDirection);
            }
            else
            {
                HandleMenuScreenMovement(desiredDirection);
            }
            
        }

        private void HandleMenuScreenMovement(Direction desiredDirection)
        {
            switch (desiredDirection)
            {
                case Direction.Left:
                    if (GoFishButton.IsHighlighted)
                    {
                        GoFishButton.UnhighlightButton();
                        BackButton.HighlightButton();
                    }
                    else if (FishopediaButton.IsHighlighted)
                    {
                        FishopediaButton.UnhighlightButton();
                        MainMenuGumRuntime.ScrollToAndHighlightFirstEligibleUnplayedTournament();
                    }
                    else if (MainMenuGumRuntime.AnyTournamentIsHighlighted)
                    {
                        MainMenuGumRuntime.UnhighlightAllTournaments();
                        BackButton.HighlightButton();
                    }
                    break;
                case Direction.Right:
                    if (BackButton.IsHighlighted && GoFishButton.Visible)
                    {
                        BackButton.UnhighlightButton();
                        GoFishButton.HighlightButton();
                    }
                    else if (MainMenuGumRuntime.AnyTournamentIsHighlighted || GoFishButton.IsHighlighted)
                    {
                        GoFishButton.UnhighlightButton();
                        MainMenuGumRuntime.UnhighlightAllTournaments();
                        FishopediaButton.HighlightButton();
                    }
                    break;
                case Direction.Down:
                    if (MainMenuGumRuntime.LastTournamentPreviewIsHighlighted)
                    {
                        MainMenuGumRuntime.UnhighlightAllTournaments();
                        
                        if (_movementInput.X > 0)
                        {
                            GoFishButton.HighlightButton();
                        }
                        else
                        {
                            BackButton.HighlightButton();
                        }
                    }
                    else if (FishopediaButton.IsHighlighted)
                    {
                        FishopediaButton.UnhighlightButton();
                        GoFishButton.HighlightButton();
                    }
                    else if (!GoFishButton.IsHighlighted && !BackButton.IsHighlighted)
                    {
                        MainMenuGumRuntime.HandleTournamentPreviewMovement(desiredDirection);
                    }
                    break;
                case Direction.Up:
                    if (MainMenuGumRuntime.FirstTournamentPreviewIsHighlighted)
                    {
                        MainMenuGumRuntime.UnhighlightAllTournaments();
                        FishopediaButton.HighlightButton();
                    }
                    else if (!FishopediaButton.IsHighlighted)
                    {
                        if (GoFishButton.IsHighlighted || BackButton.IsHighlighted)
                        {
                            GoFishButton.UnhighlightButton();
                            BackButton.UnhighlightButton();
                        }
                        MainMenuGumRuntime.HandleTournamentPreviewMovement(desiredDirection);
                    }
                    break;
            }
        }
        
        void CustomDestroy()
        {
            if (_pageTurnSound != null && !_pageTurnSound.IsDisposed)
            {
                _pageTurnSound.Dispose();
            }
            if (_bookOpenSound != null && !_bookOpenSound.IsDisposed)
            {
                _bookOpenSound.Dispose();
            }
            if (_bookCloseSound != null && !_bookCloseSound.IsDisposed)
            {
                _bookCloseSound.Dispose();
            }
        }

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }


    }
}
