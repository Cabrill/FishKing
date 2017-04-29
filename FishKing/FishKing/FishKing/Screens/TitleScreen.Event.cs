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
            this.MoveToScreen(NextScreen);   
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
		
	}
}
