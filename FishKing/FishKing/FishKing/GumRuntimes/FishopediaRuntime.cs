using FishKing.DataTypes;
using FishKing.GameClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FishKing.Entities;

namespace FishKing.GumRuntimes
{
    partial class FishopediaRuntime
    {


        private enum FishTypeDisplay { All, Caught};
        private FishTypeDisplay currentlyDisplaying;
        private int pageIndex;
        public SerializableDictionary<Fish_Types, FishRecord> CaughtFish
        {
            get; set;
        }

        private List<FishEntryRuntime> fishEntries;

        partial void CustomInitialize()
        {
            pageIndex = 0;
            LeftPageCorner.Click += LeftPageCorner_Click;
            RightPageCorner.Click += RightPageCorner_Click;
            BookmarkAll.Click += BookmarkAll_Click;
            BookmarkCaught.Click += BookmarkCaught_Click;
            BookmarkClose.Click += BookmarkClose_Click;
            BookmarkClose.Unselect();
            fishEntries = LeftPage.Children.Union(RightPage.Children).Where(c => c is FishEntryRuntime).Cast<FishEntryRuntime>().ToList();
        }

        private void BookmarkClose_Click(FlatRedBall.Gui.IWindow window)
        {
            currentlyDisplaying = FishTypeDisplay.All;
            BookmarkClose.Unselect();
            pageIndex = 0;
            this.Visible = false;
        }

        private void BookmarkCaught_Click(FlatRedBall.Gui.IWindow window)
        {
            if (currentlyDisplaying != FishTypeDisplay.Caught)
            {
                pageIndex = 0;
                LoadCaughtFish();
            }
        }

        private void BookmarkAll_Click(FlatRedBall.Gui.IWindow window)
        {
            if (currentlyDisplaying != FishTypeDisplay.All)
            {
                pageIndex = 0;
                LoadAllFish();
            }
        }

        private void RightPageCorner_Click(FlatRedBall.Gui.IWindow window)
        {
            NextPage();
        }

        public void NextPage()
        {
            int maxPage;
            switch (currentlyDisplaying)
            {
                case FishTypeDisplay.All:
                    maxPage = GlobalContent.Fish_Types.Keys.Count / 12;
                    if (pageIndex < maxPage)
                    {
                        pageIndex++;
                        LoadAllFish();
                    }
                    break;
                case FishTypeDisplay.Caught:
                    maxPage = CaughtFish.Keys.Count / 12;
                    if (pageIndex < maxPage)
                    {
                        pageIndex++;
                        LoadCaughtFish();
                    }
                    break;
            }
        }

        private void LeftPageCorner_Click(FlatRedBall.Gui.IWindow window)
        {
            PreviousPage();
        }

        public void PreviousPage()
        {
            if (pageIndex > 0)
            {
                pageIndex--;
                switch (currentlyDisplaying)
                {
                    case FishTypeDisplay.All: LoadAllFish(); break;
                    case FishTypeDisplay.Caught: LoadCaughtFish(); break;
                }
            }
        }

        public void LoadAllFish()
        {
            currentlyDisplaying = FishTypeDisplay.All;
            LeftPageDivider.Visible = false;
            RightPageDivider.Visible = false;

            var allFish = GlobalContent.Fish_Types;
            FishRecord fishRecord;
            Fish_Types fishType;
            FishEntryRuntime fishEntry;
            bool hasRightPage = false;

            var untilItem = (1 + pageIndex) * 12;
            untilItem = (int)(Math.Round(untilItem / 12.0) * 12);

            for (int i = pageIndex*12;i < untilItem; i++)
            {
                fishEntry = fishEntries.ElementAt(i % 12);

                if (i < allFish.Count)
                {
                    fishType = allFish.Values.ElementAt(i);
                    if (CaughtFish.ContainsKey(fishType))
                    {
                        fishRecord = CaughtFish[fishType];
                    }
                    else
                    {
                        fishRecord = null;
                    }
                    fishEntry.AssociateWithFish(fishType, fishRecord);
                    fishEntry.Visible = true;

                    if (i % 12 == 3)
                    {
                        LeftPageDivider.Visible = true;
                    }
                    if (i % 12 == 9)
                    {
                        RightPageDivider.Visible = true;
                    }

                    hasRightPage = i % 12 >= 6;
                }
                else
                {
                    fishEntry.Visible = false;
                }
            }

            var maxPage = GlobalContent.Fish_Types.Keys.Count / 12;

            RightPageCorner.Visible = maxPage > pageIndex;
            LeftPageCorner.Visible = pageIndex > 0;
            SetPageNumbers(pageIndex, hasRightPage);

            SelectBookmark(BookmarkAll);
        }

