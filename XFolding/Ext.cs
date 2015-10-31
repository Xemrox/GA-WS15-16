using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Folding {
    public static class Ext {
        public static int mod(this int x, int m) {
            int r = x % m;
            return  r < 0 ? r + m : r;
        }

        public static HashSet<K> ToHashSet<K,IN>(this IEnumerable<IN> data, Func<IN,K> keySelector) {
            var ret = new HashSet<K>();
            foreach(var d in data) {
                ret.Add(keySelector.Invoke(d));
            }
            return ret;
        }
    }
}
