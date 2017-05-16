using FishKing.Entities;
using FishKing.GameClasses;
using Microsoft.Xna.Framework;
using RenderingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlatRedBall.Gui;
using FishKing.Extensions;
using FishKing.Managers;
using FishKing.UtilityClasses;

namespace FishKing.GumRuntimes
{
    partial class MainMenuGumRuntime
    {
        private float _scrollAmount;
        public List<TournamentPreviewRuntime> TournamentPreviews => TournamentPreviewContainer.Children.Cast<TournamentPreviewRuntime>().ToList();

        private float ContainerY => (TournamentPreviewContainer as IPositionedSizedObject).Y;

        private float ContainerHeight => (TournamentPreviewContainer as IPositionedSizedObject).Height;

        private float PreviewHeight
        {
            get
            {
                var positionedSizedObject = (IPositionedSizedObject) TournamentPreviews.FirstOrDefault();
                return positionedSizedObject?.Height ?? 0f;
            }
        }

        public TournamentPreviewRuntime CurrentlyHighlightedTournament
        {
            get { return TournamentPreviews.FirstOrDefault(tp => tp.IsHighlighted); }
        }

        public TournamentPreviewRuntime CurrentlySelectedTournament
        {
            get { return TournamentPreviews.FirstOrDefault(tp => tp.IsSelected); }
        }

        public bool AnyTournamentIsSelected => CurrentlySelectedTournament != null;

        public bool AnyTournamentIsHighlighted => (CurrentlyHighlightedTournament != null);

        public bool FirstTournamentPreviewIsHighlighted => TournamentPreviews.First().IsHighlighted;

        public bool LastTournamentPreviewIsHighlighted
        {
            get { return TournamentPreviews.Last(tp => tp.RequirementsMet).IsHighlighted; }
        }

        partial void CustomInitialize()
        {
            LoadTournamentList();
            UnhighlightAllTournaments();
            UnselectAllTournaments();
            GoFishButton.Visible = AnyTournamentIsSelected;
            _scrollAmount = 0;
            DisplaySaveDataInfo();
        }

        private void DisplaySaveDataInfo()
        {
            var currentSave = SaveGameManager.CurrentSaveData;

            if (currentSave != null)
            {
                TimePlayedValue.Text = InfoToString.Time(currentSave.TimePlayed);
                FishCaughtValue.Text = currentSave.NumberOfFishCaught.ToString();
                LongestFishValue.Text = InfoToString.Length(currentSave.LongestFish);
                HeaviestFishValue.Text = InfoToString.Weight(currentSave.HeaviestFish);
                GoldTrophyCount.TrophyCountText = currentSave.NumberOfGoldTrophies.ToString();
                SilverTrophyCount.TrophyCountText = currentSave.NumberOfSilverTrophies.ToString();
                BronzeTrophyCount.TrophyCountText = currentSave.NumberOfBronzeTrophies.ToString();
                MoneyText.Text = "$" + currentSave.MoneyAvailable.ToString();
                //RarestFishValue.Text = ???
            }
        }

        public void HandleTournamentPreviewMovement(Direction desiredDirection)
        {
            switch (desiredDirection)
            {
                case Direction.Up:
                    if (!AnyTournamentIsHighlighted)
                    {
                        ScrollToAndHighlightLastEligibleTournament();
                    }
                    else 
                    {
                        HighlightPreviousTournamentPreview();
                    }
                    break;
                case Direction.Down:
                    HighlightNextTournamentPreview();
                    break;
            }
        }

        public void HandleSelect()
        {
            if (CurrentlyHighlightedTournament != null)
            {
                CurrentlyHighlightedTournament.CallClick();
                CurrentlyHighlightedTournament.UnhighlightButton();
            }
            if (AnyTournamentIsSelected)
            {
                GoFishButton.Visible = true;
                GoFishButton.HighlightButton();
            }
        }

        private void HighlightPreviousTournamentPreview()
        {
            var current = CurrentlyHighlightedTournament;
            var currentIdx = TournamentPreviews.IndexOf(current);

            if (currentIdx == 0) return;

            var previous = TournamentPreviews.ElementAt(currentIdx - 1);
            if (previous == null) return;

            UnhighlightAllTournaments();
            previous.HighlightButton();
            if (((IPositionedSizedObject) previous).Y < ContainerY)
            {
                ScrollUp();
            }
        }

