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
using FishKing.Managers;

namespace FishKing.Screens
{
	public partial class FlatRedBallSplashScreen
	{
        private double startTime;
        private double displaySeconds = 3;

		void CustomInitialize()
		{
            startTime = FlatRedBall.TimeManager.CurrentTime;
            SetOptions();
		}

		void CustomActivity(bool firstTimeCalled)
		{
            if (FlatRedBall.TimeManager.CurrentTime - startTime > displaySeconds ||
                InputManager.Keyboard.AnyKeyPushed() ||
                (InputManager.NumberOfConnectedGamePads > 0 && InputManager.Xbox360GamePads[0].AnyButtonPushed()))
            {
                if (AsyncLoadingState == FlatRedBall.Screens.AsyncLoadingState.NotStarted)
                {
                    StartAsyncLoad(NextScreen);
                }
                else if (AsyncLoadingState == FlatRedBall.Screens.AsyncLoadingState.Done)
                {
                    MoveToScreen(NextScreen);
                }
            }
		}

        private async void SetOptions()
        {
            var saves = await SaveGameManager.GetAllSaves();
            var mostRecentSave = saves?.OrderBy(s => s.LastPlayed).FirstOrDefault();
            if (mostRecentSave != null)
            {
                OptionsManager.Options = mostRecentSave.Options;
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
