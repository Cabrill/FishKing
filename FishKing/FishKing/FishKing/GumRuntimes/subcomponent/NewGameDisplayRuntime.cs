using FlatRedBall.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.GumRuntimes
{
    partial class NewGameDisplayRuntime
    {
        public int PlayerFishChoice {
            get
            {
                return (int)CurrentPlayerFishState;
            }
        }

        partial void CustomInitialize()
        {
            LeftArrow.Click += LeftArrow_Click;
            RightArrow.Click += RightArrow_Click;
            this.RollOn += HighlightButton;
            this.RollOff += UnhighlightButton;
        }

        public bool IsHighlighted
        {
            get { return CurrentHighlightState == Highlight.Highlighted; }
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

        public void MainClick()
        {
            FishContainer.CallClick();
        }

        public void LeftClick()
        {
            if (CurrentConfirmationState == Confirmation.Unconfirmed)
            {
                LeftArrow.CallClick();
            }
        }

        public void RightClick()
        {
            if (CurrentConfirmationState == Confirmation.Unconfirmed)
            {
                RightArrow.CallClick();
            }
        }

        private void RightArrow_Click(FlatRedBall.Gui.IWindow window)
        {
            this.CurrentPlayerFishState = IntToPlayerFish((int)CurrentPlayerFishState + 1);
        }   

        private void LeftArrow_Click(FlatRedBall.Gui.IWindow window)
        {
            this.CurrentPlayerFishState = IntToPlayerFish((int)CurrentPlayerFishState - 1);
        }

        private PlayerFish IntToPlayerFish(int fish)
        {
            int newFish = mod(fish, 8);

            switch (newFish)
            {
                case 0: return PlayerFish.Fish1;
                case 1: return PlayerFish.Fish2;
                case 2: return PlayerFish.Fish3;
                case 3: return PlayerFish.Fish4;
                case 4: return PlayerFish.Fish5;
                case 5: return PlayerFish.Fish6;
                case 6: return PlayerFish.Fish7;
                case 7: return PlayerFish.Fish8;
                default: throw new IndexOutOfRangeException("Fish doesn't exist for " + newFish);
            }
        }

        private int mod(int x, int m)
        {
            return (x % m + m) % m;
        }
    }
}
