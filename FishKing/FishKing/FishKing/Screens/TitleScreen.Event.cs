using System;
using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Specialized;
using FlatRedBall.Audio;
using FlatRedBall.Screens;
using FishKing.Entities;
using FishKing.Screens;
using FishKing.Managers;
using FishKing.GameClasses;
using FishKing.GumRuntimes;

namespace FishKing.Screens
{
	public partial class TitleScreen
	{
        //Menu forward buttons
        void OnPlayButtonClick (FlatRedBall.Gui.IWindow window)
        {
            screen.CurrentMenuScreenButtonsState = GumRuntimes.TitleScreenGumRuntime.MenuScreenButtons.PlayButtons;
        }
        void OnNewGameButtonClick(FlatRedBall.Gui.IWindow window)
        {
            currentPlayType = PlayType.NewGame;
            screen.CurrentMenuScreenButtonsState = GumRuntimes.TitleScreenGumRuntime.MenuScreenButtons.SaveGameButtons;
            ChooseSlotText.Visible = true;
        }
        void OnContinueButtonClick(FlatRedBall.Gui.IWindow window)
        {
            if (ContinueButton.CurrentEnabledState != MainMenuButtonRuntime.Enabled.IsDisabled)
            {
                currentPlayType = PlayType.Continue;
                screen.CurrentMenuScreenButtonsState = GumRuntimes.TitleScreenGumRuntime.MenuScreenButtons.SaveGameButtons;
                ChooseSlotText.Visible = false;
            }
        }

        //Menu return buttons
        void OnPlayBackButtonClick(FlatRedBall.Gui.IWindow window)
        {
            screen.CurrentMenuScreenButtonsState = GumRuntimes.TitleScreenGumRuntime.MenuScreenButtons.InitialButtons;
        }
        void OnExitButtonClick (FlatRedBall.Gui.IWindow window)
        {
            FlatRedBallServices.Game.Exit();
        }
        void OnAboutPopupOKButtonClick (FlatRedBall.Gui.IWindow window)
        {
            AboutPopup.Visible = false;
        }
        void OnAboutButtonClick (FlatRedBall.Gui.IWindow window)
        {
            AboutPopup.Visible = true;
        }
        void OnNewGameBackButtonClick(FlatRedBall.Gui.IWindow window)
        {
            screen.CurrentMenuScreenButtonsState = GumRuntimes.TitleScreenGumRuntime.MenuScreenButtons.SaveGameButtons;
        }
        void OnSaveGameBackButtonClick(FlatRedBall.Gui.IWindow window)
        {
            screen.CurrentMenuScreenButtonsState = GumRuntimes.TitleScreenGumRuntime.MenuScreenButtons.PlayButtons;
            currentPlayType = PlayType.None;
        }


        //Game start buttons
        void OnNewGameDisplayInstanceFishContainerClick(FlatRedBall.Gui.IWindow window)
        {
            if (NewGameDisplayInstance.CurrentConfirmationState == GumRuntimes.NewGameDisplayRuntime.Confirmation.Unconfirmed)
            {
                NewGameDisplayInstance.CurrentConfirmationState = GumRuntimes.NewGameDisplayRuntime.Confirmation.Confirming;
            }
            else
            {
                var newSaveGame = new SaveFileData(NewGameDisplayInstance.PlayerFishChoice, currentSaveSlot);
                SaveGameManager.SetCurrentData(newSaveGame);
                startGame();
            }
        }

        void OnSaveGamePreview1Click(FlatRedBall.Gui.IWindow callingWindow)
        {
            onSaveGamePreviewClick(SaveGamePreview1, 1);
        }
        void OnSaveGamePreview2Click (FlatRedBall.Gui.IWindow window)
        {
            onSaveGamePreviewClick(SaveGamePreview2, 2);
        }
        void OnSaveGamePreview3Click(FlatRedBall.Gui.IWindow window)
        {
            onSaveGamePreviewClick(SaveGamePreview3, 3);
        }

        void onSaveGamePreviewClick(GameSelectPreviewRuntime savePreview, int saveSlot)
        {
            if (currentPlayType == PlayType.Continue && savePreview.SaveData != null)
            {
                SaveGameManager.SetCurrentData(savePreview.SaveData);
                startGame();
            }
            else if (currentPlayType == PlayType.NewGame)
            {
                NewGameDisplayInstance.CurrentConfirmationState = GumRuntimes.NewGameDisplayRuntime.Confirmation.Unconfirmed;
                currentSaveSlot = saveSlot;
                if (savePreview.SaveData != null)
                {
                    ConfirmChoice();
                }
                else
                {
                    screen.CurrentMenuScreenButtonsState = GumRuntimes.TitleScreenGumRuntime.MenuScreenButtons.NewGameButtons;
                }
            }
        }

        private void startGame()
        {
            FlatRedBallServices.Game.IsMouseVisible = false;
            this.MoveToScreen(NextScreen);
        }

        private void ConfirmChoice()
        {
            PopupMessageInstance.CurrentPopupDisplayState = GumRuntimes.PopupMessageRuntime.PopupDisplay.YesNo;
            PopupMessageInstance.CurrentTitleBarState = GumRuntimes.PopupMessageRuntime.TitleBar.TitlePresent;

            PopupMessageInstance.TitleText = "Overwrite Game";
            PopupMessageInstance.PopupText = "There is already a save file in this slot.  If you continue it will be erased.  Are you sure you wish to overwrite it?";

            PopupMessageInstance.YesButtonClick += PopupMessageInstance_YesButtonClick;
            PopupMessageInstance.NoButtonClick += PopupMessageInstance_NoButtonClick;
        }

        private void PopupMessageInstance_NoButtonClick(FlatRedBall.Gui.IWindow window)
        {
            PopupMessageInstance.Visible = false;
        }

        private void PopupMessageInstance_YesButtonClick(FlatRedBall.Gui.IWindow window)
        {
            screen.CurrentMenuScreenButtonsState = GumRuntimes.TitleScreenGumRuntime.MenuScreenButtons.NewGameButtons;
        }       
    }
}
