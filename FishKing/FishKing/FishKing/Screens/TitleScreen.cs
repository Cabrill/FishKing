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
using FishKing.UtilityClasses;
using System.Threading.Tasks;
using FishKing.Managers;
using FishKing.GameClasses;

namespace FishKing.Screens
{
	public partial class TitleScreen
	{
        Multiple2DInputs MovementInput;
        MultiplePressableInputs SelectionInput;
        MultiplePressableInputs ExitInput;
        TitleScreenGumRuntime screen;

        private enum PlayType { None, Continue, NewGame };
        PlayType currentPlayType;
        int currentSaveSlot = 0;

        void CustomInitialize()
		{
            InitializeInput();
            currentPlayType = PlayType.None;
            screen = TitleScreenGumRuntime;
            screen.IntroAnimation.Play();
            FlatRedBallServices.Game.IsMouseVisible = true;
            Microsoft.Xna.Framework.Media.MediaPlayer.Volume = 0.3f;
            FlatRedBall.Audio.AudioManager.PlaySong(Echinoderm_Regeneration_Sting, true, false);
            LoadSaveData();
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
            this.Call(startMusic).After(3);
        }

        private async void LoadSaveData()
        {
            SaveGamePreview1.CurrentFilledState = GameSelectPreviewRuntime.Filled.Empty;
            SaveGamePreview2.CurrentFilledState = GameSelectPreviewRuntime.Filled.Empty;
            SaveGamePreview3.CurrentFilledState = GameSelectPreviewRuntime.Filled.Empty;

            var saveGames = await SaveGameManager.GetAllSaves();
            foreach (SaveFileData save in saveGames)
            {
                switch (save.SaveSlotNumber)
                {
                    case 1: SaveGamePreview1.AssociatedWithSaveGame(save); break;
                    case 2: SaveGamePreview2.AssociatedWithSaveGame(save); break;
                    case 3: SaveGamePreview3.AssociatedWithSaveGame(save); break;
                }
            }
            if (saveGames.Count == 0)
            {
                ContinueButton.CurrentEnabledState = MainMenuButtonRuntime.Enabled.IsDisabled;
            }
            else
            {
                ContinueButton.CurrentEnabledState = MainMenuButtonRuntime.Enabled.IsEnabled;
            }
        }

        void startMusic()
        {
            FlatRedBall.Audio.AudioManager.PlaySong(The_Low_Seas, true, false);
        }

        void CustomActivity(bool firstTimeCalled)
		{
            if (InputManager.Mouse.AnyButtonPushed())
            {
                NewGameDisplayInstance.TestCollision(GuiManager.Cursor);
            }

            HandleMenuMovement();
            HandleMenuSelection();
            HandleExitInput();

            //FlatRedBall.Debugging.Debugger.Write(FlatRedBall.Gui.GuiManager.Cursor.WindowOver);
        }

        private void HandleMenuSelection()
        {
            if (SelectionInput.WasJustPressed)
            {
                if (AboutPopup.Visible)
                {
                    AboutPopup.HandleSelection();
                }
                else if (PopupMessageInstance.Visible)
                {
                    PopupMessageInstance.HandleSelection();
                }
                else if (screen.CurrentMenuScreenButtonsState == TitleScreenGumRuntime.MenuScreenButtons.InitialButtons)
                {
                    HandleInitialScreenSelection();
                }
                else if (TitleScreenGumRuntime.CurrentMenuScreenButtonsState == TitleScreenGumRuntime.MenuScreenButtons.PlayButtons)
                {
                    HandlePlayScreenSelection();
                }
                else if (TitleScreenGumRuntime.CurrentMenuScreenButtonsState == TitleScreenGumRuntime.MenuScreenButtons.NewGameButtons)
                {
                    HandleNewGameScreenSelection();
                }
                else if (TitleScreenGumRuntime.CurrentMenuScreenButtonsState == TitleScreenGumRuntime.MenuScreenButtons.SaveGameButtons)
                {
                    HandleSaveGameScreenSelection();
                }
            }
        }

        private void HandleMenuMovement()
        {
            var desiredDirection = CardinalTimedDirection.GetDesiredDirection(MovementInput);

            if (desiredDirection == Direction.None)
            {
                return;
            }

            if (AboutPopup.Visible)
            {
                AboutPopup.HandleMovement(desiredDirection);
            }
            else if (PopupMessageInstance.Visible)
            {
                PopupMessageInstance.HandleMovement(desiredDirection);
            }
            else if (screen.CurrentMenuScreenButtonsState == TitleScreenGumRuntime.MenuScreenButtons.InitialButtons)
            {
                HandleInitialScreenMovement(desiredDirection);
            }
            else if (TitleScreenGumRuntime.CurrentMenuScreenButtonsState == TitleScreenGumRuntime.MenuScreenButtons.PlayButtons)
            {
                HandlePlayScreenMovement(desiredDirection);
            } else if (TitleScreenGumRuntime.CurrentMenuScreenButtonsState == TitleScreenGumRuntime.MenuScreenButtons.SaveGameButtons)
            {
                HandleSaveGameScreenMovement(desiredDirection);
            }
            else if (TitleScreenGumRuntime.CurrentMenuScreenButtonsState == TitleScreenGumRuntime.MenuScreenButtons.NewGameButtons)
            {
                HandleNewGameScreenMovement(desiredDirection);
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
                else if (PopupMessageInstance.Visible)
                {
                    PopupMessageInstance.HandleExit();
                }
                else
                {
                    if (screen.CurrentMenuScreenButtonsState == TitleScreenGumRuntime.MenuScreenButtons.InitialButtons)
                    {
                        ExitButton.CallClick();
                    }
                    else if (screen.CurrentMenuScreenButtonsState == TitleScreenGumRuntime.MenuScreenButtons.SaveGameButtons)
                    {
                        SaveGameBackButton.CallClick();
                    }
                    else if (screen.CurrentMenuScreenButtonsState == TitleScreenGumRuntime.MenuScreenButtons.NewGameButtons)
                    {
                        if (NewGameDisplayInstance.CurrentConfirmationState == NewGameDisplayRuntime.Confirmation.Confirming)
                        {
                            NewGameDisplayInstance.CurrentConfirmationState = NewGameDisplayRuntime.Confirmation.Unconfirmed;
                        }
                        else
                        {
                            NewGameBackButton.CallClick();
                        }
                    }
                }
            }
        }

