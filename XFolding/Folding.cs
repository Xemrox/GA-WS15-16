using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Folding {

    public class Folding {

        private struct Point {
            public int X { get; set; }
            public int Y { get; set; }
        }

        public string foldSeq {
            get; private set;
        }

        public Folding() {
        }

        public Folding(string foldSeq) {
            this.foldSeq = foldSeq;
        }

        private static readonly int[][][] OrientMapping = {
            /*X*/new int[][] {
                                /*U L F  R*/
                     new int[] {0, -1,  0,  1},    // Top
                     new int[] {0,  0,  1,  0},    // Right
                     new int[] {0,  1,  0, -1},    // Bottom
                     new int[] {0,  0, -1,  0}     // Left
            },
            /*Y*/new int[][] {
                     new int[] {0,  0,  1,  0},
                     new int[] {0,  1,  0, -1},
                     new int[] {0,  0, -1,  0},
                     new int[] {0, -1,  0,  1}
        } };

        private static Dictionary<Direction, List<Direction>> NeighbourMap = new Dictionary<Direction, List<Direction>>();

        static Folding() {
            NeighbourMap.Add(Direction.F, new List<Direction> { Direction.L, Direction.R });
            NeighbourMap.Add(Direction.L, new List<Direction> { Direction.F, Direction.R });
            NeighbourMap.Add(Direction.R, new List<Direction> { Direction.F, Direction.L });
            NeighbourMap.Add(Direction.U, new List<Direction> { Direction.L, Direction.F, Direction.R });
        }

        public static List<Direction> GetNeighbours(Direction from) {
            return NeighbourMap[from];
        }

        public double CalculateFitness(string seq) {
            int Orientation = 0;

            var Field = new Dictionary<Point, bool>();

            var lastPoint = new Point { X = 0, Y = 0 };

            var Min = new Point { X = 0, Y = 0 };
            var Max = new Point { X = 0, Y = 0 };

            double cNeighbour = 0;
            double cOverlapp = 0;

            int seqLength = seq.Length - 1;
            if (seqLength != this.foldSeq.Length) {
                throw new Exception("Sequenzelengths do not match");
            }
            // Create Field
            for (int i = 0; i < seqLength; i++) {
                var currentDirection = (Direction) Enum.Parse(typeof(Direction), this.foldSeq[i].ToString());
                var currentType = (PType) Enum.Parse(typeof(PType), seq[i].ToString());

                var iDirection = (int) currentDirection;

                var cP = new Point();
                cP.X = lastPoint.X + OrientMapping[0][Orientation][iDirection];
                cP.Y = lastPoint.Y + OrientMapping[1][Orientation][iDirection];

                bool bUsedByHydrophobic = true;
                if (Field.TryGetValue(lastPoint, out bUsedByHydrophobic)) {
                    //overlapp
                    cOverlapp++;

                    //hydrophil wins
                    Field[lastPoint] = bUsedByHydrophobic && currentType == PType.Hydrophobic;
                } else {
                    Field[lastPoint] = currentType == PType.Hydrophobic;
                }

                //check neighbours
                List<Direction> dirs = Folding.GetNeighbours(currentDirection);
                foreach (Direction dir in dirs) {
                    var neighbour = new Point();
                    neighbour.X = lastPoint.X + OrientMapping[0][Orientation][(int) dir];
                    neighbour.Y = lastPoint.Y + OrientMapping[1][Orientation][(int) dir];
                    //List<Node> neighbours;
                    bool bIsNeighbourHydrophobic = true;
                    if (Field.TryGetValue(neighbour, out bIsNeighbourHydrophobic)) {
                        if (bIsNeighbourHydrophobic) {
                            cNeighbour++;
                        }
                    }
                }

                Orientation = ( ( iDirection - 2 ) + Orientation ).mod(4);
                lastPoint = cP;
            }
            //check last elem
            {
                var currentDirection = Direction.U; //Unknown e.g. LastElem
                var currentType = (PType) Enum.Parse(typeof(PType), seq.Last().ToString());

                var iDirection = (int) currentDirection;

                var cP = new Point();
                cP.X = lastPoint.X + OrientMapping[0][Orientation][iDirection];
                cP.Y = lastPoint.Y + OrientMapping[1][Orientation][iDirection];

                bool bUsedByHydrophobic = true;
                if (Field.TryGetValue(lastPoint, out bUsedByHydrophobic)) {
                    //overlapp
                    cOverlapp++;
                }
                //hydrophil wins
                Field[lastPoint] = bUsedByHydrophobic && currentType == PType.Hydrophobic;

                //check neighbours
                List<Direction> dirs = Folding.GetNeighbours(currentDirection);
                foreach (Direction dir in dirs) {
                    var neighbour = new Point();
                    neighbour.X = lastPoint.X + OrientMapping[0][Orientation][(int) dir];
                    neighbour.Y = lastPoint.Y + OrientMapping[1][Orientation][(int) dir];
                    //List<Node> neighbours;
                    bool bIsNeighbourHydrophobic = true;
                    if (Field.TryGetValue(neighbour, out bIsNeighbourHydrophobic)) {
                        if (bIsNeighbourHydrophobic) {
                            cNeighbour++;
                        }
                    }
                }
            }
            Console.WriteLine(string.Format("N: {0}", cNeighbour));
            Console.WriteLine(string.Format("O: {0}", cOverlapp));

            return cNeighbour / ( cOverlapp + 1.0 );
        }
    }
}