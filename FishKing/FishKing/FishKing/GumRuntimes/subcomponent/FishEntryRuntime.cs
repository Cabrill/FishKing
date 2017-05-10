using FishKing.DataTypes;
using FishKing.GameClasses;
using FishKing.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.GumRuntimes
{
    partial class FishEntryRuntime
    {
        public void AssociateWithFish(Fish_Types fishType, FishRecord record = null)
        {
            SetTexture(fishType);
            if (record == null)
            {
                CurrentPreviouslyCaughtState = PreviouslyCaught.NotCaught;
            }
            else
            {
                FishNameValue.Text = fishType.Name;
                TimesCaughtValue.Text = record.TimesCaught.ToString();
                LongestValue.Text = InfoToString.Length(record.LongestCaught);
                HeaviestValue.Text = InfoToString.Weight(record.HeaviestCaught);

                var placesCaught = InfoToString.PlacesCaught(fishType);
                FoundInValue.Text = placesCaught;
                var numPlacesCaught = placesCaught.Count(c => c == ',')+1;
                
                if (numPlacesCaught == 1 )
                {
                    CurrentFoundItemsState = FoundItems.Item1;
                }
                else if (numPlacesCaught == 2)
                {
                    CurrentFoundItemsState = FoundItems.Item2;
                }
                else if (numPlacesCaught == 3)
                {
                    CurrentFoundItemsState = FoundItems.Item3;
                }
                else
                {
                    CurrentFoundItemsState = FoundItems.Item4;
                }
            }
        }

        private void SetTexture(Fish_Types fishType)
        {
            var textureRow = fishType.Row;
            var textureCol = fishType.Col;
            var textureHeight = 64;
            var textureWidth = 128;

            FishSprite.TextureTop = textureRow * textureHeight;
            FishSprite.TextureLeft = textureCol * textureWidth;
        }
    }
}
