using FishKing.GameClasses;
using FishKing.Managers;
using FishKing.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.GumRuntimes
{
    partial class ResultsDisplayRuntime
    {
        partial void CustomInitialize()
        {

        }

        public void DisplayResults(TournamentResults results)
        {
            TrophyDisplayInstance.SetTrophyByPlaceNumber(results.PlaceTaken);

            if (results.RewardEarned > 0)
            {
                RewardValue.Text = "$" + results.RewardEarned;
                CurrentEarnedRewardState = EarnedReward.GotReward;
                TrophyDisplayInstance.DropConfettiAnimation.Play();
                CongratulationsColorCycleAnimation.Play();
            }
            else
            {
                CurrentEarnedRewardState = EarnedReward.NoReward;
            }
        }

        public void ClickOK()
        {
            OKButton.CallClick();
        }

    }
}