        private void HandleNewGameScreenSelection()
        {
            if (NewGameBackButton.IsHighlighted) NewGameBackButton.CallClick();
            else NewGameDisplayInstance.MainClick();
        }

        private void HandleSaveGameScreenSelection()
        {
            if (SaveGameBackButton.IsHighlighted) SaveGameBackButton.CallClick();
            else if (SaveGamePreview1.IsHighlighted) SaveGamePreview1.CallClick();
            else if (SaveGamePreview2.IsHighlighted) SaveGamePreview2.CallClick();
            else if (SaveGamePreview3.IsHighlighted) SaveGamePreview3.CallClick();
        }

        private void HandlePlayScreenSelection()
        {
            if (PlayBackButton.IsHighlighted) PlayBackButton.CallClick();
            else if (NewGameButton.IsHighlighted) NewGameButton.CallClick();
            else if (ContinueButton.IsHighlighted) ContinueButton.CallClick();
        }

        private void HandleInitialScreenSelection()
        {
            if (PlayButton.IsHighlighted) PlayButton.CallClick();
            else if (AboutButton.IsHighlighted) AboutButton.CallClick();
            else if (ExitButton.IsHighlighted) ExitButton.CallClick();
        }


        private void HandleNewGameScreenMovement(Direction desiredDirection)
        {
            switch (desiredDirection)
            {
                case Direction.Left:
                    NewGameDisplayInstance.LeftClick(); break;
                case Direction.Right:
                    NewGameDisplayInstance.RightClick(); break;
                case Direction.Down:
                    NewGameDisplayInstance.UnhighlightButton();
                    NewGameBackButton.HighlightButton(); break;
                case Direction.Up:
                    NewGameDisplayInstance.HighlightButton();
                    NewGameBackButton.UnhighlightButton(); break;
            }
        }

        private void HandleSaveGameScreenMovement(Direction desiredDirection)
        {
            switch (desiredDirection)
            {
                case Direction.Left:
                    if (SaveGamePreview3.IsHighlighted)
                    {
                        SaveGamePreview3.UnhighlightButton();
                        SaveGamePreview2.HighlightButton();
                    } else if (SaveGamePreview2.IsHighlighted)
                    {
                        SaveGamePreview2.UnhighlightButton();
                        SaveGamePreview1.HighlightButton();
                    } else if (SaveGamePreview1.IsHighlighted)
                    {
                        SaveGamePreview1.UnhighlightButton();
                        SaveGameBackButton.HighlightButton();
                    }
                    break;
                case Direction.Right:
                    if (SaveGameBackButton.IsHighlighted)
                    {
                        SaveGameBackButton.UnhighlightButton();
                        SaveGamePreview1.HighlightButton();
                    }
                    else if (SaveGamePreview1.IsHighlighted)
                    {
                        SaveGamePreview1.UnhighlightButton();
                        SaveGamePreview2.HighlightButton();
                    }
                    else if (SaveGamePreview2.IsHighlighted)
                    {
                        SaveGamePreview2.UnhighlightButton();
                        SaveGamePreview3.HighlightButton();
                    }
                    break;
            }
        }

        private void HandlePlayScreenMovement(Direction desiredDirection)
        {
            switch (desiredDirection)
            {
                case Direction.Left:
                    if (ContinueButton.IsHighlighted)
                    {
                        ContinueButton.UnhighlightButton();
                        NewGameButton.HighlightButton();
                    }
                    else if (NewGameButton.IsHighlighted)
                    {
                        NewGameButton.UnhighlightButton();
                        PlayBackButton.HighlightButton();
                    }
                    break;
                case Direction.Right:
                    if (PlayBackButton.IsHighlighted)
                    {
                        PlayBackButton.UnhighlightButton();
                        NewGameButton.HighlightButton();
                    }
                    else if (NewGameButton.IsHighlighted)
                    {
                        if (ContinueButton.CurrentEnabledState != MainMenuButtonRuntime.Enabled.IsDisabled)
                        {
                            NewGameButton.UnhighlightButton();
                            ContinueButton.HighlightButton();
                        }
                    }
                    break;
                case Direction.Down:
                    if (NewGameButton.IsHighlighted)
                    {
                        NewGameButton.UnhighlightButton();
                        if (MovementInput.X > 0 && ContinueButton.CurrentEnabledState != MainMenuButtonRuntime.Enabled.IsDisabled)
                        {
                            ContinueButton.HighlightButton();
                        }
                        else
                        {
                            PlayBackButton.HighlightButton();
                        }
                    }
                    break;
                case Direction.Up:
                    if (PlayBackButton.IsHighlighted || ContinueButton.IsHighlighted)
                    {
                        PlayBackButton.UnhighlightButton();
                        ContinueButton.UnhighlightButton();
                        NewGameButton.HighlightButton();
                    }
                    break;
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
        
        void CustomDestroy()
		{

        }

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }
	}
}
