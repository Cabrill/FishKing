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
		void OnPlayButtonClickTunnel (FlatRedBall.Gui.IWindow window)
		{
			if (this.PlayButtonClick != null)
			{
				PlayButtonClick(window);
			}
		}
		void OnExitButtonClickTunnel (FlatRedBall.Gui.IWindow window)
		{
			if (this.ExitButtonClick != null)
			{
				ExitButtonClick(window);
			}
		}
		void OnAboutPopupOKButtonClickTunnel (FlatRedBall.Gui.IWindow window)
		{
			if (this.AboutPopupOKButtonClick != null)
			{
				AboutPopupOKButtonClick(window);
			}
		}
		void OnAboutButtonClickTunnel (FlatRedBall.Gui.IWindow window)
		{
			if (this.AboutButtonClick != null)
			{
				AboutButtonClick(window);
			}
		}
		void OnPlayBackButtonClickTunnel (FlatRedBall.Gui.IWindow window)
		{
			if (this.PlayBackButtonClick != null)
			{
				PlayBackButtonClick(window);
			}
		}
		void OnNewGameButtonClickTunnel (FlatRedBall.Gui.IWindow window)
		{
			if (this.NewGameButtonClick != null)
			{
				NewGameButtonClick(window);
			}
		}
		void OnContinueButtonClickTunnel (FlatRedBall.Gui.IWindow window)
		{
			if (this.ContinueButtonClick != null)
			{
				ContinueButtonClick(window);
			}
		}
		void OnNewGameBackButtonClickTunnel (FlatRedBall.Gui.IWindow window)
		{
			if (this.NewGameBackButtonClick != null)
			{
				NewGameBackButtonClick(window);
			}
		}
		void OnSaveGamePreview1ClickTunnel (FlatRedBall.Gui.IWindow window)
		{
			if (this.SaveGamePreview1Click != null)
			{
				SaveGamePreview1Click(window);
			}
		}
		void OnSaveGamePreview2ClickTunnel (FlatRedBall.Gui.IWindow window)
		{
			if (this.SaveGamePreview2Click != null)
			{
				SaveGamePreview2Click(window);
			}
		}
		void OnSaveGamePreview3ClickTunnel (FlatRedBall.Gui.IWindow window)
		{
			if (this.SaveGamePreview3Click != null)
			{
				SaveGamePreview3Click(window);
			}
		}
		void OnSaveGameBackButtonClickTunnel (FlatRedBall.Gui.IWindow window)
		{
			if (this.SaveGameBackButtonClick != null)
			{
				SaveGameBackButtonClick(window);
			}
		}
		void OnNewGameDisplayInstanceClickTunnel (FlatRedBall.Gui.IWindow window)
		{
			if (this.NewGameDisplayInstanceClick != null)
			{
				NewGameDisplayInstanceClick(window);
			}
		}
	}
}
