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
		void OnBackButtonClickTunnel (FlatRedBall.Gui.IWindow window)
		{
			if (this.BackButtonClick != null)
			{
				BackButtonClick(window);
			}
		}
		void OnGoFishButtonClickTunnel (FlatRedBall.Gui.IWindow window)
		{
			if (this.GoFishButtonClick != null)
			{
				GoFishButtonClick(window);
			}
		}
		void OnGearButtonClickTunnel (FlatRedBall.Gui.IWindow window)
		{
			if (this.GearButtonClick != null)
			{
				GearButtonClick(window);
			}
		}
		void OnStoreButtonClickTunnel (FlatRedBall.Gui.IWindow window)
		{
			if (this.StoreButtonClick != null)
			{
				StoreButtonClick(window);
			}
		}
		void OnFishopediaButtonClickTunnel (FlatRedBall.Gui.IWindow window)
		{
			if (this.FishopediaButtonClick != null)
			{
				FishopediaButtonClick(window);
			}
		}
	}
}
