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

    }
}
