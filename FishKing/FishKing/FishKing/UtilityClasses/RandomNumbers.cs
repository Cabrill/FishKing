using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.UtilityClasses
{
    static class RandomNumbers
    {
        private static Random randomSeed = new Random(DateTime.Now.Millisecond);

        public static Random Random
        {
            get { return randomSeed; }
        }
    }
}
