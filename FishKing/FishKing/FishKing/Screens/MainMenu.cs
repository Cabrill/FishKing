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

namespace FishKing.Screens
{
	public partial class MainMenu
	{
        Multiple2DInputs MovementInput;
        MultiplePressableInputs SelectionInput;
        MultiplePressableInputs ExitInput;
        I1DInput ScrollInput;
        MultiplePressableInputs leftShoulder;
        MultiplePressableInputs rightShoulder;

        void CustomInitialize()
        {
            InitializeInput();
            FlatRedBallServices.Game.IsMouseVisible = true;
            //FlatRedBall.Audio.AudioManager.PlaySong(Echinoderm_Regeneration_Sting, true, false);
        }

        private void InitializeInput()
        {
            BackgroundSprite.Enabled = false;
            
            var gamePad = InputManager.Xbox360GamePads[0];

            if (gamePad != null)
            {
                leftShoulder = new MultiplePressableInputs();
                leftShoulder.Inputs.Add(gamePad.GetButton(Xbox360GamePad.Button.LeftShoulder));
                leftShoulder.Inputs.Add(gamePad.GetButton(Xbox360GamePad.Button.LeftTrigger));

                rightShoulder = new MultiplePressableInputs();
                rightShoulder.Inputs.Add(gamePad.GetButton(Xbox360GamePad.Button.RightShoulder));
                rightShoulder.Inputs.Add(gamePad.GetButton(Xbox360GamePad.Button.RightTrigger));
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
            MovementInput = movementInputs;

            var selectionInputs = new MultiplePressableInputs();
            selectionInputs.Inputs.Add(InputManager.Keyboard.GetKey(Keys.Space));
            selectionInputs.Inputs.Add(InputManager.Keyboard.GetKey(Keys.Enter));
            if (InputManager.NumberOfConnectedGamePads > 0)
            {
                selectionInputs.Inputs.Add(InputManager.Xbox360GamePads[0].GetButton(Xbox360GamePad.Button.A));
            }
            SelectionInput = selectionInputs;

            var exitInputs = new MultiplePressableInputs();
            exitInputs.Inputs.Add(InputManager.Keyboard.GetKey(Keys.Escape));
            ExitInput = exitInputs;

            ScrollInput = InputManager.Mouse.ScrollWheel;
        }

        void CustomActivity(bool firstTimeCalled)
        {
            if (firstTimeCalled)
            {
                MainMenuGumRuntime.ScrollToAndHighlightLastEligibleTournament();
            }

            //FlatRedBall.Debugging.Debugger.Write(FlatRedBall.Gui.GuiManager.Cursor.WindowOver);
            HandleMenuMovement();
            HandleMenuSelection();
            HandleExitInput();
            HandleScrollInput();
            if (FishopediaInstance.Visible) HandlePageFlipping();
            GoFishButton.Visible = MainMenuGumRuntime.AnyTournamentIsSelected;

            if (FlatRedBall.Audio.AudioManager.CurrentlyPlayingSong == null)
            {
                //FlatRedBall.Audio.AudioManager.PlaySong(The_Low_Seas, true, false);
            }
        }

        private void HandlePageFlipping()
        {
            if (leftShoulder != null && rightShoulder != null)
            {
                if (leftShoulder.WasJustPressed) FishopediaInstance.PreviousPage();
                else if (rightShoulder.WasJustPressed) FishopediaInstance.NextPage();
            }
        }

        private void HandleScrollInput()
        {
            if (ScrollInput.Value != 0)
            {
                MainMenuGumRuntime.HandleScrollInput(ScrollInput.Velocity);
            }
        }

        private void HandleExitInput()
        {
            if (ExitInput.WasJustPressed)
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
            if (SelectionInput.WasJustPressed)
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
            var desiredDirection = CardinalTimedDirection.GetDesiredDirection(MovementInput);

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
                    else if (MainMenuGumRuntime.AnyTournamentIsHighlighted && GoFishButton.Visible)
                    {
                        MainMenuGumRuntime.UnhighlightAllTournaments();
                        GoFishButton.HighlightButton();
                    }
                    break;
                case Direction.Down:
                    if (MainMenuGumRuntime.LastTournamentPreviewIsHighlighted)
                    {
                        MainMenuGumRuntime.UnhighlightAllTournaments();
                        
                        if (MovementInput.X > 0)
                        {
                            GoFishButton.HighlightButton();
                        }
                        else
                        {
                            BackButton.HighlightButton();
                        }
                    }
                    else if (!GoFishButton.IsHighlighted && !BackButton.IsHighlighted)
                    {
                        MainMenuGumRuntime.HandleTournamentPreviewMovement(desiredDirection);
                    }
                    break;
                case Direction.Up:
                    if (GoFishButton.IsHighlighted || BackButton.IsHighlighted)
                    {
                        GoFishButton.UnhighlightButton();
                        BackButton.UnhighlightButton();
                    }
                    MainMenuGumRuntime.HandleTournamentPreviewMovement(desiredDirection);
                    break;
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
