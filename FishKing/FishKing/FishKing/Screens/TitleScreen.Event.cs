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
	public partial class TitleScreen
	{
        void OnPlayButtonClick (FlatRedBall.Gui.IWindow window)
        {
            LoadingScreen.TransitionToScreen(typeof(GameScreen).FullName);
        }
		
	}
}
