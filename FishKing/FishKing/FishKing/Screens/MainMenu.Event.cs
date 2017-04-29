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
namespace FishKing.Screens
{
	public partial class MainMenu
	{
        void OnBackButtonClick (FlatRedBall.Gui.IWindow window)
        {
            //this.IsActivityFinished = true;
            //this.IsMovingBack = true;
            HandleBackNavigation();
            
        }
        void OnGoFishButtonClick (FlatRedBall.Gui.IWindow window)
        {
            LoadingScreen.TransitionToScreen(typeof(GameScreen).FullName);
        }
        void OnGearButtonClick (FlatRedBall.Gui.IWindow window)
        {
            
        }
        void OnStoreButtonClick (FlatRedBall.Gui.IWindow window)
        {
            
        }
        void OnFishopediaButtonClick (FlatRedBall.Gui.IWindow window)
        {
            
        }
		
	}
}
