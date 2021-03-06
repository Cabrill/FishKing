﻿using FishKing.Entities;
using FishKing.GameClasses;
using FishKing.Managers;
using FishKing.Screens;
using FishKing.UtilityClasses;
using FlatRedBall.Glue.StateInterpolation;
using FlatRedBall.Instructions;
using StateInterpolationPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.GumRuntimes
{
    partial class TournamentStatusRuntime : IInstructable
    {
        public float AbsoluteHeight
        {
            get
            {
                return (this as RenderingLibrary.IPositionedSizedObject).Height - 14f;
            }
        }
        private int playerScore;
        public int PlayerScore
        {
            get { return playerScore; }
            private set { playerScore = value; UpdatePlayerScore(); }
        }
        private int playerFishNumber;
        public int PlayerFishNumber
        {
            get { return playerFishNumber; }
            set { playerFishNumber = value; SetPlayerFish(); }
        }
        private int goalScore;
        public int GoalScore
        {
            get { return goalScore; }
            set { goalScore = value; UpdateGoalScore(); }
        }
        private int playerPlace;
        public int PlayerPlace
        {
            get { return playerPlace; }
            private set { playerPlace = value; }
        }

        public InstructionList Instructions
        {
            get
            {
                return ((IInstructable)gameScreen).Instructions;
            }
        }

        private bool hasStartedCelebration;
        public bool HasStartedCelebration
        {
            get { return hasStartedCelebration; }
        }

        public GameScreen gameScreen;

        private int? lastTopFishPlace = null;
        private int? lastBottomFishPlace = null;
        private int? lastPlayerPlace = null;
        private int? lastTopFishNum = null;
        private int? lastBottomFishNum = null;
        private int? lastTopFishScore = null;
        private int? lastBottomFishScore = null;
        private int? lastPlayerScore = null;
        private int[] score;
        private int[] sortedScore;

        partial void CustomInitialize()
        {
            Reset();
            WaveMovementAnimation.Play();
            TopFishInstance.FishSwimAnimation.Play();
            PlayerFishInstance.FishSwimAnimation.Play();
            BottomFishInstance.FishSwimAnimation.Play();
            GlowingStarInstance.StarGlowAnimation.Play();
            TrophyDisplayInstance.TrophyPulseAnimation.Play();
            BottomFishInstance.Visible = false;
            TopFishInstance.Visible = false;
        }

        public void CustomActivity()
        {
            if (PlayerScore >= GoalScore && !ThrowConfettiAnimation.IsPlaying())
            {
                ThrowConfettiAnimation.Play();
            }
        }

        public void UpdateFishPlaceMarkers(int[] scoreArray)
        {
            score = scoreArray;
            sortedScore = (int[])scoreArray.Clone();
            Array.Sort<int>(sortedScore,
                new Comparison<int>(
                        (i1, i2) => i2.CompareTo(i1)
                ));

            playerScore = scoreArray[0];
            int maxScore = scoreArray.Max();
            int minScore = scoreArray.Min();

            bool playerInFirst = playerScore == maxScore;
            bool playerInLast = playerScore == minScore;
            playerPlace = (playerInFirst ? 1 : playerInLast ? scoreArray.Count() : Array.IndexOf(sortedScore, playerScore) + 1);

            int topFishNum = -1, bottomFishNum = -1,
                topFishPlace = 0, bottomFishPlace = 0,
                topFishScore = 0, bottomFishScore = 0;

            //Update player
            if (lastPlayerPlace != playerPlace)
            {
                SetFishPlaceFromNumber(PlayerFishInstance, playerPlace);
            }
            if (playerScore != lastPlayerScore)
            {
                JumpFishTo(PlayerFishInstance, GetGoalProgress(playerScore));
            }
            //Check if top fish needs updating
            if (lastTopFishNum.HasValue)
            {
                topFishNum = lastTopFishNum.Value;
                topFishScore = scoreArray[topFishNum];
                topFishPlace = Array.IndexOf(sortedScore, topFishScore) + 1;

                if (IsFishNearPlayerPlace(playerInFirst, playerInLast, topFishPlace) && topFishScore < goalScore)
                {
                    if (topFishScore != lastTopFishScore)
                    {
                        JumpFishTo(TopFishInstance, GetGoalProgress(topFishScore));
                    }
                    if (topFishPlace != lastTopFishPlace)
                    {
                        SetFishPlaceFromNumber(TopFishInstance, topFishPlace);
                    }
                }
                else //Choose a new fish for top
                {
                    int exceptNum = (lastBottomFishNum.HasValue ? lastBottomFishNum.Value : 0);
                    var newFish = GetSuitableFish(playerInFirst, playerInLast, exceptNum);
                    if (newFish.Item1 != -1)
                    {
                        topFishNum = newFish.Item1;
                        topFishScore = newFish.Item2;
                        topFishPlace = newFish.Item3;
                    }
                    else
                    {
                        topFishNum = -1;
                    }
                }
                if (topFishNum != lastTopFishNum.Value)
                {
                    if (scoreArray[lastTopFishNum.Value] > scoreArray[topFishNum])
                    {
                        FishPassing(TopFishInstance, topFishNum, topFishPlace, topFishScore);
                    }
                    else
                    {
                        FishPassedBy(TopFishInstance, topFishNum, topFishPlace, topFishScore);
                    }
                }
            }
            else
            {
                int exceptNum = (lastBottomFishNum.HasValue ? lastBottomFishNum.Value : 0);
                var newFish = GetSuitableFish(playerInFirst, playerInLast, exceptNum);
                if (newFish.Item1 != -1)
                {
                    topFishNum = newFish.Item1;
                    topFishScore = newFish.Item2;
                    topFishPlace = newFish.Item3;
                    TopFishInstance.TournamentFishProgress = GetGoalProgress(topFishScore);
                    SetFishPlaceFromNumber(TopFishInstance, topFishPlace);
                    SetFishTypeFromNumber(TopFishInstance, topFishNum);
                }
            }

            //Check if bottom fish needs updating
            if (lastBottomFishNum.HasValue)
            {
                bottomFishNum = lastBottomFishNum.Value;
                bottomFishScore = scoreArray[bottomFishNum];
                bottomFishPlace = Array.IndexOf(sortedScore, bottomFishScore) + 1;

                if (IsFishNearPlayerPlace(playerInFirst, playerInLast, bottomFishPlace) && bottomFishScore < goalScore)
                {
                    //Bottom fish keeps its place, update if necessary
                    if (bottomFishScore != lastBottomFishScore)
                    {
                        JumpFishTo(BottomFishInstance, GetGoalProgress(bottomFishScore));
                    }
                    if (bottomFishPlace != lastBottomFishPlace)
                    {
                        SetFishPlaceFromNumber(BottomFishInstance, bottomFishPlace);
                    }
                }
                else //Choose a new fish for bottom
                {
                    var newFish = GetSuitableFish(playerInFirst, playerInLast, topFishNum);
                    if (newFish.Item1 != -1)
                    {
                        bottomFishNum = newFish.Item1;
                        bottomFishScore = newFish.Item2;
                        bottomFishPlace = newFish.Item3;
                    }
                    else
                    {
                        bottomFishNum = -1;
                    }
                }
                if (bottomFishNum != lastBottomFishNum.Value)
                {
                    if (scoreArray[lastBottomFishNum.Value] > scoreArray[bottomFishNum])
                    {
                        FishPassing(BottomFishInstance, bottomFishNum, bottomFishPlace, bottomFishScore);
                    }
                    else
                    {
                        FishPassedBy(BottomFishInstance, bottomFishNum, bottomFishPlace, bottomFishScore);
                    }
                }
            }
            else //Choose a new fish for bottom
            {
                var newFish = GetSuitableFish(playerInFirst, playerInLast, topFishNum);
                if (newFish.Item1 != -1)
                {
                    bottomFishNum = newFish.Item1;
                    bottomFishScore = newFish.Item2;
                    bottomFishPlace = newFish.Item3;
                    BottomFishInstance.TournamentFishProgress = GetGoalProgress(bottomFishScore);
                    SetFishPlaceFromNumber(BottomFishInstance, bottomFishPlace);
                    SetFishTypeFromNumber(BottomFishInstance, bottomFishNum);
                }
            }

            //Handle tournamentplaceholder markers
            var maxPossiblePlace = score.Where(s => s >= goalScore).Count() + 1;
            if (playerPlace != maxPossiblePlace && topFishPlace != maxPossiblePlace && bottomFishPlace != maxPossiblePlace)
            {
                SetPlaceHolder(PlaceHolder1, maxPossiblePlace, GetGoalProgress(sortedScore[maxPossiblePlace - 1]));
            }
            else if (playerPlace == maxPossiblePlace && topFishPlace > 0 && bottomFishPlace > 0)
            {
                var placeToShow = Math.Max(topFishPlace, bottomFishPlace) + 1;
                if (placeToShow < score.Count())
                {
                    SetPlaceHolder(PlaceHolder1, placeToShow, GetGoalProgress(sortedScore[placeToShow - 1]));
                }
            }
            else
            {
                HidePlaceHolder(PlaceHolder1);
            }
            if (PlayerPlace > maxPossiblePlace + 2)
            {
                var placeToShow = PlayerPlace - 2;
                var scoreForPlace = sortedScore[placeToShow - 1];
                if (scoreForPlace > playerScore && scoreForPlace != topFishScore && scoreForPlace != bottomFishScore)
                {
                    SetPlaceHolder(PlaceHolder2, placeToShow, GetGoalProgress(scoreForPlace));
                }
            }
            else
            {
                HidePlaceHolder(PlaceHolder2);
            }
            
            if (topFishNum != -1)
            {
                lastTopFishNum = topFishNum;
                lastTopFishScore = topFishScore;
                lastTopFishPlace = topFishPlace;
                TopFishInstance.Visible = true;
            }
            else
            {
                lastTopFishNum = lastTopFishScore = lastTopFishPlace = null;
                TopFishInstance.Visible = false;
            }
            if (bottomFishNum != -1)
            {
                lastBottomFishNum = bottomFishNum;
                lastBottomFishScore = bottomFishScore;
                lastBottomFishPlace = bottomFishPlace;
                BottomFishInstance.Visible = true;
            }
            else
            {
                lastBottomFishNum = lastBottomFishScore = lastBottomFishPlace = null;
                BottomFishInstance.Visible = false;
            }

            lastPlayerScore = playerScore;
            lastPlayerPlace = PlayerPlace;
            PlayerScore = playerScore;

            TrophyDisplayInstance.SetTrophyByPlaceNumber(Math.Min(playerPlace,maxPossiblePlace));
        }

        private bool IsFishNearPlayerPlace(bool playerInFirst, bool playerInLast, int fishPlace)
        {
            int nonZeroPlace1 = -1;
            int nonZeroPlace2 = -1;
            var lastNonZero = sortedScore.Where(s => s > 0 && s < goalScore);
            if (lastNonZero.Count() > 0)
            {
                nonZeroPlace1 = Array.IndexOf(sortedScore, lastNonZero.Last())+1;
            }
            if (lastNonZero.Count() > 1)
            {
                nonZeroPlace2 = Array.IndexOf(sortedScore, lastNonZero.ElementAt(lastNonZero.Count() - 2))+1;
            }

            return ((fishPlace == PlayerPlace)
                    ||
                    (playerInFirst && fishPlace < 4) //Fish in top 3
                    ||
                    (playerInLast &&
                        (fishPlace == nonZeroPlace1 ||
                        fishPlace == nonZeroPlace2 ||
                        fishPlace == PlayerPlace + 1 || //Second-to-last
                        fishPlace == PlayerPlace + 2)) //Third-to-last
                    ||
                    (!playerInFirst && !playerInLast &&
                        (fishPlace == PlayerPlace + 1 || //One place ahead of player
                        fishPlace == PlayerPlace - 1))); //One place behind player
        }

        private Tuple<int, int, int> GetSuitableFish(bool playerInFirst, bool playerInLast, int exceptNum)
        {
            int fishScore = 0;
            int fishNum = 0;
            int fishPlace = 0;
            if (playerInFirst)
            {
                int possibleScore = sortedScore[1];
                int possibleFishNum = Array.IndexOf(score, possibleScore, 1);

                if (exceptNum == possibleFishNum)
                {
                    fishScore = sortedScore[2];
                    if (fishScore == possibleScore)
                    {
                        fishNum = Array.IndexOf(score, fishScore, exceptNum + 1);
                    }
                    else
                    {
                        fishNum = Array.IndexOf(score, fishScore, 1);
                    }
                }
                else
                {
                    fishScore = possibleScore;
                    fishNum = possibleFishNum;
                }
            }
            else if (playerInLast)
            {
                var nonZeroScores = sortedScore.Where(s => s > 0 && s < goalScore);
                if (nonZeroScores.Count() > 0)
                {
                    int possibleScore = nonZeroScores.Last();
                    int possibleFishNum = Array.IndexOf(score, possibleScore, 1);

                    if (exceptNum == possibleFishNum)
                    {
                        if (nonZeroScores.Count() > 1)
                        {
                            fishScore = nonZeroScores.ElementAt(nonZeroScores.Count() - 2);
                            if (fishScore == possibleScore)
                            {
                                fishNum = Array.IndexOf(score, fishScore, exceptNum + 1);
                            }
                            else
                            {
                                fishNum = Array.IndexOf(score, fishScore, 1);
                            }
                        }
                    }
                    else
                    {
                        fishScore = possibleScore;
                        fishNum = possibleFishNum;
                    }
                }
            }
            else
            {
                int possibleScore = sortedScore[PlayerPlace];
                int possibleFishNum = Array.IndexOf(score, possibleScore, 1);

                if (exceptNum == possibleFishNum)
                {
                    fishScore = sortedScore[PlayerPlace - 2];
                    if (fishScore == possibleScore)
                    {
                        fishNum = Array.IndexOf(score, fishScore, exceptNum + 1);
                    }
                    else
                    {
                        fishNum = Array.IndexOf(score, fishScore, 1);
                    }
                }
                else
                {
                    fishScore = possibleScore;
                    fishNum = possibleFishNum;
                }
            }
            fishPlace = Array.IndexOf(sortedScore, fishScore) + 1;

            if (fishScore == 0 && fishScore >= goalScore)
            {
                fishNum = -1;
            }

            return new Tuple<int, int, int>(fishNum, fishScore, fishPlace);
        }
        private float GetGoalProgress(int score)
        {
            return (float)Decimal.Divide(score, goalScore) * 100;
        }

        private void FishPassedBy(TournamentFishRuntime fish, int newFishNum, int newFishPlace, int newFishScore)
        {
            fish.Visible = true;
            fish.FishSwimAnimation.Stop();
            fish.PassedOutAnimation.Play();
            this.Call(() =>
            {
                SetFishTypeFromNumber(fish, newFishNum);
                fish.TournamentFishProgress = GetGoalProgress(newFishScore);
                SetFishPlaceFromNumber(fish, newFishPlace);
                fish.PassedInAnimation.Play();
                this.Call(fish.FishSwimAnimation.Play).After(fish.PassedInAnimation.Length);
            }).After(fish.PassedOutAnimation.Length);
        }

        private void FishPassing(TournamentFishRuntime fish, int newFishNum, int newFishPlace, int newFishScore)
        {
            fish.Visible = true;
            fish.FishSwimAnimation.Stop();
            fish.PassingOutAnimation.Play();
            this.Call(() =>
            {
                SetFishTypeFromNumber(fish, newFishNum);
                fish.TournamentFishProgress = GetGoalProgress(newFishScore);
                SetFishPlaceFromNumber(fish, newFishPlace);
                fish.PassingInAnimation.Play();
                this.Call(fish.FishSwimAnimation.Play).After(fish.PassingInAnimation.Length);
            }).After(fish.PassingOutAnimation.Length);
        }

        private void JumpFishTo(TournamentFishRuntime fish, float newPosition)
        {
            fish.Visible = true;
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

        public void Reset()
        {
            hasStartedCelebration = false;
            ScoreText.Text = "0";

            lastTopFishScore = 0;
            PlayerScore = 0;
            lastBottomFishScore = 0;

            lastTopFishPlace = -1;
            lastPlayerPlace = -1;
            lastBottomFishPlace = -1;

            TopFishInstance.TournamentFishProgress = 0;
            PlayerFishInstance.TournamentFishProgress = 0;
            BottomFishInstance.TournamentFishProgress = 0;

            TopFishInstance.CurrentTournamentPlaceState = TournamentFishRuntime.TournamentPlace.None;
            PlayerFishInstance.CurrentTournamentPlaceState = TournamentFishRuntime.TournamentPlace.None;
            BottomFishInstance.CurrentTournamentPlaceState = TournamentFishRuntime.TournamentPlace.None;

            HidePlaceHolder(PlaceHolder1);
            HidePlaceHolder(PlaceHolder2);
        }

        private void SetPlaceHolder(TournamentPlaceHolderRuntime holder, int place, float progress)
        {
            TournamentPlaceHolderRuntime.Place placeState;
            switch (place)
            {
                case 1: placeState = TournamentPlaceHolderRuntime.Place.First; break;
                case 2: placeState = TournamentPlaceHolderRuntime.Place.Second; break;
                case 3: placeState = TournamentPlaceHolderRuntime.Place.Third; break;
                case 4: placeState = TournamentPlaceHolderRuntime.Place.Fourth; break;
                case 5: placeState = TournamentPlaceHolderRuntime.Place.Fifth; break;
                case 6: placeState = TournamentPlaceHolderRuntime.Place.Sixth; break;
                case 7: placeState = TournamentPlaceHolderRuntime.Place.Seventh; break;
                case 8: placeState = TournamentPlaceHolderRuntime.Place.Eighth; break;
                default: placeState = TournamentPlaceHolderRuntime.Place.First; break;
            }

            holder.TournamentPlaceHolderProgress = progress;
            holder.CurrentPlaceState = placeState;
            holder.Visible = true;
            if (!holder.PlacePulseAnimation.IsPlaying())
            {
                holder.PlacePulseAnimation.Play();
            }
        }

        private void HidePlaceHolder(TournamentPlaceHolderRuntime holder)
        {
            holder.PlacePulseAnimation.Stop();
            holder.Visible = false;
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
            GlowingStarInstance.StarFlashAnimation.Play();
        }

        public void SetPlayerFish()
        {
            SetFishTypeFromNumber(PlayerFishInstance, PlayerFishNumber);
        }

        private void SetFishTypeFromNumber(TournamentFishRuntime fish, int fishNumber)
        {
            if (fish != PlayerFishInstance && fishNumber == PlayerFishNumber)
            {
                fishNumber = 0;
            }

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

        public void Setup(GameScreen screen)
        {
#if DEBUG
            if (DebuggingVariables.MapDebugMode) return;
#endif
            this.gameScreen = screen;
            PlayerFishNumber = SaveGameManager.CurrentSaveData.PlayerFishNumber;
            GoalScore = TournamentManager.CurrentTournament.GoalPoints;

            var currentScores = TournamentManager.CurrentScores.AsArray;

            if (currentScores.Sum() > 0)
            {
                SetupFromScores(currentScores);
            }
        }

        private void SetupFromScores(int[] existingScore)
        {
            sortedScore = (int[])existingScore.Clone();
            Array.Sort<int>(sortedScore,
                new Comparison<int>(
                        (i1, i2) => i2.CompareTo(i1)
                ));

            playerScore = existingScore[0];

            int maxScore = existingScore.Max();
            int minScore = existingScore.Min();

            bool playerInFirst = playerScore == maxScore;
            bool playerInLast = playerScore == minScore;
            playerPlace = (playerInFirst ? 1 : playerInLast ? existingScore.Count() : Array.IndexOf(sortedScore, playerScore) + 1);
            lastPlayerPlace = playerPlace;
            lastPlayerScore = playerScore;

            SetFishPlaceFromNumber(PlayerFishInstance, playerPlace);
            PlayerFishInstance.TournamentFishProgress = GetGoalProgress(playerScore);
            UpdateFishPlaceMarkers(existingScore);
        }

        public void StartCelebration()
        {
            hasStartedCelebration = true;
            ThrowConfettiAnimation.Play();
        }
    }
}
