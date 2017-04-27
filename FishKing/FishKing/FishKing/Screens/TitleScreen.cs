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
using Microsoft.Xna.Framework.Input;
using FlatRedBall.Gui;

namespace FishKing.Screens
{
	public partial class TitleScreen
	{
        Multiple2DInputs buttonMovementInput;
        MultiplePressableInputs buttonSelectionInput;

		void CustomInitialize()
		{
            InitializeInput();
            TitleScreenGumRuntime.IntroAnimation.Play();
            FlatRedBallServices.Game.IsMouseVisible = true;

        }

        private void InitializeInput()
        {
            var gamePad = InputManager.Xbox360GamePads[0];

            var movementInputs = new Multiple2DInputs();
            movementInputs.Inputs.Add(InputManager.Keyboard.Get2DInput(
                Keys.A, Keys.D, Keys.W, Keys.S));
            if (InputManager.NumberOfConnectedGamePads > 0)
            {
                movementInputs.Inputs.Add(gamePad.DPad);
                movementInputs.Inputs.Add(gamePad.LeftStick);
            }
            buttonMovementInput = movementInputs;

            var actionInputs = new MultiplePressableInputs();
            actionInputs.Inputs.Add(InputManager.Keyboard.GetKey(Keys.Space));
            if (InputManager.NumberOfConnectedGamePads > 0)
            {
                actionInputs.Inputs.Add(InputManager.Xbox360GamePads[0].GetButton(Xbox360GamePad.Button.A));
            }
            buttonSelectionInput = actionInputs;
        }

        void CustomActivity(bool firstTimeCalled)
		{
            HandleMenuMovement();
            
        }

        private void HandleMenuMovement()
        {
            //throw new NotImplementedException();
        }

        void CustomDestroy()
		{

        }

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

	}
}
