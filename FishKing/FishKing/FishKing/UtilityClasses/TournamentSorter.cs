using FishKing.Enums;
using FishKing.Extensions;
using FishKing.GameClasses;
using FishKing.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FishKing.Enums.TrophyTypes;

namespace FishKing.UtilityClasses
{
    public class TournamentSorter : IComparer<TournamentStructure>
    {
        public int Compare(TournamentStructure x, TournamentStructure y)
        {
            var xScore = scoreTournament(x);
            var yScore = scoreTournament(y);

            if (xScore < yScore)
            {
                return 1;
            }
            else if (xScore > yScore)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        private int scoreTournament(TournamentStructure tournament)
        {
            int modifier;
            var saveData = SaveGameManager.CurrentSaveData;

            if (saveData != null && saveData.MeetsRequirements(tournament))
            {
                modifier = 1;
            }
            else
            {
                modifier = -1;
            }

            if (tournament.TrophyRequirements.Item1 == TrophyType.None)
            {
                return int.MaxValue;
            }

            int multiplier = trophyTypeToMultiplier(tournament.TrophyRequirements.Item1, modifier);


            return tournament.TrophyRequirements.Item2 * modifier * multiplier;            
        }

        private int trophyTypeToMultiplier(TrophyType trophy, int modifier)
        {
            switch (trophy)
            {
                case TrophyType.Gold: return (modifier == 1 ? 1 : 100);
                case TrophyType.Silver: return 10;
                case TrophyType.Bronze: return (modifier == 1 ? 100 : 1);
                default: return 1000;
            }
        }
    }
}
