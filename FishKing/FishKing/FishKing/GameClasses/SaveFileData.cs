using FishKing.DataTypes;
using FishKing.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.GameClasses
{
    public class SaveFileData
    {
        public string PlayerName { get; private set; }

        public string UniqueID { get; private set; }

        public DateTime DateCreated { get; private set; }
        public DateTime LastPlayed { get; private set; }
        public TimeSpan TimePlayed { get; private set; }
        private DateTime recentStartTime;

        public int MoneySpent { get; private set; }
        public int MoneyEarned
        {
            get
            {
                return ParticipatedTournaments.Sum(tr => tr.RewardEarned);
            }
        }
        public int MoneyAvailable { get { return MoneyEarned - MoneySpent; } }

        public List<TournamentResults> ParticipatedTournaments { get; private set; }
        public int NumberOfGoldTrophies
        {
            get
            {
                return ParticipatedTournaments.Where(tr => tr.PlaceTaken == 1).Count();
            }
        }
        public int NumberOfSilverTrophies
        {
            get
            {
                return ParticipatedTournaments.Where(tr => tr.PlaceTaken == 2).Count();
            }
        }
        public int NumberOfBronzeTrophies
        {
            get
            {
                return ParticipatedTournaments.Where(tr => tr.PlaceTaken == 3).Count();
            }
        }

        public Dictionary<Fish_Types, FishRecord> FishCaught;
        public int NumberOfFishCaught { get { return FishCaught.Keys.Count; } }
        public int HeaviestFish
        {
            get
            {
                if (FishCaught.Count == 0)
                {
                    return 0;
                }
                else
                {
                   return FishCaught.Values.Max(f => f.HeaviestCaught);
                }
            }
        }
        public int LongestFish
        {
            get
            {
                if (FishCaught.Count == 0)
                {
                    return 0;
                }
                else
                {
                    return FishCaught.Values.Max(f => f.LongestCaught);
                }
            }
        }

        public SaveFileData() { }

        public SaveFileData(string playerName, string saveFileId)
        {
            PlayerName = playerName;
            DateCreated = DateTime.Now;
            LastPlayed = DateTime.Now;
            TimePlayed = new TimeSpan(0);
            FishCaught = new Dictionary<Fish_Types, FishRecord>();
            ParticipatedTournaments = new List<TournamentResults>();
            UniqueID = saveFileId;
        }

        public void StartPlaySession()
        {
            recentStartTime = DateTime.Now;
        }

        public void StopPlaySession()
        {
            var newTime = DateTime.Now - recentStartTime;
            TimePlayed += newTime;
            LastPlayed = DateTime.Now;
        }

        public void AddTournamentResult(TournamentResults result)
        {
            var existingResult = ParticipatedTournaments.Where(tr => tr.Tournament.TournamentName == result.Tournament.TournamentName).First();
            if (existingResult != null)
            {
                if (existingResult.PlaceTaken > result.PlaceTaken)
                {
                    ParticipatedTournaments.Remove(existingResult);
                    ParticipatedTournaments.Add(result);
                }
            }
            else
            {
                ParticipatedTournaments.Add(result);
            }
        }
    }
}
