﻿using FishKing.DataTypes;
using FishKing.GameClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        partial void CustomInitialize()
        {
            pageIndex = 0;
            LeftPageCorner.Click += LeftPageCorner_Click;
            RightPageCorner.Click += RightPageCorner_Click;
            BookmarkAll.Click += BookmarkAll_Click;
            BookmarkCaught.Click += BookmarkCaught_Click;
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
            int maxPage;
            switch(currentlyDisplaying)
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
            LeftPage.Children.Clear();
            RightPage.Children.Clear();

            var allFish = GlobalContent.Fish_Types;
            FishRecord fishRecord;
            Fish_Types fishType;

            var untilItem = (1 + pageIndex) * 12;

            for (int i = pageIndex*12; i < untilItem && i < allFish.Values.Count; i++)
            {
                var newFishEntry = new FishEntryRuntime();
                fishType = allFish.Values.ElementAt(i);
                if (CaughtFish.ContainsKey(fishType))
                {
                    fishRecord = CaughtFish[fishType];
                }
                else
                {
                    fishRecord = null;
                }
                newFishEntry.AssociateWithFish(fishType, fishRecord);

                newFishEntry.Parent = (i % 12 < 6 ? LeftPage : RightPage);
            }

            var maxPage = GlobalContent.Fish_Types.Keys.Count / 12;

            RightPageCorner.Visible = maxPage > pageIndex;
            LeftPageCorner.Visible = pageIndex > 0;
        }

        private void LoadCaughtFish(int atPageIndex = 0)
        {
            currentlyDisplaying = FishTypeDisplay.Caught;
            LeftPage.Children.Clear();
            RightPage.Children.Clear();

            var allFish = CaughtFish.Keys;
            FishRecord fishRecord;
            Fish_Types fishType;

            var untilItem = (1 + pageIndex) * 12;

            for (int i = pageIndex * 12; i < untilItem && i < allFish.Count; i++)
            {
                var newFishEntry = new FishEntryRuntime();
                fishType = allFish.ElementAt(i);
                fishRecord = CaughtFish[fishType];

                newFishEntry.AssociateWithFish(fishType, fishRecord);

                newFishEntry.Parent = (i % 12 < 6 ? LeftPage : RightPage);
            }

            var maxPage = CaughtFish.Keys.Count / 12;

            RightPageCorner.Visible = maxPage > pageIndex;
            LeftPageCorner.Visible = pageIndex > 0;
        }
    }
}
