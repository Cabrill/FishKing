using FishKing.GameClasses;
using FishKing.GumRuntimes;
using FishKing.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.Managers
{
    public static class TournamentManager
    {
        public static bool TournamentHasStarted
        {
            get; private set;
        }

        public static TournamentStructure CurrentTournament
        {
            get; private set;
        }

        public static TournamentScores CurrentScores
        {
            get; private set;
        }

        public static TournamentResults CurrentTournamentResults
        {
            get; private set;
        }

        public static bool HasTournamentFinished
        {
            get { return CurrentScores.HasPlayerFinished; }
        }

        public static void SetCurrentTournament(TournamentStructure newTournament)
        {
            CurrentTournament = newTournament;
            CurrentScores = new TournamentScores(newTournament.NumberOfParticipants);
            TournamentHasStarted = false;
        }

        public static void StartTournament()
        {
            TournamentHasStarted = true;
        }
    }
}
