using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.GameClasses
{
    public class TournamentStructure
    {
        public string TournamentName { get; private set; }
        public int NumberOfParticipants { get; private set; }
        public int RewardAmount { get; private set; }
        public string MapName { get; private set; }
        public TournamentRules TournamentRules { get; private set; }

        public TournamentStructure(string name, int numParticipants, int reward, string map, TournamentRules rules)
        {
            TournamentName = name;
            NumberOfParticipants = numParticipants;
            RewardAmount = reward;
            MapName = map;
            TournamentRules = rules;
        }
    }
}
