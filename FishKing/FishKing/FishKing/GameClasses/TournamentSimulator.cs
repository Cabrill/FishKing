using FishKing.Managers;
using FlatRedBall.TileCollisions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FishKing.Extensions;
using static FishKing.Enums.WaterTypes;
using FishKing.UtilityClasses;
using FishKing.Entities;

namespace FishKing.GameClasses
{
    public static class TournamentSimulator
    {
        private static Dictionary<int, double> NonPlayerFishTimes;
        private static List<WaterType> waterTypesAvailable;

        private static int baseSecondsBetweenCatches = 60;
        private static int secondsBetweenCatch
        {
            get
            {
                return (int)(baseSecondsBetweenCatches / (OptionsManager.Options.Difficulty + 1));
            }
        }

        public static void Initialize(int numberOfParticipants, List<WaterType> waterTypes)
        {
            waterTypesAvailable = waterTypes;
            NonPlayerFishTimes = new Dictionary<int, double>();
            for (int i = 1; i < numberOfParticipants; i++)
            {
                NonPlayerFishTimes.Add(i, 0);
            }
        }

        public static void Update()
        {
            var rnd = RandomNumbers.Random;
            var seconds = FlatRedBall.TimeManager.LastSecondDifference;

            for (int i = 0; i < NonPlayerFishTimes.Count; i++)
            {
                var player = NonPlayerFishTimes.ElementAt(i);

                if (!TournamentManager.CurrentScores.HasNonPlayerFinished(player.Key))
                {
                    if (player.Value > secondsBetweenCatch)
                    {
                        var waterType = waterTypesAvailable.RandomElement();

                        var catchChance = 0.0025;
                        var catchRoll = rnd.NextDouble();

                        if (catchRoll <= catchChance)
                        {
                            NonPlayerFishTimes[player.Key] = 0;
                            var fish = FishGenerator.CreateFish(waterType);
                            if (TournamentManager.CurrentTournament.DoesFishMeetRequirements(fish))
                            {
                                TournamentManager.CurrentScores.AddToNonPlayerScore(player.Key, fish.Points);
                            }
                        }
                        else
                        {
                            NonPlayerFishTimes[player.Key] = NonPlayerFishTimes[player.Key] + seconds;
                        }
                    }
                    else
                    {
                        NonPlayerFishTimes[player.Key] = NonPlayerFishTimes[player.Key] + seconds;
                    }
                }
            }
        }
    }
}
