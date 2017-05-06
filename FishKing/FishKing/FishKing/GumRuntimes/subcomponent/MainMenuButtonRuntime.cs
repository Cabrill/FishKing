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
        public bool IsHighlighted
        {
            get { return CurrentHighlightState == Highlight.Highlighted; }
        }

        partial void CustomInitialize()
        {
            this.RollOn += HighlightButton;
            this.RollOff += UnhighlightButton;
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
            if (CurrentEnabledState != MainMenuButtonRuntime.Enabled.IsDisabled)
            {
                this.CurrentHighlightState = Highlight.Highlighted;
            }
        }

        public void UnhighlightButton()
        {
            if (CurrentEnabledState != MainMenuButtonRuntime.Enabled.IsDisabled)
            {
                this.CurrentHighlightState = Highlight.NotHighlighted;
            }
        }
    }
}
