using FishKing.GameClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.Extensions
{
    static class TournamentExtensions
    {
        public static bool MeetsRequirementsOf(this List<TournamentResults> results, TournamentStructure tournament)
        {
            if (results.Count == 0) return false;

            var req = tournament.TrophyRequirements;
            var trophyType = req.Item1;
            var numReq = req.Item2;

            if (trophyType == Enums.TrophyTypes.TrophyType.None) return true;

            var meetsRequirements = results.Where(tr => tr.PlaceTaken <= (int)trophyType).Count() > numReq;

            return meetsRequirements;
        }

        public static int DetermineRewardAmount(this TournamentStructure tournament, int playerPlace)
        {
            var rewardCutoff = tournament.NumberOfParticipants - 3;

            if (playerPlace < rewardCutoff)
            {
                return 0;
            }
            else if (playerPlace == 1)
            {
                return tournament.RewardAmount;
            }
            else
            {
                return (1 / playerPlace) * tournament.RewardAmount;
            }
        }
    }
}
