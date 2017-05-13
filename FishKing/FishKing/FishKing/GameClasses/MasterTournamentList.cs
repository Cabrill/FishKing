using FishKing.DataTypes;
using FishKing.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FishKing.Enums.TrophyTypes;

namespace FishKing.GameClasses
{
    public static class MasterTournamentList
    {
        private static List<TournamentStructure> allTournaments;
        public static List<TournamentStructure> AllTournaments
        {
            get
            {
                if (allTournaments == null) loadTournaments();
                return allTournaments;
            }
        }

        private static void loadTournaments()
        {
            var tournaments = GlobalContent.Tournaments;
            allTournaments = new List<TournamentStructure>();
            foreach (var tourney in tournaments.Values)
            {
                var tournamentStructure = new TournamentStructure(
                    name: tourney.Name,
                    goal: tourney.Goal,
                    numParticipants: tourney.Participants,
                    reward: tourney.Reward,
                    map: tourney.Map,
                    rules: new TournamentRules(tourney.RequiredFish.ToList(), tourney.WeightReq, tourney.LengthReq),
                    requirements: SpecifyRequirements(tourney.TrophyReq, tourney.TrophyNumRequired)
                    );
                allTournaments.Add(tournamentStructure);
            }
        }

        private static SerializableTuple<TrophyType, int> SpecifyRequirements(TrophyType tt, int i)
        {
            return SerializableTuple<TrophyType, int>.Create(tt, i);
        }
    }
}
