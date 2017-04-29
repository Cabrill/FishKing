using FlatRedBall.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.UtilityClasses
{
    public class PressableTo1DInput : I1DInput
    {
        private IPressableInput pressableInput;

        public PressableTo1DInput(IPressableInput input)
        {
            this.pressableInput = input;
        }

        public float Value
        {
            get
            {
                return (pressableInput.IsDown ? 1f : 0f);
            }
        }

        public float Velocity
        {
            get
            {
                return (pressableInput.WasJustPressed ? 1f : 0f);
            }
        }
    }
}
