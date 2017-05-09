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

        private static TournamentResults results;
        public static TournamentResults CurrentTournamentResults
        {
            get { return results; }
        }

        public static bool HasTournamentFinished
        {
            get { return CurrentScores.HasPlayerFinished; }
        }

        public static void SetCurrentTournament(TournamentStructure newTournament)
        {
            CurrentTournament = newTournament;
            CurrentScores = new TournamentScores(newTournament.NumberOfParticipants, newTournament.GoalPoints);
            TournamentHasStarted = false;
            results = null;
        }

        public static void StartTournament()
        {
            TournamentHasStarted = true;
        }

        public static void EndTournament()
        {
            CurrentScores.MarkScoreReviewed();
            results = new TournamentResults(CurrentTournament, CurrentScores.PlayerPlace);
        }
    }
}
