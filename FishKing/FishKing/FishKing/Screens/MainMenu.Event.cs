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

namespace FishKing.Screens
{
	public partial class MainMenu
	{
        void OnBackButtonClick (FlatRedBall.Gui.IWindow window)
        {
            SaveGameManager.CurrentSaveData.StopPlaySession ();
            SaveGameManager.SaveCurrentData();
            MoveToScreen(typeof(TitleScreen));
        }
        void OnGoFishButtonClick (FlatRedBall.Gui.IWindow window)
        {
            if (MainMenuGumRuntime.AnyTournamentIsSelected)
            {
                StartTournament(MainMenuGumRuntime.CurrentlySelectedTournament.Tournament);               
            }
        }
        void OnGearButtonClick (FlatRedBall.Gui.IWindow window)
        {
            
        }
        void OnStoreButtonClick (FlatRedBall.Gui.IWindow window)
        {
            
        }
        void OnFishopediaButtonClick (FlatRedBall.Gui.IWindow window)
        {
            FishopediaInstance.CaughtFish = SaveGameManager.CurrentSaveData.FishCaught;
            FishopediaInstance.LoadAllFish();
            FishopediaInstance.Visible = true;
            _bookOpenSound.Volume = OptionsManager.Options.SoundEffectsVolume;
            _bookOpenSound.Play();
        }

        private void StartTournament(TournamentStructure tournament)
        {
            TournamentManager.SetCurrentTournament(tournament);
            MusicManager.Volume = OptionsManager.Options.MusicVolume;
            MusicManager.LoadPlayListByMapName(tournament.MapName);
            LoadingScreen.TransitionToScreen(typeof(GameScreen).FullName);
        }
		
	}
}
