using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BananaPattern.Extensions
{
    static class IListExtensions
    {
        public static int FindFirst<T>(this IList<T> list, Predicate<T> predicate)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (predicate(list[i]))
                    return i;
            }

            throw new InvalidOperationException("No item matching the predicate was found.");
        }

        public static int FindLast<T>(this IList<T> list, Predicate<T> predicate)
        {
            int last = list.Count - 1;
            for (int i = last; i >= 0; i--)
            {
                if (predicate(list[i]))
                    return i;
            }

            throw new InvalidOperationException("No item matching the predicate was found.");
        }
    }
}
