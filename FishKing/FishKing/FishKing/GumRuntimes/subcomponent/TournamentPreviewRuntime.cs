using System;
using System.Linq;
using FishKing.Enums;
using FishKing.Extensions;
using FishKing.GameClasses;
using FishKing.Managers;
using FlatRedBall.Gui;

namespace FishKing.GumRuntimes
{
    partial class TournamentPreviewRuntime
    {
        public TournamentStructure Tournament { get; private set; }

        public bool IsHighlighted => CurrentHighlightState == Highlight.Highlighted;

        public bool IsSelected => CurrentSelectionState == Selection.Selected;

        public bool RequirementsMet => CurrentRequirementsState == Requirements.Met;

        partial void CustomInitialize()
        {
            CurrentSelectionState = Selection.NotSelected;
            RollOn += HighlightButton;
            RollOff += UnhighlightButton;
            Click += TournamentPreviewRuntime_Click;
        }

        private void TournamentPreviewRuntime_Click(IWindow window)
        {
            Select();
        }

        public void Select()
        {
            if (CurrentRequirementsState == Requirements.Met)
                CurrentSelectionState = Selection.Selected;
        }

        public void Unselect()
        {
            CurrentSelectionState = Selection.NotSelected;
        }

        private void UnhighlightButton(IWindow window)
        {
            UnhighlightButton();
        }

        private void HighlightButton(IWindow window)
        {
            HighlightButton();
        }

        public void HighlightButton()
        {
            if (RequirementsMet)
                CurrentHighlightState = Highlight.Highlighted;
        }

        public void UnhighlightButton()
        {
            CurrentHighlightState = Highlight.NotHighlighted;

            //Refresh values changed by highlight change
            CurrentSelectionState = CurrentSelectionState;
        }

        public void AssociateWithTournament(TournamentStructure tournament)
        {
            var saveData = SaveGameManager.CurrentSaveData;

            Tournament = tournament;
            TournamentTitleText.Text = tournament.TournamentName;
            ParticipantsValue.Text = tournament.NumberOfParticipants.ToString();
            GoalValue.Text = tournament.GoalPoints.ToString();
            RulesValue.Text = tournament.TournamentRules.ToString();
            RewardText.Text = "$" + tournament.RewardAmount;

            if (saveData != null)
                if (saveData.MeetsRequirements(tournament))
                {
                    CurrentRequirementsState = Requirements.Met;
                }
                else
                {
                    CurrentRequirementsState = Requirements.Unmet;
                    TrophyRequirementType = TrophyTypeToTrophy(tournament.TrophyRequirements.Item1);
                    RequirementTrophyCount = tournament.TrophyRequirements.Item2.ToString();
                }

            var previousPlay = saveData?.ParticipatedTournaments?.Where(pt => pt.TournamentName == tournament.TournamentName)
                .FirstOrDefault();
            if (previousPlay != null)
            {
                CurrentPlayedState = Played.PreviousPlayed;
                TrophyDisplayInstance.SetTrophyByPlaceNumber(previousPlay.PlaceTaken);
            }
            else
            {
                CurrentPlayedState = Played.Unplayed;
            }
        }

        private TrophyCountRuntime.Trophy TrophyTypeToTrophy(TrophyTypes.TrophyType trophy)
        {
            switch (trophy)
            {
                case TrophyTypes.TrophyType.Bronze: return TrophyCountRuntime.Trophy.Bronze;
                case TrophyTypes.TrophyType.Silver: return TrophyCountRuntime.Trophy.Silver;
                case TrophyTypes.TrophyType.Gold: return TrophyCountRuntime.Trophy.Gold;
                case TrophyTypes.TrophyType.None: throw new ArgumentException("No such trophy type, none.");
            }
            return TrophyCountRuntime.Trophy.Bronze;
        }
    }
}