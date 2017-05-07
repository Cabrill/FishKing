using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.UtilityClasses
{
    public class TournamentScores
    {
        int goalScore;
        public int GoalScore
        {
            get { return goalScore; }
            set { goalScore = value; }
        }
        int[] scores;
        public int[] AsArray
        {
            get { return scores;  }
        }

        public int PlayerScore
        {
            get { return scores[0]; }
        }

        private bool scoreHasChanged = false;
        public bool HasScoreChanged
        {
            get { return scoreHasChanged; }
        }

        public bool HasPlayerFinished
        {
            get { return PlayerScore >= GoalScore; }
        }

        public TournamentScores(int playerSize = 8)
        {
            scores = new int[playerSize];
            for (int i = 0; i < playerSize; i++)
            {
                scores[i] = 0;
            }
        }

        public void AddToPlayerScore(int scoreIncrement)
        {
            scores[0] += scoreIncrement;
            scoreHasChanged = true;
        }

        public void AddToNonPlayerScore(int npcNum, int scoreIncrement)
        {
            if (npcNum > 0)
            {
                scores[npcNum] += scoreIncrement;
                scoreHasChanged = true;
            }
        }

        public void MarkScoreReviewed()
        {
            scoreHasChanged = false;
        }

#if DEBUG
        private double startTime = 0;
        public void SimulateTournament()
        {
            if (startTime == 0)
            {
                startTime = FlatRedBall.TimeManager.CurrentTime;
            }
            if (FlatRedBall.TimeManager.CurrentTime - startTime > 10)
            {
                var random = RandomNumbers.Random;

                if (random.NextDouble() > 0.999)
                {
                    var randomScore = random.Next(1, scores.Length);
                    var randomPoints = random.Next(3, 26);
                    if (scores[randomScore] < goalScore)
                    {
                        scores[randomScore] += randomPoints;
                        scoreHasChanged = true;
                    }
                }
            }
        }
#endif
    }
}
