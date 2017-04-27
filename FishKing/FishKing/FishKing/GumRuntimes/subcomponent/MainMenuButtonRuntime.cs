using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlatRedBall.Gui;

namespace FishKing.GumRuntimes
{
    partial class MainMenuButtonRuntime
    {
        partial void CustomInitialize()
        {
            this.RollOn += HighlightButton;
            this.RollOff += UnhighlightButton;
        }

        private void UnhighlightButton(IWindow window)
        {
            this.CurrentHighlightState = Highlight.NotHighlighted;
        }

        private void HighlightButton(IWindow window)
        {
            this.CurrentHighlightState = Highlight.Highlighted;
        }
    }
}
