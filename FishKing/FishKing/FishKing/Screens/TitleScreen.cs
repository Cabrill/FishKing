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
using Microsoft.Xna.Framework.Input;
using FlatRedBall.Gui;
using FishKing.Entities;
using FishKing.GumRuntimes;

namespace FishKing.Screens
{
	public partial class TitleScreen
	{
        Multiple2DInputs MovementInput;
        MultiplePressableInputs SelectionInput;
        MultiplePressableInputs ExitInput;
        TitleScreenGumRuntime screen;
        double lastMovementTime = 0;
        double timeBetweenMovement = 0.2;

        void CustomInitialize()
		{
            InitializeInput();
            screen = TitleScreenGumRuntime;
            screen.IntroAnimation.Play();
            FlatRedBallServices.Game.IsMouseVisible = true;
            Microsoft.Xna.Framework.Media.MediaPlayer.Volume = 0.5f;
            FlatRedBall.Audio.AudioManager.PlaySong(Echinoderm_Regeneration_Sting, true, false);
        }

        private void InitializeInput()
        {
            var gamePad = InputManager.Xbox360GamePads[0];

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
        }

        void CustomActivity(bool firstTimeCalled)
		{
            HandleMenuMovement();
            HandleMenuSelection();
            HandleExitInput();
            if (FlatRedBall.Audio.AudioManager.CurrentlyPlayingSong == null)
            {
                //FlatRedBall.Audio.AudioManager.PlaySong(The_Low_Seas, true, false);
            }
        }

        private void HandleExitInput()
        {
            if (ExitInput.WasJustPressed)
            {
                if (AboutPopup.Visible)
                {
                    AboutPopup.HandleExit();
                }
                else
                {
                    ExitButton.CallClick();
                }
            }
        }

        private void HandleMenuSelection()
        {
            if (SelectionInput.WasJustPressed)
            {
                if (AboutPopup.Visible)
                {
                    AboutPopup.HandleSelection();
                }
                else if (screen.CurrentMenuScreenButtonsState == TitleScreenGumRuntime.MenuScreenButtons.InitialButtons)
                {
                    HandleInitialScreenSelection();
                }
                else if (TitleScreenGumRuntime.CurrentMenuScreenButtonsState == TitleScreenGumRuntime.MenuScreenButtons.PlayButtons)
                {
                    //TODO:  Add new game/continue
                }
            }
        }

        private void HandleInitialScreenSelection()
        {
            if (PlayButton.IsHighlighted)
            {
                PlayButton.CallClick();
            }
            else if (AboutButton.IsHighlighted)
            {
                AboutButton.CallClick();
            }
            else if (ExitButton.IsHighlighted)
            {
                ExitButton.CallClick();
            }
        }

        private void HandleMenuMovement()
        {
            var desiredDirection = GetDesiredDirection();

            if (desiredDirection == Direction.None)
            {
                return;
            }

            if (AboutPopup.Visible)
            {
                AboutPopup.HandleMovement(desiredDirection);
            }
            else if (screen.CurrentMenuScreenButtonsState == TitleScreenGumRuntime.MenuScreenButtons.InitialButtons)
            {
                HandleInitialScreenMovement(desiredDirection);
            }
            else if (TitleScreenGumRuntime.CurrentMenuScreenButtonsState == TitleScreenGumRuntime.MenuScreenButtons.PlayButtons)
            {
                //TODO:  Add new game/continue
            }
        }

        private void HandleInitialScreenMovement(Direction desiredDirection)
        {
            switch (desiredDirection)
            {
                case Direction.Left:
                    if (AboutButton.IsHighlighted)
                    {
                        AboutButton.UnhighlightButton();
                        PlayButton.HighlightButton();
                    }
                    else if (PlayButton.IsHighlighted)
                    {
                        PlayButton.UnhighlightButton();
                        ExitButton.HighlightButton();
                    }
                    break;
                case Direction.Right:
                    if (ExitButton.IsHighlighted)
                    {
                        ExitButton.UnhighlightButton();
                        PlayButton.HighlightButton();
                    }
                    else if (PlayButton.IsHighlighted)
                    {
                        PlayButton.UnhighlightButton();
                        AboutButton.HighlightButton();
                    }
                    break;
                case Direction.Down:
                    if (PlayButton.IsHighlighted)
                    {
                        PlayButton.UnhighlightButton();
                        if (MovementInput.X > 0)
                        {
                            AboutButton.HighlightButton();
                        }
                        else
                        {
                            ExitButton.HighlightButton();
                        }
                    }
                    break;
                case Direction.Up:
                    if (ExitButton.IsHighlighted || AboutButton.IsHighlighted)
                    {
                        ExitButton.UnhighlightButton();
                        AboutButton.UnhighlightButton();
                        PlayButton.HighlightButton();
                    }
                    break;
            }
        }

        private Direction GetDesiredDirection()
        {

            Direction desiredDirection = Direction.None;

            if (MovementInput != null && FlatRedBall.TimeManager.CurrentTime - lastMovementTime > timeBetweenMovement)
            {
                var x = MovementInput.X;
                var y = MovementInput.Y;
                if (Math.Abs(x) > Math.Abs(y))
                {
                    y = 0;
                }
                else if (Math.Abs(x) < Math.Abs(y))
                {
                    x = 0;
                }

                if (x < 0)
                {
                    desiredDirection = Direction.Left;
                }
                else if (x > 0)
                {
                    desiredDirection = Direction.Right;
                }
                else if (y < 0)
                {
                    desiredDirection = Direction.Down;
                }
                else if (y > 0)
                {
                    desiredDirection = Direction.Up;
                }
            }

            if (desiredDirection != Direction.None)
            {
                lastMovementTime = FlatRedBall.TimeManager.CurrentTime;
            }

            return desiredDirection;
        }

        void CustomDestroy()
		{

        }

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

	}
}
