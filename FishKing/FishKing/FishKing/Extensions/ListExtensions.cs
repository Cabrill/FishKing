﻿using FishKing.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.Extensions
{
    static class ListExtensions
    {
        static T RandomElement<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.RandomElementUsing<T>();
        }

        static T RandomElementUsing<T>(this IEnumerable<T> enumerable)
        {
            int index = RandomNumbers.Random.Next(0, enumerable.Count());
            return enumerable.ElementAt(index);
        }
    }
}