        internal void HandleExit()
        {
            BookmarkClose.CallClick();
        }

        internal void HandleSelection()
        {
            if (LeftPageCorner.IsHighlighted)
            {
                LeftPageCorner.CallClick();
            }
            else if (RightPageCorner.IsHighlighted)
            {
                RightPageCorner.CallClick();
            }
            else if (BookmarkAll.IsHighlighted)
            {
                BookmarkAll.CallClick();
            }
            else if (BookmarkCaught.IsHighlighted)
            {
                BookmarkCaught.CallClick();
            }
            else if (BookmarkClose.IsHighlighted)
            {
                BookmarkClose.CallClick();
            }
        }

        internal void HandleMovement(Direction desiredDirection)
        {
            switch (desiredDirection)
            {
                case Direction.Up:
                    if (BookmarkClose.IsHighlighted) RightPageCorner.HighlightButton();
                    if (BookmarkAll.IsHighlighted || BookmarkCaught.IsHighlighted) LeftPageCorner.HighlightButton();
                    BookmarkAll.UnhighlightButton();
                    BookmarkCaught.UnhighlightButton();
                    BookmarkClose.UnhighlightButton();
                    break;
                case Direction.Down:
                    if (LeftPageCorner.IsHighlighted) BookmarkAll.HighlightButton();
                    if (RightPageCorner.IsHighlighted) BookmarkClose.HighlightButton();
                    LeftPageCorner.UnhighlightButton();
                    RightPageCorner.UnhighlightButton();
                    break;
                case Direction.Left:
                    if (RightPageCorner.IsHighlighted) LeftPageCorner.HighlightButton();
                    if (BookmarkClose.IsHighlighted) BookmarkCaught.HighlightButton();
                    if (BookmarkCaught.IsHighlighted)
                    {
                        BookmarkCaught.UnhighlightButton();
                        BookmarkAll.HighlightButton();
                    }

                    RightPageCorner.UnhighlightButton();
                    BookmarkClose.UnhighlightButton();
                    break;
                case Direction.Right:
                    if (LeftPageCorner.IsHighlighted) RightPageCorner.HighlightButton();
                    if (BookmarkAll.IsHighlighted) BookmarkCaught.HighlightButton();
                    if (BookmarkCaught.IsHighlighted)
                    {
                        BookmarkCaught.UnhighlightButton();
                        BookmarkClose.HighlightButton();
                    }
                    BookmarkAll.UnhighlightButton();
                    LeftPageCorner.UnhighlightButton();
                    break;
            }
        }

        private void LoadCaughtFish(int atPageIndex = 0)
        {
            currentlyDisplaying = FishTypeDisplay.Caught;
            LeftPageDivider.Visible = false;
            RightPageDivider.Visible = false;

            var allFish = CaughtFish.Keys;
            FishRecord fishRecord;
            Fish_Types fishType;
            FishEntryRuntime fishEntry;
            bool hasRightPage = false;

            var untilItem = (1 + pageIndex) * 12;
            untilItem = (int)(Math.Round(untilItem / 12.0) * 12);

            for (int i = pageIndex * 12; i < untilItem; i++)
            {
                fishEntry = fishEntries.ElementAt(i % 12);

                if (i < allFish.Count)
                {
                    fishType = allFish.ElementAt(i);
                    fishRecord = CaughtFish[fishType];

                    fishEntry.AssociateWithFish(fishType, fishRecord);
                    fishEntry.Visible = true;

                    if (i % 12 == 3)
                    {
                        LeftPageDivider.Visible = true;
                    }
                    if (i % 12 == 9)
                    {
                        RightPageDivider.Visible = true;
                    }

                    hasRightPage = i % 12 >= 6;
                }
                else
                {
                    fishEntry.Visible = false;
                }
            }

            var maxPage = CaughtFish.Keys.Count / 12;

            RightPageCorner.Visible = maxPage > pageIndex;
            LeftPageCorner.Visible = pageIndex > 0;
            SetPageNumbers(pageIndex, hasRightPage);
            SelectBookmark(BookmarkCaught);
        }

        private void SetPageNumbers(int page, bool hasRightPage)
        {
            LeftPageNumber.Text = (page*2 + 1).ToString();
            RightPageNumber.Text = (page*2 + 2).ToString();
            RightPageNumber.Visible = hasRightPage;
        }

        private void SelectBookmark(BookMarkRuntime bookmark)
        {
            BookmarkAll.Unselect();
            BookmarkCaught.Unselect();

            bookmark.Select();
        }
    }
}
