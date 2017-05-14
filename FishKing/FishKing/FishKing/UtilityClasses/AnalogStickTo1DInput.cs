using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlatRedBall.Input;

namespace FishKing.UtilityClasses
{
    public class AnalogStickTo1DInput : I1DInput
    {
        private AnalogStick analogStick;

        public AnalogStickTo1DInput(AnalogStick stick)
        {
            analogStick = stick;
        }

        public float Value
        {
            get
            {
                return (float)analogStick.Magnitude;
            }
        }

        public float Velocity
        {
            get
            {
                return analogStick.Velocity.Y;
            }
        }
    }
}
