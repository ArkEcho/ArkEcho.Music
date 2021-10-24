using System;
using System.Collections.Generic;
using System.Linq;

namespace ArkEcho.Core
{
    public class RandomShuffle
    {
        private class ShuffleClass
        {
            public Guid GuidRND { get; } = new Guid();
            public object Object { get; set; } = null;
        }

        public static List<T> GetShuffledList<T>(List<T> ToShuffle)
        {
            List<ShuffleClass> Shuffle = new List<ShuffleClass>();

            foreach (object obj in ToShuffle)
                Shuffle.Add(new ShuffleClass() { Object = obj });

            Shuffle = Shuffle.OrderBy(x => x.GuidRND).ToList();

            List<T> list = new List<T>();
            foreach (ShuffleClass clas in Shuffle)
                list.Add((T)clas.Object);
            return list;
        }
    }
}
