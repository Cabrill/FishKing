using FlatRedBall.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.GumRuntimes
{
    partial class TextEntryBoxRuntime
    {
        private IPressableInput keyboardInput;

        partial void CustomInitialize()
        {
            //keyboardInput = InputManager.Keyboard.GetStringTyped
        }
    }
}
