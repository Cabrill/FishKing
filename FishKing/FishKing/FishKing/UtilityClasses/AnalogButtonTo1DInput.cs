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
        private AnalogButton button;

        public AnalogButtonTo1DInput(AnalogButton b)
        {
            button = b;
        }

        public float Value
        {
            get
            {
                return button.Position;
            }
        }

        public float Velocity
        {
            get
            {
                return button.Velocity;
            }
        }
    }
}
