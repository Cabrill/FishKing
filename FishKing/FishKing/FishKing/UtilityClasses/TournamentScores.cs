using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.UtilityClasses
{
    class TournamentScores
    {
        int goalScore;
        public int GoalScore
        {
            get { return goalScore; }
            set { goalScore = value; }
        }
        int[] scores;
        public int[] Scores
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
        public void SimulateTournament()
        {
            if (FlatRedBall.TimeManager.CurrentTime > 10)
            {
                var random = RandomNumbers.Random;

                if (random.NextDouble() > 0.999)
                {
                    var randomScore = random.Next(1, scores.Length);
                    var randomPoints = random.Next(3, 51);
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
