using FishKing.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.Extensions
{
    public static class ListExtensions
    {
        public static T RandomElement<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.RandomElementUsing<T>();
        }

        public static T RandomElementUsing<T>(this IEnumerable<T> enumerable)
        {
            int index = RandomNumbers.Random.Next(0, enumerable.Count());
            return enumerable.ElementAt(index);
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T element in source)
            {
                action(element);
            }
        }
    }
}
