using FlatRedBall.Glue.StateInterpolation;
using StateInterpolationPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.GumRuntimes
{
    partial class TournamentStatusRuntime
    {
        public float AbsoluteHeight
        {
            get
            {
                return (this as RenderingLibrary.IPositionedSizedObject).Height - 14f;
            }
        }
        private int playerScore = 0;
        public int PlayerScore
        {
            get { return playerScore; }
            private set { playerScore = value;  UpdatePlayerScore();  }
        }
        private int playerFishNumber = 0;
        public int PlayerFishNumber
        {
            get { return playerFishNumber;  }
            set { playerFishNumber = value; SetPlayerFish();  }
        }
        private int goalScore = 0;
        public int GoalScore
        {
            get { return goalScore; }
            set { goalScore = value; UpdateGoalScore();  }
        }
        private int playerPlace = 8;
        public int PlayerPlace
        {
            get { return playerPlace; }
            private set { playerPlace = value; }
        }

        private int lastTopFishPlace = 8;
        private int lastBottomFishPlace = 8;
        private int lastPlayerPlace = 8;
        private int lastTopFishNum = -1;
        private int lastBottomFishNum = -1;
        private int lastTopFishScore = 0;
        private int lastBottomFishScore = 0;
        private int lastPlayerScore = 0;

        partial void CustomInitialize()
        {
            Reset();

            Wave1AnimateAnimation.Play();
            Wave2AnimateAnimation.Play();
            Wave3AnimateAnimation.Play();
            Wave4AnimateAnimation.Play();
            TopFishInstance.FishSwimAnimation.Play();
            PlayerFishInstance.FishSwimAnimation.Play();
            BottomFishInstance.FishSwimAnimation.Play();
        }

        public void CustomActivity()
        {

        }

        public void UpdateFishPlaceMarkers(int[] scoreArray)
        {
            if (scoreArray.Sum() > 0)
            {
                int playerScore = scoreArray[0];

                int maxScore = scoreArray.Max();
                int minScore = scoreArray.Min();

                bool playerInFirst = playerScore == maxScore;
                bool playerInLast = playerScore == minScore;

                int scoreDiff, topFishScore, bottomFishScore, topFishNum, bottomFishNum;
                topFishNum = bottomFishNum = 0;

                if (playerInFirst)
                {
                    topFishScore = bottomFishScore = -1;
                }
                else if (playerInLast)
                {
                    topFishScore = bottomFishScore = 1000;
                }
                else
                {
                    topFishScore = -1;
                    bottomFishScore = 1000;
                }

                for (int i = 1; i < scoreArray.Length; i++)
                {
                    if (playerInFirst)
                    {
                        if (scoreArray[i] > topFishScore)
                        {
                            bottomFishScore = topFishScore;
                            bottomFishNum = topFishNum;
                            topFishScore = scoreArray[i];
                            topFishNum = i;
                        }
                        else if (scoreArray[i] > bottomFishScore)
                        {
                            bottomFishScore = scoreArray[i];
                            bottomFishNum = i;
                        }
                    }
                    else if (playerInLast)
                    {
                        scoreDiff = scoreArray[i] - playerScore;
                        if (scoreDiff < topFishScore - playerScore)
                        {
                            bottomFishScore = topFishScore;
                            bottomFishNum = topFishNum;
                            topFishScore = scoreArray[i];
                            topFishNum = i;
                        }
                        else if (scoreDiff < bottomFishScore - playerScore)
                        {
                            bottomFishScore = scoreArray[i];
                            bottomFishNum = i;
                        }
                    }
                    else
                    {
                        if (scoreArray[i] > playerScore)
                        {
                            scoreDiff = scoreArray[i] - playerScore;
                            if (scoreDiff < topFishScore - playerScore)
                            {
                                topFishScore = scoreArray[i];
                                topFishNum = i;
                            }
                        }
                        else if (scoreArray[i] < playerScore)
                        {
                            scoreDiff = playerScore - scoreArray[i];
                            if (scoreDiff < playerScore - bottomFishScore)
                            {
                                bottomFishScore = scoreArray[i];
                                bottomFishNum = i;
                            }
                        }
                        else
                        {
                            if (topFishScore != playerScore)
                            {
                                topFishScore = scoreArray[i];
                                topFishNum = i;
                            }
                            else if (bottomFishScore != playerScore)
                            {
                                bottomFishScore = scoreArray[i];
                                bottomFishNum = i;
                            }
                        }
                    }
                }
                topFishNum = (topFishNum == playerFishNumber ? 0 : topFishNum);
                bottomFishNum = (bottomFishNum == playerFishNumber ? 0 : bottomFishNum);

                if (topFishNum != lastTopFishNum)
                {
                    SetFishTypeFromNumber(TopFishInstance, topFishNum);
                    TopFishInstance.TournamentFishProgress = (float)Decimal.Divide(topFishScore, GoalScore) * 100;
                }
                else if (lastTopFishScore != topFishScore)
                {
                    JumpFishTo(TopFishInstance, (float)Decimal.Divide(topFishScore, GoalScore)*100);
                }

                if (bottomFishNum != lastBottomFishNum)
                {
                    SetFishTypeFromNumber(BottomFishInstance, bottomFishNum);
                    BottomFishInstance.TournamentFishProgress = (float)Decimal.Divide(bottomFishScore, GoalScore) * 100;
                }
                else if (lastBottomFishScore != bottomFishScore)
                {
                    JumpFishTo(BottomFishInstance, (float)Decimal.Divide(bottomFishScore, GoalScore)*100);
                }

                if (playerScore != lastPlayerScore)
                {
                    JumpFishTo(PlayerFishInstance, (float)Decimal.Divide(playerScore, GoalScore)*100);
                }

                Array.Sort<int>(scoreArray,
                    new Comparison<int>(
                            (i1, i2) => i2.CompareTo(i1)
                    ));

                if (playerInFirst)
                {
                    PlayerPlace = 1;
                }
                else if (playerInLast)
                {
                    PlayerPlace = 8;
                }
                else
                {
                    PlayerPlace = Array.IndexOf(scoreArray, playerScore) + 1;
                }
                var topFishPlace = Array.IndexOf(scoreArray, topFishScore) + 1;
                var bottomFishPlace = Array.IndexOf(scoreArray, bottomFishScore) + 1;

                if (topFishPlace != lastTopFishPlace)
                {
                    SetFishPlaceFromNumber(TopFishInstance, topFishPlace);
                }
                if (PlayerPlace != lastPlayerPlace)
                {
                    SetFishPlaceFromNumber(PlayerFishInstance, PlayerPlace);
                }
                if (bottomFishPlace != lastBottomFishPlace)
                {
                    SetFishPlaceFromNumber(BottomFishInstance, bottomFishPlace);
                }

                lastTopFishPlace = topFishPlace;
                lastPlayerPlace = PlayerPlace;
                lastBottomFishPlace = bottomFishPlace;

                PlayerScore = playerScore;
            }
        }

        private void JumpFishTo(TournamentFishRuntime fish, float newPosition)
        {
            var timeToTween = fish.FishJumpAnimation.Length;
            var startValue = fish.TournamentFishProgress;
            var tweenHolder = new TweenerHolder();
            tweenHolder.Caller = fish;
            var fishJumpTweener = new Tweener(startValue, newPosition, timeToTween, InterpolationType.Quartic, Easing.Out);
            fishJumpTweener.PositionChanged += (position) =>
            {
                fish.TournamentFishProgress = position;
            };
            //fishJumpTweener.Ended += afterAction;
            fishJumpTweener.Start();
            TweenerManager.Self.Add(fishJumpTweener);
            fish.FishJumpAnimation.Play();
        }

        private void Reset()
        {
            ScoreText.Text = "0";

            TopFishInstance.TournamentFishProgress = 0;
            PlayerFishInstance.TournamentFishProgress = 0;
            BottomFishInstance.TournamentFishProgress = 0;

            TopFishInstance.CurrentTournamentPlaceState = TournamentFishRuntime.TournamentPlace.None;
            PlayerFishInstance.CurrentTournamentPlaceState = TournamentFishRuntime.TournamentPlace.None;
            BottomFishInstance.CurrentTournamentPlaceState = TournamentFishRuntime.TournamentPlace.None;
        }

        private void UpdateGoalScore()
        {
            GoalScoreText.Text = goalScore.ToString();
            if (goalScore > 199)
            {
                CurrentGoalValueState = GoalValue.More;
            }
            else
            {
                CurrentGoalValueState = GoalValue.OneHundreds;
            }
        }

        private void UpdatePlayerScore()
        {
            ScoreText.Text = PlayerScore.ToString();
        }

        public void SetPlayerFish()
        {
            SetFishTypeFromNumber(PlayerFishInstance, PlayerFishNumber);
        }

        private void SetFishTypeFromNumber(TournamentFishRuntime fish, int fishNumber)
        {
            TournamentFishRuntime.FishType fishType;
            switch (fishNumber)
            {
                case 0: fishType = TournamentFishRuntime.FishType.FishType1; break;
                case 1: fishType = TournamentFishRuntime.FishType.FishType2; break;
                case 2: fishType = TournamentFishRuntime.FishType.FishType3; break;
                case 3: fishType = TournamentFishRuntime.FishType.FishType4; break;
                case 4: fishType = TournamentFishRuntime.FishType.FishType5; break;
                case 5: fishType = TournamentFishRuntime.FishType.FishType6; break;
                case 6: fishType = TournamentFishRuntime.FishType.FishType7; break;
                case 7: fishType = TournamentFishRuntime.FishType.FishType8; break;
                default: fishType = TournamentFishRuntime.FishType.FishType1; break;
            }
            fish.CurrentFishTypeState = fishType;
        }

        private void SetFishPlaceFromNumber(TournamentFishRuntime fish, int place)
        {
            TournamentFishRuntime.TournamentPlace fishPlace;
            switch (place)
            {
                case 1: fishPlace = TournamentFishRuntime.TournamentPlace.FirstPlace; break;
                case 2: fishPlace = TournamentFishRuntime.TournamentPlace.SecondPlace; break;
                case 3: fishPlace = TournamentFishRuntime.TournamentPlace.ThirdPlace; ; break;
                case 4: fishPlace = TournamentFishRuntime.TournamentPlace.FourthPlace; break;
                case 5: fishPlace = TournamentFishRuntime.TournamentPlace.FifthPlace; break;
                case 6: fishPlace = TournamentFishRuntime.TournamentPlace.SixthPlace; break;
                case 7: fishPlace = TournamentFishRuntime.TournamentPlace.SeventhPlace; break;
                case 8: fishPlace = TournamentFishRuntime.TournamentPlace.EighthPlace; break;
                default: fishPlace = TournamentFishRuntime.TournamentPlace.None; break;
            }
            fish.CurrentTournamentPlaceState = fishPlace;
        }

        private void CustomDestroy()
        {


        }

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
    }
}
