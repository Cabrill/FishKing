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

        private int? lastTopFishPlace = null;
        private int? lastBottomFishPlace = null;
        private int? lastPlayerPlace = null;
        private int? lastTopFishNum = null;
        private int? lastBottomFishNum = null;
        private int? lastTopFishScore = null;
        private int? lastBottomFishScore = null;
        private int? lastPlayerScore = null;

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
            GlowingStarInstance.StarGlowAnimation.Play();
            TrophyDisplayInstance.TrophyPulseAnimation.Play();
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

                bool playerInFirst = playerScore > 0 && playerScore == maxScore;
                bool playerInLast = playerScore == minScore && scoreArray.Where(i => i > PlayerScore).Count() > 0;

                int scoreDiff, topFishNum, bottomFishNum;
                topFishNum = bottomFishNum = 0;

                int? topFishScore = null;
                int? bottomFishScore = null;
                
                for (int i = 1; i < scoreArray.Length; i++)
                {
                    if (playerInFirst)
                    {
                        if (!topFishScore.HasValue || scoreArray[i] > topFishScore)
                        {
                            bottomFishScore = topFishScore;
                            bottomFishNum = topFishNum;
                            topFishScore = scoreArray[i];
                            topFishNum = i;
                        }
                        else if (!bottomFishScore.HasValue || scoreArray[i] > bottomFishScore)
                        {
                            bottomFishScore = scoreArray[i];
                            bottomFishNum = i;
                        }
                    }
                    else if (playerInLast)
                    {
                        scoreDiff = scoreArray[i] - playerScore;
                        if (scoreDiff > 0 && (!topFishScore.HasValue || scoreDiff < topFishScore - playerScore))
                        {
                            bottomFishScore = topFishScore;
                            bottomFishNum = topFishNum;
                            topFishScore = scoreArray[i];
                            topFishNum = i;
                        }
                        else if (!bottomFishScore.HasValue || scoreDiff < bottomFishScore - playerScore)
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
                            if (!topFishScore.HasValue || scoreDiff < topFishScore - playerScore)
                            {
                                topFishScore = scoreArray[i];
                                topFishNum = i;
                            }
                        }
                        else if (scoreArray[i] < playerScore)
                        {
                            scoreDiff = playerScore - scoreArray[i];
                            if (!bottomFishScore.HasValue || scoreDiff < playerScore - bottomFishScore)
                            {
                                bottomFishScore = scoreArray[i];
                                bottomFishNum = i;
                            }
                        }
                        else
                        {
                            if (!topFishScore.HasValue || topFishScore != playerScore)
                            {
                                topFishScore = scoreArray[i];
                                topFishNum = i;
                            }
                            else if (!bottomFishScore.HasValue || bottomFishScore != playerScore)
                            {
                                bottomFishScore = scoreArray[i];
                                bottomFishNum = i;
                            }
                        }
                    }
                }
                topFishNum = (topFishNum == playerFishNumber ? 0 : topFishNum);
                bottomFishNum = (bottomFishNum == playerFishNumber ? 0 : bottomFishNum);

                if (lastTopFishNum.HasValue && topFishNum != lastTopFishNum)
                {
                    SetFishTypeFromNumber(TopFishInstance, topFishNum);
                    TopFishInstance.TournamentFishProgress = (float)Decimal.Divide(topFishScore.Value, GoalScore) * 100;
                }
                else if ((!lastTopFishScore.HasValue && topFishScore  > 0) || topFishScore != lastTopFishScore)
                {
                    JumpFishTo(TopFishInstance, (float)Decimal.Divide(topFishScore.Value, GoalScore)*100);
                }
                

                if (lastBottomFishNum.HasValue && bottomFishNum != lastBottomFishNum)
                {
                    SetFishTypeFromNumber(BottomFishInstance, bottomFishNum);
                    BottomFishInstance.TournamentFishProgress = (float)Decimal.Divide(bottomFishScore.Value, GoalScore) * 100;
                }
                else if ((!lastBottomFishScore.HasValue && bottomFishScore > 0) || bottomFishScore != lastBottomFishScore)
                {
                    JumpFishTo(BottomFishInstance, (float)Decimal.Divide(bottomFishScore.Value, GoalScore)*100);
                }
                
                if ((!lastPlayerScore.HasValue && playerScore > 0) || playerScore != lastPlayerScore)
                {
                    JumpFishTo(PlayerFishInstance, (float)Decimal.Divide(playerScore, GoalScore)*100);
                }

                var sortedScore = (int[])scoreArray.Clone();
                Array.Sort<int>(sortedScore,
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
                    PlayerPlace = Array.IndexOf(sortedScore, playerScore) + 1;
                }
                var topFishPlace = Array.IndexOf(sortedScore, topFishScore) + 1;
                var bottomFishPlace = Array.IndexOf(sortedScore, bottomFishScore) + 1;

                if (!lastTopFishPlace.HasValue || topFishPlace != lastTopFishPlace)
                {
                    SetFishPlaceFromNumber(TopFishInstance, topFishPlace);
                }
                if (!lastPlayerPlace.HasValue || PlayerPlace != lastPlayerPlace)
                {
                    SetFishPlaceFromNumber(PlayerFishInstance, PlayerPlace);
                }
                if (!lastBottomFishPlace.HasValue || bottomFishPlace != lastBottomFishPlace)
                {
                    SetFishPlaceFromNumber(BottomFishInstance, bottomFishPlace);
                }

                lastTopFishNum = topFishNum;
                lastBottomFishNum = bottomFishNum;

                lastTopFishScore = topFishScore;
                lastPlayerScore = PlayerScore;
                lastBottomFishScore = bottomFishScore;

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
            if (!fish.FishJumpAnimation.IsPlaying())
            {
                fish.FishJumpAnimation.Play();
            }
        }

        private void Reset()
        {
            ScoreText.Text = "0";

            lastTopFishScore = 0;
            PlayerScore = 0;
            lastBottomFishScore = 0;

            lastTopFishPlace = 8;
            lastPlayerPlace = 8;
            lastBottomFishPlace = 8;

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
