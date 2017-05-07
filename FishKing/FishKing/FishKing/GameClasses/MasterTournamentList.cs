using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            allTournaments = new List<TournamentStructure>();

            allTournaments.Add(new TournamentStructure(name: "Desert Island Challenge",
                goal: 500,
                numParticipants: 8,
                reward: 200,
                map: "DesertIsland",
                rules: new TournamentRules(),
                requirements: Tuple.Create(Enums.TrophyTypes.TrophyType.Gold, 5)
            ));

            allTournaments.Add(new TournamentStructure(name: "Desert Island Challenge",
                goal: 500,
                numParticipants: 8,
                reward: 200,
                map: "DesertIsland",
                rules: new TournamentRules(),
                requirements: Tuple.Create(Enums.TrophyTypes.TrophyType.Gold, 5)
            ));

            allTournaments.Add(new TournamentStructure(name: "Desert Island Challenge",
                goal: 500,
                numParticipants: 8,
                reward: 200,
                map: "DesertIsland",
                rules: new TournamentRules(),
                requirements: Tuple.Create(Enums.TrophyTypes.TrophyType.Gold, 5)
            ));

            allTournaments.Add(new TournamentStructure(name: "Desert Island Challenge",
                goal: 500,
                numParticipants: 8,
                reward: 200,
                map: "DesertIsland",
                rules: new TournamentRules(),
                requirements: Tuple.Create(Enums.TrophyTypes.TrophyType.Gold, 5)
            ));

            allTournaments.Add(new TournamentStructure(name: "Desert Island Challenge",
                goal: 500,
                numParticipants: 8,
                reward: 200,
                map: "DesertIsland",
                rules: new TournamentRules(),
                requirements: Tuple.Create(Enums.TrophyTypes.TrophyType.Gold, 5)
            ));

            allTournaments.Add(new TournamentStructure(name: "Desert Island Challenge",
                goal: 500,
                numParticipants: 8,
                reward: 200,
                map: "DesertIsland",
                rules: new TournamentRules(),
                requirements: Tuple.Create(Enums.TrophyTypes.TrophyType.Gold, 5)
            ));

        }
    }
}