        private void HighlightNextTournamentPreview()
        {
            var current = CurrentlyHighlightedTournament;
            var currentIdx = TournamentPreviews.IndexOf(current);

            if (currentIdx == TournamentPreviews.Count - 1) return;

            var next = TournamentPreviews.ElementAt(currentIdx + 1);
            if (next == null) return;

            UnhighlightAllTournaments();
            next.HighlightButton();
            if (((IPositionedSizedObject) next).Y+PreviewHeight > ContainerY+ContainerHeight)
            {
                ScrollDown();
            }
        }

        private void ScrollToAndHighlightLastEligibleTournament()
        {
            var lastTournament = GetLastEligibleTournament();

            lastTournament.HighlightButton();
            ScrollTo(lastTournament);
        }

        public void ScrollToAndHighlightFirstEligibleUnplayedTournament()
        {
            var firstTourmament = GetFirstEligibleUnplayedTournament();

            firstTourmament.HighlightButton();
            ScrollTo(firstTourmament);
        }

        public void HandleScrollInput(float scrollWheel)
        {
            SetScrollAmount(_scrollAmount + (scrollWheel*10));
        }

        public void ScrollTo(TournamentPreviewRuntime lastTournament)
        {
            var idx = TournamentPreviews.IndexOf(lastTournament);

            SetScrollAmount(-idx * PreviewHeight);
        }

        private void ScrollUp()
        {
            SetScrollAmount(_scrollAmount + PreviewHeight);
        }

        private void ScrollDown()
        {
            SetScrollAmount(_scrollAmount - PreviewHeight);
        }

        private void SetScrollAmount(float scroll)
        {
            if (FirstTournamentPreviewIsHighlighted)
            {
                _scrollAmount = 0;
            }
            else
            {
                _scrollAmount = MathHelper.Clamp(scroll, -PreviewHeight * (TournamentPreviews.Count - 3), 0);
            }
            TournamentPreviews.First().Y = 100 * _scrollAmount/ContainerHeight;
        }

        public void UnhighlightAllTournaments()
        {
            TournamentPreviews.ForEach(tp => tp.UnhighlightButton());
        }

        public void UnselectAllTournaments()
        {
            TournamentPreviews.ForEach(tp => tp.Unselect());
        }

        private void LoadTournamentList()
        {

            TournamentPreviewContainer.Children.Clear();
            var allTournaments = MasterTournamentList.AllTournaments;
            allTournaments.Sort(new TournamentSorter());
            foreach (TournamentStructure tournament in allTournaments)
            {
                var tournamentPreview = new TournamentPreviewRuntime();
                tournamentPreview.AssociateWithTournament(tournament);
                tournamentPreview.Parent = TournamentPreviewContainer;
                tournamentPreview.Click += HandleTournamentPreviewClick;
            }
        }

        private void HandleTournamentPreviewClick(IWindow window)
        {
            var tournamentSelected = window as TournamentPreviewRuntime;
            if (tournamentSelected != null && tournamentSelected.RequirementsMet)
            {
                UnselectAllTournamentsExcept(tournamentSelected);
            }
        }

        private void UnselectAllTournamentsExcept(TournamentPreviewRuntime tournamentSelected)
        {
            TournamentPreviews.Where(tp => tp != tournamentSelected).ForEach(tp => tp.Unselect());
        }

        private TournamentPreviewRuntime GetLastEligibleTournament()
        {
            var previews = TournamentPreviews.Where(tp => tp.RequirementsMet);
            return previews.LastOrDefault();
        }

        private TournamentPreviewRuntime GetFirstEligibleUnplayedTournament()
        {
            var previews = TournamentPreviews.Where(tp => tp.RequirementsMet);
            var tournamentPreviewRuntimes = previews as IList<TournamentPreviewRuntime> ?? previews.ToList();
            var result = tournamentPreviewRuntimes.FirstOrDefault(tp => tp.CurrentPlayedState == TournamentPreviewRuntime.Played.Unplayed) ??
                         tournamentPreviewRuntimes.FirstOrDefault();
            return result;
        }
    }
}
