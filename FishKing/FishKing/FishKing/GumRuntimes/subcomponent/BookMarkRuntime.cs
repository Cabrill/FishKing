using FlatRedBall.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.GumRuntimes
{
    partial class BookMarkRuntime
    {
        public bool IsHighlighted
        {
            get { return CurrentHighlightState == Highlight.Highlighted; }
        }

        public bool IsSelected
        {
            get { return CurrentSelectionState == Selection.Selected; }
        }

        partial void CustomInitialize()
        {
            CurrentSelectionState = Selection.NotSelected;
            this.RollOn += HighlightButton;
            this.RollOff += UnhighlightButton;
            this.Click += BookMarkRuntime_Click;
        }

        private void BookMarkRuntime_Click(IWindow window)
        {
            Select();
        }

        public void Select()
        {
            CurrentSelectionState = Selection.Selected;
        }

        public void Unselect()
        {
            CurrentSelectionState = Selection.NotSelected;
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

            //Refresh values changed by highlight change
            CurrentSelectionState = CurrentSelectionState;
        }
    }
}
