using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.GumRuntimes
{
    public partial class TrophyDisplayRuntime
    {
        partial void CustomInitialize()
        {
            
        }

        public void SetTrophyByPlaceNumber(int place)
        {
            switch (place)
            {
                case 1: CurrentTrophyPlaceState = TrophyDisplayRuntime.TrophyPlace.FirstPlace; break;
                case 2: CurrentTrophyPlaceState = TrophyDisplayRuntime.TrophyPlace.SecondPlace; break;
                case 3: CurrentTrophyPlaceState = TrophyDisplayRuntime.TrophyPlace.ThirdPlace; break;
                case 4: CurrentTrophyPlaceState = TrophyDisplayRuntime.TrophyPlace.FourthPlace; break;
                case 5: CurrentTrophyPlaceState = TrophyDisplayRuntime.TrophyPlace.FifthPlace; break;
                case 6: CurrentTrophyPlaceState = TrophyDisplayRuntime.TrophyPlace.SixthPlace; break;
                case 7: CurrentTrophyPlaceState = TrophyDisplayRuntime.TrophyPlace.SeventhPlace; break;
                case 8: CurrentTrophyPlaceState = TrophyDisplayRuntime.TrophyPlace.EighthPlace; break;
                default: CurrentTrophyPlaceState = TrophyDisplayRuntime.TrophyPlace.EighthPlace; break;
            }
        }
    }
}
