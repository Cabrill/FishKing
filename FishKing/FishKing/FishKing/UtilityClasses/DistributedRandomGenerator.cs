using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.UtilityClasses
{
    class DistributedRandomGenerator
    {
        private Dictionary<object, double> distribution = new Dictionary<object, double>();
        private double distSum;

        public void AddValue(object value, double dist)
        {
            if (distribution.ContainsKey(value))
            {
                distSum -= distribution[value];
            }
            distribution[value] = dist;
            distSum += dist;
        }

        public object GetDistributedRandomValue(double rand)
        {
            double ratio = 1.0f / distSum;
            double tempDist = 0;
            foreach (KeyValuePair<object, double> kvp in distribution)
            {
                tempDist += distribution[kvp.Key];
                if (rand / ratio <= tempDist)
                {
                    return kvp.Key;
                }
            }
            return 0;
        }

    }
}
