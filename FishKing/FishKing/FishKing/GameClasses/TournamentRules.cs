using FishKing.Entities;
using FishKing.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.GameClasses
{
    public class TournamentRules
    {
        public List<Fish> FishTypesAllowed
        {
            get; private set;
        }
        public string FishTypesAllowedString
        {
            get
            {
                if (FishTypesAllowed.Count == 0)
                {
                    return "All fish";
                }
                else
                {
                    var fishNames = FishTypesAllowed.Select(f => f.Name);
                    return string.Join(",", fishNames);
                }
            }
        }

        public int MinimumWeight
        {
            get; private set;
        }
        public string MinimumWeightString
        {
            get
            {
                return InfoToString.Weight(MinimumWeight);
            }
        }

        public int MinimumLength
        {
            get; private set;
        }
        public string MinimumLengthString
        {
            get
            {
                return InfoToString.Length(MinimumLength);
            }
        }

        public TournamentRules(List<Fish> fishType = null, int minWeight = 0, int minLength = 0)
        {
            if (fishType == null)
            {
                fishType = new List<Fish>();
            }
            FishTypesAllowed = fishType;
            MinimumWeight = minWeight;
            MinimumLength = minLength;
        }

        public override string ToString()
        {
            var returnString = "";
            if (FishTypesAllowed.Count == 0)
            {
                if (MinimumWeight == 0 && MinimumLength == 0)
                {
                    returnString = FishTypesAllowedString;
                }
                else if (MinimumLength > 0 && MinimumWeight > 0)
                {
                    returnString = string.Format("Measuring: {0}{1}Weighing: {2}",
                        MinimumLengthString, System.Environment.NewLine, MinimumWeightString);
                }
                else if (MinimumWeight > 0)
                {
                    returnString = string.Format("Weighing: {0}", MinimumWeightString);
                }
                else if (MinimumLength > 0)
                {
                    returnString = string.Format("Measuring: {0}", MinimumLengthString);
                }
            }
            else
            {
                if (MinimumWeight == 0 && MinimumLength == 0)
                {
                    returnString = FishTypesAllowedString;
                }
                else if (MinimumLength > 0)
                {
                    returnString = string.Format("{0}{1}Weighing: {2}",
                        FishTypesAllowedString, System.Environment.NewLine, MinimumWeightString);
                }
                else if (MinimumWeight > 0)
                {
                    returnString = string.Format("{0}{1}Measuring: {2}",
                        FishTypesAllowedString, System.Environment.NewLine, MinimumWeightString);
                }
            }

            return returnString;
        }
    }
}
