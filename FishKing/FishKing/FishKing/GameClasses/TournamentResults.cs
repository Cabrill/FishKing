using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.GameClasses
{
    public class TournamentResults
    {
        public TournamentStructure Tournament
        {
            get; private set;
        }
        public int PlaceTaken
        {
            get; private set;
        }
        public int RewardEarned
        {
            get; private set;
        }

        public TournamentResults(TournamentStructure tournament, int place, int rewardEarned)
        {
            Tournament = tournament;
            PlaceTaken = place;
            RewardEarned = rewardEarned;
        }
    }
}
