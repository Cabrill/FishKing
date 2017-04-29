using FishKing.Entities;
using FishKing.UtilityClasses;
using FlatRedBall.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.GumRuntimes
{
    partial class PauseMenuRuntime
    {
        partial void CustomInitialize()
        {
            
        }

        public void HandleMovement(I2DInput input)
        {
            var desiredDirection = CardinalTimedDirection.GetDesiredDirection(input);

            if (desiredDirection == Direction.None) return;

            switch (desiredDirection)
            {
                case Direction.Down:
                    if (ResumeGameButton.IsHighlighted)
                    {
                        ResumeGameButton.UnhighlightButton();
                        HelpButton.HighlightButton();
                    }
                    else if (HelpButton.IsHighlighted)
                    {
                        HelpButton.UnhighlightButton();
                        SettingsButton.HighlightButton();
                    }
                    else if (SettingsButton.IsHighlighted)
                    {
                        SettingsButton.UnhighlightButton();
                        ExitButton.HighlightButton();
                    }
                    break;
                case Direction.Up:
                    if (ExitButton.IsHighlighted)
                    {
                        ExitButton.UnhighlightButton();
                        SettingsButton.HighlightButton();
                    }
                    else if (SettingsButton.IsHighlighted)
                    {
                        SettingsButton.UnhighlightButton();
                        HelpButton.HighlightButton();
                    }
                    else if (HelpButton.IsHighlighted)
                    {
                        HelpButton.UnhighlightButton();
                        ResumeGameButton.HighlightButton();
                    }
                    break;
            }

        }

        public void HandleSelection(IPressableInput input)
        {
            if (input.WasJustPressed)
            {
                if (ResumeGameButton.IsHighlighted)
                {
                    ResumeGameButton.CallClick();
                }
                else if (SettingsButton.IsHighlighted)
                {
                    SettingsButton.CallClick();
                }
                else if (HelpButton.IsHighlighted)
                {
                    HelpButton.CallClick();
                }
                else if (ExitButton.IsHighlighted)
                {
                    ExitButton.CallClick();
                }
            }
        }
    }
}
