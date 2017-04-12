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
	public partial class FlatRedBallSplash
	{
        private double secondsToWait;
        private double startTime;

		void CustomInitialize()
		{
            startTime = FlatRedBall.TimeManager.CurrentTime;
		}

		void CustomActivity(bool firstTimeCalled)
		{
            if (FlatRedBall.TimeManager.CurrentTime - startTime >= secondsToWait)
            {
                if (this.AsyncLoadingState == FlatRedBall.Screens.AsyncLoadingState.NotStarted)
                {
                    StartAsyncLoad(typeof(MainMenuScreen).FullName);
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
