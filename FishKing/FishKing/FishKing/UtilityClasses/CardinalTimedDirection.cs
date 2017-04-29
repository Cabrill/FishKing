using FishKing.Entities;
using FlatRedBall.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.UtilityClasses
{
    public static class CardinalTimedDirection
    {
        private static double lastMovementTime = 0;
        private static double timeBetweenMovement = 0.2;
        public static Direction GetDesiredDirection(I2DInput movementInput)
        {
            Direction desiredDirection = Direction.None;

            if (movementInput != null && FlatRedBall.TimeManager.CurrentTime - lastMovementTime > timeBetweenMovement)
            {
                var x = movementInput.X;
                var y = movementInput.Y;
                if (Math.Abs(x) > Math.Abs(y))
                {
                    y = 0;
                }
                else if (Math.Abs(x) < Math.Abs(y))
                {
                    x = 0;
                }

                if (x < 0)
                {
                    desiredDirection = Direction.Left;
                }
                else if (x > 0)
                {
                    desiredDirection = Direction.Right;
                }
                else if (y < 0)
                {
                    desiredDirection = Direction.Down;
                }
                else if (y > 0)
                {
                    desiredDirection = Direction.Up;
                }
            }

            if (desiredDirection != Direction.None)
            {
                lastMovementTime = FlatRedBall.TimeManager.CurrentTime;
            }

            return desiredDirection;
        }
    }
}
