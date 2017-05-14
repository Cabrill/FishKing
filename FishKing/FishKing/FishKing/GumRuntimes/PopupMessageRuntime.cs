using FishKing.Entities;
using Microsoft.Xna.Framework;
using RenderingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.GumRuntimes
{
    partial class PopupMessageRuntime
    {
        private float scrollAmount;
        private float messageHeight;
        private float containerHeight;

        partial void CustomInitialize()
        {
            if (CurrentPopupDisplayState == PopupDisplay.JustOK)
            {
                OKButton.HighlightButton();
            }
            else if (CurrentPopupDisplayState == PopupDisplay.OKCancel)
            {
                CancelButton.HighlightButton();
            }
            else if (CurrentPopupDisplayState == PopupDisplay.YesNo)
            {
                YesButton.HighlightButton();
            }
            else if (CurrentPopupDisplayState == PopupDisplay.YesNoCancel)
            {
                CancelButton.HighlightButton();
            }
        }

        public void HandleMovement(Direction directionDesired)
        {
            switch (directionDesired)
            {
                case Direction.Left:
                    if (CurrentPopupDisplayState == PopupDisplay.OKCancel)
                    {
                        OKButton.HighlightButton();
                        CancelButton.UnhighlightButton();
                    }
                    else if (CurrentPopupDisplayState == PopupDisplay.YesNo)
                    {
                        NoButton.UnhighlightButton();
                        YesButton.HighlightButton();
                    }
                    else if (CurrentPopupDisplayState == PopupDisplay.YesNoCancel)
                    {
                        if (NoButton.IsHighlighted)
                        {
                            NoButton.UnhighlightButton();
                            YesButton.HighlightButton();
                        }
                        else if (CancelButton.IsHighlighted)
                        {
                            CancelButton.UnhighlightButton();
                            NoButton.HighlightButton();
                        }
                    }
                    break;
                case Direction.Right:
                    if (CurrentPopupDisplayState == PopupDisplay.OKCancel)
                    {
                        OKButton.UnhighlightButton();
                        CancelButton.HighlightButton();
                    }
                    else if (CurrentPopupDisplayState == PopupDisplay.YesNo)
                    {
                        NoButton.HighlightButton();
                        YesButton.UnhighlightButton();
                    }
                    else if (CurrentPopupDisplayState == PopupDisplay.YesNoCancel)
                    {
                        if (NoButton.IsHighlighted)
                        {
                            NoButton.UnhighlightButton();
                            CancelButton.HighlightButton();
                        }
                        else if (YesButton.IsHighlighted)
                        {
                            YesButton.UnhighlightButton();
                            NoButton.HighlightButton();
                        }
                    }
                    break;
                default: break;
            }
        }

        public void HandleSelection()
        {
            switch (CurrentPopupDisplayState)
            {
                case PopupDisplay.JustOK:
                    OKButton.CallClick(); break;
                case PopupDisplay.OKCancel:
                    if (OKButton.IsHighlighted) OKButton.CallClick();
                    else if (CancelButton.IsHighlighted) CancelButton.CallClick();
                    break;
                case PopupDisplay.YesNo:
                    if (YesButton.IsHighlighted) YesButton.CallClick();
                    else if (NoButton.IsHighlighted) NoButton.CallClick();
                    break;
                case PopupDisplay.YesNoCancel:
                    if (YesButton.IsHighlighted) YesButton.CallClick();
                    else if (NoButton.IsHighlighted) NoButton.CallClick();
                    else if (CancelButton.IsHighlighted) CancelButton.CallClick();
                    break;
                default: break;
            }
        }

        public void HandleScrollInput(float scrollWheel)
        {
            SetScrollAmount(scrollAmount + (scrollWheel * 10));
        }

        public void MeasureComponents()
        {
            scrollAmount = 0;
            messageHeight = (PopupTextInstance as IPositionedSizedObject).Height;
            containerHeight = (TopTextContainer as IPositionedSizedObject).Height;
        }

        private void SetScrollAmount(float scroll)
        {
            scrollAmount = MathHelper.Clamp(scroll, -containerHeight*3, 0);
            PopupTextContainer.Y = 100 * scrollAmount / containerHeight;
        }

        public void HandleExit()
        {
            switch (CurrentPopupDisplayState)
            {
                case PopupDisplay.JustOK:
                    OKButton.CallClick(); break;
                case PopupDisplay.OKCancel:
                    CancelButton.CallClick();
                    break;
                case PopupDisplay.YesNo:
                    NoButton.CallClick();
                    break;
                case PopupDisplay.YesNoCancel:
                    CancelButton.CallClick();
                    break;
                default: break;
            }
        }

    }
}
