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

namespace FishKing.Screens
{
	public partial class MainMenu
	{
        Multiple2DInputs MovementInput;
        MultiplePressableInputs SelectionInput;
        MultiplePressableInputs ExitInput;
        MainMenuGumRuntime screen;
        double lastMovementTime = 0;
        double timeBetweenMovement = 0.2;

        void CustomInitialize()
        {
            InitializeInput();
            screen = MainMenuGumRuntime;
            FlatRedBallServices.Game.IsMouseVisible = true;
            Microsoft.Xna.Framework.Media.MediaPlayer.Volume = 0.5f;
            //FlatRedBall.Audio.AudioManager.PlaySong(Echinoderm_Regeneration_Sting, true, false);
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
            FlatRedBall.Debugging.Debugger.Write(FlatRedBall.Gui.GuiManager.Cursor.WindowOver);
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
                //if (aboutpopup.visible)
                //{
                //    aboutpopup.handleexit();
                //}
                //else
                //{
                    BackButton.CallClick();
                //}
            }
        }

        private void HandleMenuSelection()
        {
            if (SelectionInput.WasJustPressed)
            {
                //if (AboutPopup.Visible)
                //{
                //    AboutPopup.HandleSelection();
                //}
                //else if (screen.CurrentMenuScreenButtonsState == TitleScreenGumRuntime.MenuScreenButtons.InitialButtons)
                //{
                    HandleInitialScreenSelection();
                //}
                //else if (TitleScreenGumRuntime.CurrentMenuScreenButtonsState == TitleScreenGumRuntime.MenuScreenButtons.PlayButtons)
                //{
                //    //TODO:  Add new game/continue
                //}
            }
        }

        private void HandleInitialScreenSelection()
        {
            if (GoFishButton.IsHighlighted)
            {
                GoFishButton.CallClick();
            }
            else if (BackButton.IsHighlighted)
            {
                BackButton.CallClick();
            }
        }

        private void HandleMenuMovement()
        {
            var desiredDirection = GetDesiredDirection();

            if (desiredDirection == Direction.None)
            {
                return;
            }

            //if (AboutPopup.Visible)
            //{
            //    AboutPopup.HandleMovement(desiredDirection);
            //}
            //else if (screen.CurrentMenuScreenButtonsState == TitleScreenGumRuntime.MenuScreenButtons.InitialButtons)
            //{
                HandleInitialScreenMovement(desiredDirection);
            //}
            //else if (TitleScreenGumRuntime.CurrentMenuScreenButtonsState == TitleScreenGumRuntime.MenuScreenButtons.PlayButtons)
            //{
            //    //TODO:  Add new game/continue
            //}
        }

        private void HandleInitialScreenMovement(Direction desiredDirection)
        {
            switch (desiredDirection)
            {
                case Direction.Left:
                    if (GoFishButton.IsHighlighted)
                    {
                        GoFishButton.UnhighlightButton();
                        BackButton.HighlightButton();
                    }
                    break;
                case Direction.Right:
                    if (BackButton.IsHighlighted)
                    {
                        BackButton.UnhighlightButton();
                        GoFishButton.HighlightButton();
                    }
                    break;
                case Direction.Down:
                    if (TournamentPreviewInstance.IsHighlighted)
                    {
                        TournamentPreviewInstance.UnhighlightButton();
                        if (MovementInput.X > 0)
                        {
                            GoFishButton.HighlightButton();
                        }
                        else
                        {
                            BackButton.HighlightButton();
                        }
                    }
                    break;
                case Direction.Up:
                    if (GoFishButton.IsHighlighted || BackButton.IsHighlighted)
                    {
                        GoFishButton.UnhighlightButton();
                        BackButton.UnhighlightButton();
                        TournamentPreviewInstance.HighlightButton();
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
