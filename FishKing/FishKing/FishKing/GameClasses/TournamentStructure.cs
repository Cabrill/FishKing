using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FishKing.Enums.TrophyTypes;

namespace FishKing.GameClasses
{
    public class TournamentStructure
    {
        public string TournamentName { get; private set; }
        public int GoalPoints { get; private set; }
        public int NumberOfParticipants { get; private set; }
        public int RewardAmount { get; private set; }
        public string MapName { get; private set; }
        public TournamentRules TournamentRules { get; private set; }
        public Tuple<TrophyType, int> TrophyRequirements { get; private set; } 

        public TournamentStructure(string name, int goal, int numParticipants, int reward, string map, TournamentRules rules, Tuple<TrophyType, int> requirements)
        {
            TournamentName = name;
            NumberOfParticipants = numParticipants;
            GoalPoints = goal;
            RewardAmount = reward;
            MapName = map;
            TournamentRules = rules;
            TrophyRequirements = requirements;
        }
    }
}
