using FishKing.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.GameClasses
{
    public class TournamentResults
    {
        public string TournamentName
        {
            get; set;
        }
        public int PlaceTaken
        {
            get; set;
        }
        public int RewardEarned
        {
            get; set;
        }

        public TournamentResults() { }

        public TournamentResults(TournamentStructure tournament, int place)
        {
            TournamentName = tournament.TournamentName;
            PlaceTaken = place;
            RewardEarned = tournament.DetermineRewardAmount(place);
        }
    }
}
