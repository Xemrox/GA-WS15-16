using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Folding {

    public enum Direction {
        U = 0,
        L,
        F,
        R
    }

    public static class DirectionExt {
        public static string ToString(this Direction elem, bool ShowUnknown = false) {
            string Dir = elem.ToString();
            return ( !ShowUnknown ? ( Dir == "U" ? "" : Dir ) : Dir );
        }
    }
}