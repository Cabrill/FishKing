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
	public partial class Character
	{
        void OnAfterZSet (object sender, EventArgs e)
        {
            ShadowInstance.Z = this.Z - 1.5f;
        }
	}
}
