using FishKing.Enums;
using FishKing.Extensions;
using FishKing.GameClasses;
using FishKing.Managers;
using FishKing.UtilityClasses;
using FlatRedBall.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.GumRuntimes
{
    partial class TournamentPreviewRuntime
    {
        public TournamentStructure Tournament { get; private set; }

        public bool IsHighlighted
        {
            get { return CurrentHighlightState == Highlight.Highlighted; }
        }

        public bool IsSelected
        {
            get { return CurrentSelectionState == Selection.Selected;  }
        }

        public bool RequirementsMet
        {
            get { return CurrentRequirementsState == Requirements.Met; }
        }

        partial void CustomInitialize()
        {
            CurrentSelectionState = Selection.NotSelected;
            this.RollOn += HighlightButton;
            this.RollOff += UnhighlightButton;
            this.Click += TournamentPreviewRuntime_Click;
        }

        private void TournamentPreviewRuntime_Click(IWindow window)
        {
            Select();
        }

        public void Select()
        {
            if (CurrentRequirementsState == Requirements.Met)
            {
            CurrentSelectionState = Selection.Selected;
            }
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
            {
                this.CurrentHighlightState = Highlight.Highlighted;
            }
        }

        public void UnhighlightButton()
        {
            this.CurrentHighlightState = Highlight.NotHighlighted;

            //Refresh values changed by highlight change
            CurrentSelectionState = CurrentSelectionState;
        }

        public void AssociateWithTournament(TournamentStructure tournament)
        {
            var saveData = SaveGameManager.CurrentSaveData;

            Tournament = tournament;
            this.TournamentTitleText.Text = tournament.TournamentName;
            this.ParticipantsValue.Text = tournament.NumberOfParticipants.ToString();
            this.GoalValue.Text = tournament.GoalPoints.ToString();
            this.RulesValue.Text = tournament.TournamentRules.ToString();
            this.RewardText.Text = "$"+tournament.RewardAmount.ToString();

            if (saveData != null)
            {
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
            }

            var previousPlay = saveData?.ParticipatedTournaments?.Where(pt => pt.Tournament == tournament).FirstOrDefault();
            if (previousPlay != null)
            {
                this.CurrentPlayedState = Played.PreviousPlayed;
                this.TrophyDisplayInstance.SetTrophyByPlaceNumber(previousPlay.PlaceTaken);
            }
            else
            {
                this.CurrentPlayedState = Played.Unplayed;
            }
        }

        private TrophyCountRuntime.Trophy TrophyTypeToTrophy(TrophyTypes.TrophyType trophy)
        {
            switch(trophy)
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

