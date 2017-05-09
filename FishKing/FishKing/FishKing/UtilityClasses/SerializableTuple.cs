using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.UtilityClasses
{
    public class SerializableTuple<T1, T2>
    {
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }

        public SerializableTuple()
        {

        }
        public SerializableTuple(T1 value1, T2 value2)
        {
            Item1 = value1;
            Item2 = value2;
        }

        public static SerializableTuple<T1, T2> Create(T1 value1, T2 value2)
        {
            return new SerializableTuple<T1, T2>(value1, value2);
        }
    }
}
