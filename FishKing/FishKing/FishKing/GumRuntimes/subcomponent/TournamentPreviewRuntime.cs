using FishKing.GameClasses;
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
            this.CurrentHighlightState = Highlight.Highlighted;
        }

        public void UnhighlightButton()
        {
            this.CurrentHighlightState = Highlight.NotHighlighted;
        }

        public void AssociateWithTournament(TournamentStructure tournament)
        {
            Tournament = tournament;
            this.TournamentTitleText.Text = tournament.TournamentName;
            this.ParticipantsValue.Text = tournament.NumberOfParticipants.ToString();
            this.GoalValue.Text = tournament.GoalPoints.ToString();
            this.RulesValue.Text = tournament.TournamentRules.ToString();
            this.RewardText.Text = "$"+tournament.RewardAmount.ToString();
        }
    }
}

