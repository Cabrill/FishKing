using FishKing.DataTypes;
using FishKing.Entities;
using FishKing.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FishKing.GameClasses
{
    [Serializable]
    public class SaveFileData
    {
        public int PlayerFishNumber { get; set; }

        public int SaveSlotNumber { get; set; }

        public GameOptions Options { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime LastPlayed { get; set; }

        private DateTime _recentStartTime;

        private TimeSpan _mTimePlayed;

        [XmlIgnore]
        public TimeSpan TimePlayed
        {
            get { return _mTimePlayed; }
            set { _mTimePlayed = value; }
        }

        // Pretend property for serialization
        [XmlElement("TimePlayed")]
        public long TimeSinceLastEventTicks
        {
            get { return _mTimePlayed.Ticks; }
            set { _mTimePlayed = new TimeSpan(value); }
        }

        public int MoneySpent { get; set; }

        [XmlIgnore]
        public int MoneyEarned
        {
            get
            {
                return ParticipatedTournaments.Sum(tr => tr.RewardEarned);
            }
        }

        [XmlIgnore]
        public int MoneyAvailable { get { return MoneyEarned - MoneySpent; } }

        [XmlArray("TournamentResults")]
        [XmlArrayItem("TournamentResult")]
        public List<TournamentResults> ParticipatedTournaments { get; set; }

        [XmlIgnore]
        public int NumberOfGoldTrophies
        {
            get
            {
                return ParticipatedTournaments.Count(tr => tr.PlaceTaken == 1);
            }
        }

        [XmlIgnore]
        public int NumberOfSilverTrophies
        {
            get
            {
                return ParticipatedTournaments.Count(tr => tr.PlaceTaken == 2);
            }
        }

        [XmlIgnore]
        public int NumberOfBronzeTrophies
        {
            get
            {
                return ParticipatedTournaments.Count(tr => tr.PlaceTaken == 3);
            }
        }

        [XmlIgnore]
        public int NumberOfAllTrophies
        {
            get
            {
                return ParticipatedTournaments.Count(tr => tr.PlaceTaken <= 3);
            }
        }

        public SerializableDictionary<string, FishRecord> FishCaught;

        [XmlIgnore]
        public int NumberOfFishCaught => FishCaught.Keys.Count;

        [XmlIgnore]
        public int HeaviestFish
        {
            get { return FishCaught.Count == 0 ? 0 : FishCaught.Values.Max(f => f.HeaviestCaught); }
        }

        [XmlIgnore]
        public int LongestFish
        {
            get { return FishCaught.Count == 0 ? 0 : FishCaught.Values.Max(f => f.LongestCaught); }
        }

        public SaveFileData() {
            Options = new GameOptions();
            DateCreated = DateTime.Now;
            LastPlayed = DateTime.Now;
            TimePlayed = new TimeSpan(0);
            FishCaught = new SerializableDictionary<string, FishRecord>();
            ParticipatedTournaments = new List<TournamentResults>();
            _recentStartTime = DateTime.MinValue;
        }

        public SaveFileData(int saveSlot, int playerFishNumber)
        {
            Options = new GameOptions();
            SaveSlotNumber = saveSlot;
            PlayerFishNumber = playerFishNumber;
            DateCreated = DateTime.Now;
            LastPlayed = DateTime.Now;
            TimePlayed = new TimeSpan(0);
            FishCaught = new SerializableDictionary<string, FishRecord>();
            ParticipatedTournaments = new List<TournamentResults>();
            _recentStartTime = DateTime.MinValue;
        }

        public void StartPlaySession()
        {
            if (_recentStartTime == DateTime.MinValue)
            {
                _recentStartTime = DateTime.Now;
            }
        }

        public void StopPlaySession()
        {
            var newTime = DateTime.Now - _recentStartTime;
            TimePlayed += newTime;
            LastPlayed = DateTime.Now;
            _recentStartTime = DateTime.MinValue;
        }

        public void AddTournamentResult(TournamentResults result)
        {
            var existingResult = ParticipatedTournaments.FirstOrDefault(tr => tr.TournamentName == result.TournamentName);
            if (existingResult != null)
            {
                if (existingResult.PlaceTaken <= result.PlaceTaken) return;

                ParticipatedTournaments.Remove(existingResult);
                ParticipatedTournaments.Add(result);
            }
            else
            {
                ParticipatedTournaments.Add(result);
            }
        }

        public bool AddCaughtFish(Fish fish)
        {
            bool isNewCatch;
            FishRecord fishRecord;

            if (FishCaught.ContainsKey(fish.Name))
            {
                isNewCatch = false;
                fishRecord = FishCaught[fish.Name];
                FishCaught.Remove(fish.Name);
                fishRecord.AddFish(fish);
            }
            else
            {
                isNewCatch = true;
                fishRecord = new FishRecord(fish.Name, fish.Grams, fish.LengthMM);
            }
            
            FishCaught.Add(fish.Name, fishRecord);

            return isNewCatch;
        }
    }
}
