using FishKing.GameClasses;
using FishKing.UtilityClasses;
using FlatRedBall.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.GumRuntimes
{
    partial class GameSelectPreviewRuntime
    {
        public SaveFileData SaveData { get; private set; }

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
            this.CurrentHighlightState = Highlight.Highlighted;
        }

        public void UnhighlightButton()
        {
            this.CurrentHighlightState = Highlight.NotHighlighted;
        }

        public void AssociatedWithSaveGame(SaveFileData saveData)
        {
            SaveData = saveData;

            this.LastPlayedValue.Text = InfoToString.Date(saveData.LastPlayed);
            this.TimePlayedValue.Text = InfoToString.Time(saveData.TimePlayed);

            this.GoldTrophyCount.TrophyCountText = saveData.NumberOfGoldTrophies.ToString();
            this.SilverTrophyCount.TrophyCountText = saveData.NumberOfSilverTrophies.ToString();
            this.BronzeTrophyCount.TrophyCountText = saveData.NumberOfBronzeTrophies.ToString();

            switch (saveData.PlayerFishNumber)
            {
                case 0: CurrentPlayerFishState = PlayerFish.Fish1; break;
                case 1: CurrentPlayerFishState = PlayerFish.Fish2; break;
                case 2: CurrentPlayerFishState = PlayerFish.Fish3; break;
                case 3: CurrentPlayerFishState = PlayerFish.Fish4; break;
                case 4: CurrentPlayerFishState = PlayerFish.Fish5; break;
                case 5: CurrentPlayerFishState = PlayerFish.Fish6; break;
                case 6: CurrentPlayerFishState = PlayerFish.Fish7; break;
                case 7: CurrentPlayerFishState = PlayerFish.Fish8; break;
            }
            this.CurrentFilledState = Filled.Full;
        }
    }
}
