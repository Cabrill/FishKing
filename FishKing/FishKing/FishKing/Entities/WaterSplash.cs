using System;
using System.Collections.Generic;
using System.Text;
using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using FlatRedBall.AI.Pathfinding;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Graphics.Particle;
using FlatRedBall.Math.Geometry;

namespace FishKing.Entities
{
	public partial class WaterSplash
	{
        public bool JustCycled => this.SpriteInstance.JustCycled;
        public int CurrentFrameIndex
        {
            get { return this.SpriteInstance.CurrentFrameIndex; }
            set { this.SpriteInstance.CurrentFrameIndex = value; }
        }

        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
		private void CustomInitialize()
		{
            Visible = false;
		}

		private void CustomActivity()
		{
            if (JustCycled)
            {
                Visible = false;
                IgnoreParentPosition = false;
            }
		}

        public void Play()
        {
            this.Position = Parent.Position;
            var thisSplah = this;
            IgnoreParentPosition = true;
            Visible = true;
            CurrentFrameIndex = 0;
        }

		private void CustomDestroy()
		{


		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
	}
}
