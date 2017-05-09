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
            get { return scores; }
        }

        public int PlayerScore
        {
            get { return scores[0]; }
        }

        public int PlayerPlace
        {
            get
            {
                var sortedScore = (int[])scores.Clone();
                Array.Sort<int>(sortedScore,
                    new Comparison<int>(
                            (i1, i2) => i2.CompareTo(i1)
                    ));

                return Array.IndexOf(sortedScore, PlayerScore) + 1;
            }
        }

        private bool scoreHasChanged = false;
        public bool HasScoreChanged
        {
            get { return scoreHasChanged; }
        }

        public bool HasPlayerFinished
        {
            get { return (GoalScore > 0 && PlayerScore >= GoalScore); }
        }

        public TournamentScores(int playerSize, int goalScore)
        {
            scores = new int[playerSize];
            for (int i = 0; i < playerSize; i++)
            {
                scores[i] = 0;
            }
            GoalScore = goalScore;
        }

        public void AddToPlayerScore(int scoreIncrement)
        {
            scores[0] += scoreIncrement;
            scoreHasChanged = true;
        }

        public void AddToNonPlayerScore(int npcNum, int scoreIncrement)
        {
            if (npcNum > 0 && npcNum < scores.Count()-1)
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
