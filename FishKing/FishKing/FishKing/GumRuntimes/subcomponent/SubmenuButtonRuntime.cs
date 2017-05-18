using FlatRedBall.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.GumRuntimes
{
    partial class SubmenuButtonRuntime
    {
        public bool IsHighlighted => CurrentHighlightState == Highlight.Highlighted;

        partial void CustomInitialize()
        {
            this.RollOn += HighlightButton;
            this.RollOff += UnhighlightButton;
            this.CurrentHighlightState = Highlight.NotHighlighted;
        }

        private void UnhighlightButton(IWindow window)
        {
            UnhighlightButton();
        }

        private void HighlightButton(IWindow window)
        {
            HighlightButton();
        }

        public void HighlightButton()
        {
            this.CurrentHighlightState = Highlight.Highlighted;
        }

        public void UnhighlightButton()
        {
            this.CurrentHighlightState = Highlight.NotHighlighted;
        }
    }
}
