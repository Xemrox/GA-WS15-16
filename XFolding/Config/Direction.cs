using System.Collections.Generic;

namespace XGA {

    public enum Direction {
        U = 0,
        L,
        F,
        R
    }

    public static class DirectionExt {
        private static readonly Dictionary<Direction, List<Direction>> NeighbourMap = new Dictionary<Direction, List<Direction>>();

        static DirectionExt() {
            NeighbourMap.Add(Direction.F, new List<Direction> { Direction.L, Direction.R });
            NeighbourMap.Add(Direction.L, new List<Direction> { Direction.F, Direction.R });
            NeighbourMap.Add(Direction.R, new List<Direction> { Direction.F, Direction.L });
            NeighbourMap.Add(Direction.U, new List<Direction> { Direction.L, Direction.F, Direction.R });
        }

        public static string ToString(this Direction elem, bool ShowUnknown = false) {
            string Dir = elem.ToString();
            return ( !ShowUnknown ? ( Dir == "U" ? "" : Dir ) : Dir );
        }

        public static List<Direction> GetNeighbours(this Direction from) {
            return NeighbourMap[from];
        }
    }
}