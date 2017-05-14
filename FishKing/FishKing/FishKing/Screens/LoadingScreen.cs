using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using FlatRedBall.AI.Pathfinding;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Graphics.Particle;
using FlatRedBall.Math.Geometry;
using FlatRedBall.Localization;



namespace FishKing.Screens
{
	public partial class LoadingScreen
	{

		void CustomInitialize()
		{
            LoadingScreenComponentInstance.SpinFishAnimation.Play();
		}

		void CustomActivity(bool firstTimeCalled)
		{
            if (this.NextScreen != null)
            {
                if (this.AsyncLoadingState == FlatRedBall.Screens.AsyncLoadingState.NotStarted)
                {
                    StartAsyncLoad(NextScreen);
                }
                else if (this.AsyncLoadingState == FlatRedBall.Screens.AsyncLoadingState.Done)
                {
                    IsActivityFinished = true;
                }
            }
        }

		void CustomDestroy()
		{


		}

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

	}
}
