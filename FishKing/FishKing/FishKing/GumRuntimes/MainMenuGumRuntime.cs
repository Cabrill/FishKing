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

namespace FishKing.GumRuntimes
{
    partial class MainMenuGumRuntime
    {
        private float scrollAmount;

        private float ContainerY
        {
            get { return (TournamentPreviewContainer as IPositionedSizedObject).Y; }
        }

        private float ContainerHeight
        {
            get { return  (TournamentPreviewContainer as IPositionedSizedObject).Height; }
        }

        private float PreviewHeight
        {
            get { return (TournamentPreviews.FirstOrDefault() as IPositionedSizedObject).Height; }
        }

        public List<TournamentPreviewRuntime> TournamentPreviews
        {
            get { return TournamentPreviewContainer.Children.Cast<TournamentPreviewRuntime>().ToList(); }
        }

        public TournamentPreviewRuntime CurrentlyHighlightedTournament
        {
            get { return TournamentPreviews.Where(tp => tp.IsHighlighted).FirstOrDefault(); }
        }

        public bool TournamentIsSelected
        {
            get { return TournamentPreviewContainer.Children.Where(tp => (tp as TournamentPreviewRuntime).IsSelected).Any(); }
        }

        public bool AnyTournamentIsHighlighted
        {
            get { return (CurrentlyHighlightedTournament != null); }
        }

        public bool FirstTournamentPreviewIsHighlighted
        {
            get { return TournamentPreviews.First().IsHighlighted; }
        }

        public bool LastTournamentPreviewIsHighlighted
        {
            get { return TournamentPreviews.Last().IsHighlighted; }
        }

        partial void CustomInitialize()
        {
            LoadTournamentList();
            UnhighlightAllTournaments();
            UnselectAllTournaments();
            GoFishButton.Visible = TournamentIsSelected;
            scrollAmount = 0;
        }

        public void HandleTournamentPreviewMovement(Direction desiredDirection)
        {
            switch (desiredDirection)
            {
                case Direction.Up:
                    if (!AnyTournamentIsHighlighted)
                    {
                        GetBottomVisibleTournamentPreview().HighlightButton();
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
            if (TournamentIsSelected)
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
            if (previous != null)
            {
                UnhighlightAllTournaments();
                previous.HighlightButton();
                if ((previous as IPositionedSizedObject).Y + PreviewHeight < ContainerY)
                {
                    ScrollUp();
                }
            }
        }

        private void HighlightNextTournamentPreview()
        {
            var current = CurrentlyHighlightedTournament;
            var currentIdx = TournamentPreviews.IndexOf(current);

            if (currentIdx == TournamentPreviews.Count - 1) return;

            var next = TournamentPreviews.ElementAt(currentIdx + 1);
            if (next != null)
            {
                UnhighlightAllTournaments();
                next.HighlightButton();
                if ((next as IPositionedSizedObject).Y + PreviewHeight > ContainerY + ContainerHeight)
                {
                    ScrollDown();
                }
            }
        }

        public void HandleScrollInput(float scrollWheel)
        {
            SetScrollAmount(scrollAmount + (scrollWheel*10));
        }

        private void ScrollUp()
        {
            SetScrollAmount(scrollAmount + PreviewHeight);
        }

        private void ScrollDown()
        {
            SetScrollAmount(scrollAmount - PreviewHeight);
        }

        private void SetScrollAmount(float scroll)
        {
            scrollAmount = MathHelper.Clamp(scroll, -PreviewHeight * (TournamentPreviews.Count - 3), 0);
            TournamentPreviews.First().Y = 100 * scrollAmount/ContainerHeight;
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
            foreach (TournamentStructure tournament in MasterTournamentList.AllTournaments)
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
            UnselectAllTournamentsExcept(tournamentSelected);
        }

        private void UnselectAllTournamentsExcept(TournamentPreviewRuntime tournamentSelected)
        {
            TournamentPreviews.Where(tp => tp != tournamentSelected).ForEach(tp => tp.Unselect());
        }

        private TournamentPreviewRuntime GetBottomVisibleTournamentPreview()
        {
            var container = TournamentPreviewContainer;
            var returnTP = container.Children.First() as TournamentPreviewRuntime;

            for (int i = container.Children.Count - 1; i > 0; i--)
            {
                if ((container.Children[i].Y + PreviewHeight) <= ContainerY + ContainerHeight + scrollAmount)
                {
                    returnTP = container.Children[i] as TournamentPreviewRuntime;
                    break;
                }
            }
            return returnTP;
        }
    }
}
