﻿using FishKing.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.GameClasses
{
    public class FishRecord
    {
        public string FishName { get; private set; }
        public int TimesCaught { get; private set; }
        public int HeaviestCaught { get; private set; }
        public int LongestCaught { get; private set; }

        public FishRecord(string fishName, int weight, int length)
        {
            FishName = fishName;
            TimesCaught = 1;
            HeaviestCaught = weight;
            LongestCaught = length;
        }

        public void AddFish(Fish fish)
        {
            if (fish.Name == this.FishName)
            {
                TimesCaught++;
                if (fish.LengthMM > LongestCaught)
                {
                    LongestCaught = fish.LengthMM;
                }
                if (fish.Grams > HeaviestCaught)
                {
                    HeaviestCaught = fish.Grams;
                }
            }
            else
            {
                throw new ArgumentException("Fish of wrong type added to a record");
            }

        }
    }
}
