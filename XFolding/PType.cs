using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Folding {

    public enum PType {
        Hydrophobic,
        Hydrophilic
    }

    public static class PTypeExt {
        public static string Print(this PType elem) {
            return elem == PType.Hydrophilic ? "1": "0";
        }
    }
}