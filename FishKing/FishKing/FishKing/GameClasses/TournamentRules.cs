using FishKing.DataTypes;
using FishKing.Entities;
using FishKing.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FishKing.GameClasses
{
    public class TournamentRules
    {
        [XmlArray("FishTypesAllowed")]
        [XmlArrayItem("FishType")]
        public List<Fish_Types> FishTypesAllowed
        {
            get; set;
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
            get; set;
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
            get; set;
        }
        public string MinimumLengthString
        {
            get
            {
                return InfoToString.Length(MinimumLength);
            }
        }

        internal bool DoesFishMeetRequirements(Fish fish)
        {
            return IsFishHeavyEnough(fish) && IsFishLongEnough(fish) && IsFishRightType(fish);
        }

        public bool IsFishHeavyEnough(Fish fish)
        {
            return fish.Grams >= MinimumWeight;
        }

        public bool IsFishLongEnough(Fish fish)
        {
            return fish.LengthMM >= MinimumLength;
        }

        public bool IsFishRightType(Fish fish)
        {
            if (!FishTypesAllowed.Any())
            {
                return true;
            }
            else
            {
                return FishTypesAllowed.Contains(fish.FishType);
            }
        }

        public TournamentRules()
        {
            FishTypesAllowed = new List<Fish_Types>();
            MinimumWeight = 0;
            MinimumLength = 0;
        }
        public TournamentRules(List<Fish_Types> fishType = null, int minWeight = 0, int minLength = 0)
        {
            if (fishType == null)
            {
                fishType = new List<Fish_Types>();
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
                    returnString = string.Format("At least {0}{1}At least{2}",
                        MinimumLengthString, System.Environment.NewLine, MinimumWeightString);
                }
                else if (MinimumWeight > 0)
                {
                    returnString = string.Format("At least {0}", MinimumWeightString);
                }
                else if (MinimumLength > 0)
                {
                    returnString = string.Format("At least {0}", MinimumLengthString);
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
