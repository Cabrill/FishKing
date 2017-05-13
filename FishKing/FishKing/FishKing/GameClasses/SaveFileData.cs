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

        private DateTime recentStartTime;

        private TimeSpan m_TimePlayed;

        [XmlIgnore]
        public TimeSpan TimePlayed
        {
            get { return m_TimePlayed; }
            set { m_TimePlayed = value; }
        }

        // Pretend property for serialization
        [XmlElement("TimePlayed")]
        public long TimeSinceLastEventTicks
        {
            get { return m_TimePlayed.Ticks; }
            set { m_TimePlayed = new TimeSpan(value); }
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
                return ParticipatedTournaments.Where(tr => tr.PlaceTaken == 1).Count();
            }
        }

        [XmlIgnore]
        public int NumberOfSilverTrophies
        {
            get
            {
                return ParticipatedTournaments.Where(tr => tr.PlaceTaken == 2).Count();
            }
        }

        [XmlIgnore]
        public int NumberOfBronzeTrophies
        {
            get
            {
                return ParticipatedTournaments.Where(tr => tr.PlaceTaken == 3).Count();
            }
        }

        [XmlIgnore]
        public int NumberOfAllTrophies
        {
            get
            {
                return ParticipatedTournaments.Where(tr => tr.PlaceTaken <= 3).Count();
            }
        }

        public SerializableDictionary<Fish_Types, FishRecord> FishCaught;

        [XmlIgnore]
        public int NumberOfFishCaught { get { return FishCaught.Keys.Count; } }

        [XmlIgnore]
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

        [XmlIgnore]
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

        public SaveFileData() {
            Options = new GameOptions();
            DateCreated = DateTime.Now;
            LastPlayed = DateTime.Now;
            TimePlayed = new TimeSpan(0);
            FishCaught = new SerializableDictionary<Fish_Types, FishRecord>();
            ParticipatedTournaments = new List<TournamentResults>();
        }

        public SaveFileData(int saveSlot, int playerFishNumber)
        {
            Options = new GameOptions();
            SaveSlotNumber = saveSlot;
            PlayerFishNumber = playerFishNumber;
            DateCreated = DateTime.Now;
            LastPlayed = DateTime.Now;
            TimePlayed = new TimeSpan(0);
            FishCaught = new SerializableDictionary<Fish_Types, FishRecord>();
            ParticipatedTournaments = new List<TournamentResults>();
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
            var existingResult = ParticipatedTournaments.Where(tr => tr.Tournament.TournamentName == result.Tournament.TournamentName).FirstOrDefault();
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

        public void AddCaughtFish(Fish fish)
        {
            FishRecord fishRecord;

            if (FishCaught.ContainsKey(fish.FishType))
            {
                fishRecord = FishCaught[fish.FishType];
                FishCaught.Remove(fish.FishType);
                fishRecord.AddFish(fish);
            }
            else
            {
                fishRecord = new FishRecord(fish.Name, fish.Grams, fish.LengthMM);
            }
            
            FishCaught.Add(fish.FishType, fishRecord);
        }
    }
}
