using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.DataTypes
{
    partial class Tournaments
    {
        public IEnumerable<Fish_Types> RequiredFish
        {
            get
            {
                foreach (var fishtype in FishReq.SelectMany(fr => fr.Split(',')))
                {
                    yield return GlobalContent.Fish_Types[fishtype.Trim()];
                }
            }
        }
    }
}
