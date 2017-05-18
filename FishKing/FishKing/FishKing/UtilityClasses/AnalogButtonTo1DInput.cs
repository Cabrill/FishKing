using FlatRedBall.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.UtilityClasses
{
    class AnalogButtonTo1DInput : I1DInput
    {
        private readonly AnalogButton _button;

        public AnalogButtonTo1DInput(AnalogButton b)
        {
            _button = b;
        }

        public float Value => _button.Position;

        public float Velocity => _button.Velocity;
    }
}
