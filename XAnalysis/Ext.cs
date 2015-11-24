using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XAnalysis {

    public static class Ext {

        public static int Median(this IEnumerable<int> source) {
            var sC = source.Count();
            if (sC == 0) {
                throw new InvalidOperationException("Cannot compute median for an empty set.");
            }

            var sortedList = from number in source
                             orderby number
                             select number;

            int itemIndex = (int) sC / 2;

            if (sC % 2 == 0) {
                // Even number of items.
                return ( sortedList.ElementAt(itemIndex) + sortedList.ElementAt(itemIndex - 1) ) / 2;
            } else {
                // Odd number of items.
                return sortedList.ElementAt(itemIndex);
            }
        }

        public static string Repeat(this string s, int n) {
            return new String(Enumerable.Range(0, n).SelectMany(x => s).ToArray());
        }
    }
}