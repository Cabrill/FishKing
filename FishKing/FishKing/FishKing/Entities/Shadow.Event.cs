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
namespace FishKing.Entities
{
	public partial class Shadow
	{
        void OnAfterSpriteInstanceWidthSet (object sender, EventArgs e)
        {
            SpriteInstanceHeight = SpriteInstanceWidth / 3;
        }
		
	}
}
