using FishKing.DataTypes;
using FishKing.GameClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FishKing.Entities;
using Microsoft.Xna.Framework.Audio;
using FishKing.Managers;

namespace FishKing.GumRuntimes
{
    partial class FishopediaRuntime
    {
        public SoundEffectInstance PageTurnSound;
        public SoundEffectInstance BookCloseSound;

        private enum FishTypeDisplay { All, Caught};
        private FishTypeDisplay _currentlyDisplaying;
        private int _pageIndex;
        public SerializableDictionary<string, FishRecord> CaughtFish
        {
            private get; set;
        }

        private List<FishEntryRuntime> _fishEntries;
        private List<BookMarkRuntime> _bookmarks;

        partial void CustomInitialize()
        {
            PageTurnSound = GlobalContent.PageTurn.CreateInstance();
            _pageIndex = 0;
            LeftPageCorner.Click += LeftPageCorner_Click;
            RightPageCorner.Click += RightPageCorner_Click;
            BookmarkAll.Click += BookmarkAll_Click;
            BookmarkCaught.Click += BookmarkCaught_Click;
            BookmarkClose.Click += BookmarkClose_Click;
            BookmarkClose.Unselect();
            _fishEntries = LeftPage.Children.Union(RightPage.Children).Where(c => c is FishEntryRuntime).Cast<FishEntryRuntime>().ToList();
            _bookmarks = LeftBookmarkContainer.Children.Union(RightBookmarkContainer.Children).Where(bm => bm is BookMarkRuntime).Cast<BookMarkRuntime>().ToList();
        }

        private void BookmarkClose_Click(FlatRedBall.Gui.IWindow window)
        {
            _currentlyDisplaying = FishTypeDisplay.All;
            BookmarkClose.Unselect();
            _pageIndex = 0;
            this.Visible = false;
            BookCloseSound.Volume = OptionsManager.Options.SoundEffectsVolume;
            BookCloseSound.Play();
        }

        private void BookmarkCaught_Click(FlatRedBall.Gui.IWindow window)
        {
            if (_currentlyDisplaying != FishTypeDisplay.Caught)
            {
                _pageIndex = 0;
                LoadCaughtFish();
            }
        }

        private void BookmarkAll_Click(FlatRedBall.Gui.IWindow window)
        {
            if (_currentlyDisplaying != FishTypeDisplay.All)
            {
                _pageIndex = 0;
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
            switch (_currentlyDisplaying)
            {
                case FishTypeDisplay.All:
                    maxPage = GlobalContent.Fish_Types.Keys.Count / 12;
                    if (_pageIndex < maxPage)
                    {
                        _pageIndex++;
                        LoadAllFish();
                        PlayPageTurnSound();
                    }
                    break;
                case FishTypeDisplay.Caught:
                    maxPage = CaughtFish.Keys.Count / 12;
                    if (_pageIndex < maxPage)
                    {
                        _pageIndex++;
                        LoadCaughtFish();
                        PlayPageTurnSound();
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
            if (_pageIndex > 0)
            {
                _pageIndex--;
                switch (_currentlyDisplaying)
                {
                    case FishTypeDisplay.All: LoadAllFish(); break;
                    case FishTypeDisplay.Caught: LoadCaughtFish(); break;
                }
                PlayPageTurnSound();
            }
        }

        public void LoadAllFish()
        {
            _currentlyDisplaying = FishTypeDisplay.All;
            LeftPageDivider.Visible = false;
            RightPageDivider.Visible = false;

            var allFish = GlobalContent.Fish_Types;
            FishRecord fishRecord;
            Fish_Types fishType;
            FishEntryRuntime fishEntry;
            bool hasRightPage = false;

            var untilItem = (1 + _pageIndex) * 12;
            untilItem = (int)(Math.Round(untilItem / 12.0) * 12);

            for (int i = _pageIndex*12;i < untilItem; i++)
            {
                fishEntry = _fishEntries.ElementAt(i % 12);

                if (i < allFish.Count)
                {
                    fishType = allFish.Values.ElementAt(i);
                    fishRecord = CaughtFish.ContainsKey(fishType.Name) ? CaughtFish[fishType.Name] : null;
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

            RightPageCorner.Visible = maxPage > _pageIndex;
            LeftPageCorner.Visible = _pageIndex > 0;
            SetPageNumbers(_pageIndex, hasRightPage);

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
            _currentlyDisplaying = FishTypeDisplay.Caught;
            LeftPageDivider.Visible = false;
            RightPageDivider.Visible = false;

            var allFish = CaughtFish.Keys;
            FishRecord fishRecord;
            Fish_Types fishType;
            FishEntryRuntime fishEntry;
            bool hasRightPage = false;

            var untilItem = (1 + _pageIndex) * 12;
            untilItem = (int)(Math.Round(untilItem / 12.0) * 12);

            for (int i = _pageIndex * 12; i < untilItem; i++)
            {
                fishEntry = _fishEntries.ElementAt(i % 12);

                if (i < allFish.Count)
                {
                    fishType = GlobalContent.Fish_Types[allFish.ElementAt(i)];
                    fishRecord = CaughtFish[fishType.Name];

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

            RightPageCorner.Visible = maxPage > _pageIndex;
            LeftPageCorner.Visible = _pageIndex > 0;
            SetPageNumbers(_pageIndex, hasRightPage);
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

        private void PlayPageTurnSound()
        {
            PageTurnSound.Volume = OptionsManager.Options.SoundEffectsVolume;
            PageTurnSound.Play();
        }

        public void SelectNextBookmark()
        {
            var currentBookmark = _bookmarks.FirstOrDefault(bm => bm.IsHighlighted);
            if (currentBookmark != null)
            {
                currentBookmark.Unselect();
                _bookmarks.SkipWhile(bm => bm != currentBookmark).Skip(1).FirstOrDefault()?.CallClick();
            }
        }

        public void SelectPreviousBookmark()
        {
            var currentBookmark = _bookmarks.FirstOrDefault(bm => bm.IsHighlighted);
            if (currentBookmark != null)
            {
                currentBookmark.Unselect();
                _bookmarks.SkipWhile(bm => bm != currentBookmark).Skip(-1).FirstOrDefault()?.CallClick();
            }
        }
    }
}
