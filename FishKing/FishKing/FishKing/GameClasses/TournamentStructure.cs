using FishKing.UtilityClasses;
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
        public string TournamentName { get; set; }
        public int GoalPoints { get; set; }
        public int NumberOfParticipants { get; set; }
        public int RewardAmount { get; set; }
        public string MapName { get; set; }
        public TournamentRules TournamentRules { get; set; }
        public SerializableTuple<TrophyType, int> TrophyRequirements { get; set; }

        public TournamentStructure() { }

        public TournamentStructure(string name, int goal, int numParticipants, int reward, string map, TournamentRules rules, SerializableTuple<TrophyType, int> requirements)
        {
            TournamentName = name;
            NumberOfParticipants = numParticipants;
            GoalPoints = goal;
            RewardAmount = reward;
            MapName = map;
            TournamentRules = rules;
            TrophyRequirements = requirements;
        }

        public static bool operator ==(TournamentStructure a, TournamentStructure b)
        {

            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.TournamentName == b.TournamentName;
        }

        public static bool operator !=(TournamentStructure a, TournamentStructure b)
        {
            return !(a == b);
        }
    }
}
