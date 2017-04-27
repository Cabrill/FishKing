using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.GumRuntimes
{
    partial class TitleScreenGumRuntime
    {
        partial void CustomInitialize()
        {
            CurrentFishKingTextState = FishKingText.Start;
            CurrentButtonsVisibilityState = ButtonsVisibility.Start;
            CurrentKingFishState = KingFish.Start;
            CurrentLetsGoTextState = LetsGoText.Start;
            CurrentMenuScreenButtonsState = MenuScreenButtons.InitialButtons;
        }

        public void CustomActivity()
        {

        }

        private void CustomDestroy()
        {


        }
    }
}