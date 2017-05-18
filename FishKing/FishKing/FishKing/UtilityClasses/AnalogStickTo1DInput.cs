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
        private readonly AnalogStick _analogStick;

        public AnalogStickTo1DInput(AnalogStick stick)
        {
            _analogStick = stick;
        }

        public float Value => _analogStick.Velocity.Y != 0 ? (float)_analogStick.Magnitude : 0;

        public float Velocity => _analogStick.Position.Y;
    }
}
