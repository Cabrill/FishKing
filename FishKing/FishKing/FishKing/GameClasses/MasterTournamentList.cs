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
            allTournaments = new List<TournamentStructure>();

            allTournaments.Add(new TournamentStructure(name: "Desert Island Challenge",
                goal: 500,
                numParticipants: 8,
                reward: 200,
                map: "DesertIsland",
                rules: new TournamentRules(),
                requirements: SpecifyRequirements(TrophyType.Gold, 5)
            ));

            allTournaments.Add(new TournamentStructure(name: "Ice Floe",
                goal: 200,
                numParticipants: 4,
                reward: 100,
                map: "IceFloe",
                rules: new TournamentRules(new List<Fish_Types>() { GlobalContent.Fish_Types[Fish_Types.Coho_Salmon] }),
                requirements: SpecifyRequirements(TrophyType.Bronze, 1)
            ));

            allTournaments.Add(new TournamentStructure(name: "Easy Peasey2",
                goal: 100,
                numParticipants: 4,
                reward: 20,
                map: "SmallForest",
                rules: new TournamentRules(),
                requirements: SpecifyRequirements(TrophyType.None, 0)
            ));

            allTournaments.Add(new TournamentStructure(name: "Paradise Isle",
                goal: 500,
                numParticipants: 8,
                reward: 200,
                map: "DesertIsland",
                rules: new TournamentRules(null, 500),
                requirements: SpecifyRequirements(TrophyType.Silver, 3)
            ));

            allTournaments.Add(new TournamentStructure(name: "Easy Peasey4",
                goal: 100,
                numParticipants: 4,
                reward: 20,
                map: "DesertIsland",
                rules: new TournamentRules(),
                requirements: SpecifyRequirements(TrophyType.Gold, 2)
            ));

            allTournaments.Add(new TournamentStructure(name: "Haunted Forest",
                goal: 666,
                numParticipants: 6,
                reward: 66,
                map: "DesertIsland",
                rules: new TournamentRules(null, 0, 100),
                requirements: SpecifyRequirements(TrophyType.Gold, 25)
            ));

            allTournaments.Add(new TournamentStructure(name: "Easy Peasey5",
                goal: 100,
                numParticipants: 4,
                reward: 20,
                map: "DesertIsland",
                rules: new TournamentRules(),
                requirements: SpecifyRequirements(TrophyType.Bronze, 10)
            ));

            allTournaments.Add(new TournamentStructure(name: "Flooded Library",
                goal: 50,
                numParticipants: 2,
                reward: 300,
                map: "DesertIsland",
                rules: new TournamentRules(),
                requirements: SpecifyRequirements(TrophyType.Gold, 1)
            ));

            allTournaments.Add(new TournamentStructure(name: "Easy Peasey",
                goal: 100,
                numParticipants: 4,
                reward: 20,
                map: "DesertIsland",
                rules: new TournamentRules(),
                requirements: SpecifyRequirements(TrophyType.Silver, 5)
            ));

        }

        private static SerializableTuple<TrophyType, int> SpecifyRequirements(TrophyType tt, int i)
        {
            return SerializableTuple<TrophyType, int>.Create(tt, i);
        }
    }
}
