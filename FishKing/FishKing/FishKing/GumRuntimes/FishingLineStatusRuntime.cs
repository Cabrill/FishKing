using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.GumRuntimes
{
    partial class FishingLineStatusRuntime
    {
        public int LineStress
        {
            get; set;
        }

        public void Update()
        {
            FishingLineStressHeight = LineStress;
        }

        public void Reset()
        {
            LineStress = 0;
        }
    }
}
