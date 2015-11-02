using System;
using System.Collections.Generic;
using System.IO;
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
            get; set;
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

        public Folding(Folding folding) {
            this.foldSeq = string.Copy(folding.foldSeq);
        }

        public static List<Direction> GetNeighbours(Direction from) {
            return NeighbourMap[from];
        }

        private struct printhelper {
            public Direction dir { get; set; }
            public PType type { get; set; }
        }

        public void print(string seq, StreamWriter Output) {
            int Orientation = 0;
            var Field = new Dictionary<Point, List<printhelper>>();
            var lastPoint = new Point { X = 0, Y = 0 };

            double cNeighbour = 0;
            double cOverlapp = 0;

            int seqLength = seq.Length;
            var foldSeq = this.foldSeq + "U";
            if (seqLength != foldSeq.Length) {
                throw new Exception("Sequenzelengths do not match");
            }

            var MinP = new Point { X = 0, Y = 0 };
            var MaxP = new Point { X = 0, Y = 0 };

            // Create Field
            for (int i = 0; i < seqLength; i++) {
                var currentDirection = (Direction) Enum.Parse(typeof(Direction), foldSeq[i].ToString());
                var currentType = (PType) Enum.Parse(typeof(PType), seq[i].ToString());

                var iDirection = (int) currentDirection;

                var cP = new Point();
                cP.X = lastPoint.X + OrientMapping[0][Orientation][iDirection];
                cP.Y = lastPoint.Y + OrientMapping[1][Orientation][iDirection];

                MinP.X = Math.Min(MinP.X, cP.X);
                MinP.Y = Math.Min(MinP.Y, cP.Y);

                MaxP.X = Math.Max(MaxP.X, cP.X);
                MaxP.Y = Math.Max(MaxP.Y, cP.Y);

                List<printhelper> elems;
                if (!Field.TryGetValue(lastPoint, out elems)) {
                    Field[lastPoint] = new List<printhelper>();
                } else {
                    cOverlapp++;
                }
                Field[lastPoint].Add(new printhelper { type = currentType, dir = currentDirection });

                //check neighbours
                List<Direction> dirs = Folding.GetNeighbours(currentDirection);
                foreach (Direction dir in dirs) {
                    var neighbour = new Point();
                    neighbour.X = lastPoint.X + OrientMapping[0][Orientation][(int) dir];
                    neighbour.Y = lastPoint.Y + OrientMapping[1][Orientation][(int) dir];
                    //List<Node> neighbours;
                    //List<printhelper> elems;
                    if (Field.TryGetValue(neighbour, out elems)) {
                        if (elems.Last().type == PType.Hydrophobic && currentType == PType.Hydrophobic) {
                            cNeighbour++;
                        }
                    }
                }

                Orientation = ( ( iDirection - 2 ) + Orientation ).mod(4);
                lastPoint = cP;
            }

            /*{
                var currentDirection = Direction.U; //Unknown e.g. LastElem
                var currentType = (PType) Enum.Parse(typeof(PType), seq.Last().ToString());

                var iDirection = (int) currentDirection;

                var cP = new Point();
                cP.X = lastPoint.X + OrientMapping[0][Orientation][iDirection];
                cP.Y = lastPoint.Y + OrientMapping[1][Orientation][iDirection];

                List<printhelper> elems;
                if (!Field.TryGetValue(lastPoint, out elems)) {
                    Field[lastPoint] = new List<printhelper>();
                } else {
                    cOverlapp++;
                }
                Field[lastPoint].Add(new printhelper { type = currentType, dir = currentDirection });

                //check neighbours
                List<Direction> dirs = Folding.GetNeighbours(currentDirection);
                foreach (Direction dir in dirs) {
                    var neighbour = new Point();
                    neighbour.X = lastPoint.X + OrientMapping[0][Orientation][(int) dir];
                    neighbour.Y = lastPoint.Y + OrientMapping[1][Orientation][(int) dir];
                    //List<Node> neighbours;
                    //List<printhelper> elems;
                    if (Field.TryGetValue(neighbour, out elems)) {
                        if (elems.Last().type == PType.Hydrophobic && currentType == PType.Hydrophobic) {
                            cNeighbour++;
                        }
                    }
                }
            }*/

            for (int y = MaxP.Y + 1; y >= MinP.Y - 1; y--) {
                for (int x = MinP.X - 1; x <= MaxP.X + 1; x++) {
                    var cP = new Point { X = x, Y = y };

                    List<printhelper> elems;
                    if (Field.TryGetValue(cP, out elems)) {
                        var last = elems.Last();
                        if (elems.Count > 1) {
                            Output.Write(" X{0}{1}", last.type.Print(), last.dir.ToString(true));
                            var fitness = this.CalculateFitness(seq);
                        } else {
                            Output.Write("  {0}{1}", last.type.Print(), last.dir.ToString(true));
                        }
                    } else {
                        Output.Write("  - ");
                    }
                }
                Output.WriteLine();
            }
            var dBase = 1000.0;
            var Fitness = 0.0;
            if (cOverlapp > 0) {
                Fitness = ( dBase / ( ( cOverlapp + 1 ) * 3 ) ) + cNeighbour;
            } else {
                Fitness = dBase + cNeighbour * 100;
            }
            Output.WriteLine("N: {0} O: {1} F: {2} S: {3}", cNeighbour, cOverlapp, Fitness, foldSeq);
        }

        public double CalculateFitness(string seq) {
            int Orientation = 0;

            var Field = new Dictionary<Point, bool>();

            var lastPoint = new Point { X = 0, Y = 0 };

            double cNeighbour = 0;
            double cOverlapp = 0;

            int seqLength = seq.Length;
            var foldSeq = this.foldSeq + "U";
            if (seqLength != foldSeq.Length) {
                throw new Exception("Sequenzelengths do not match");
            }
            // Create Field
            for (int i = 0; i < seqLength; i++) {
                var currentDirection = (Direction) Enum.Parse(typeof(Direction), foldSeq[i].ToString());
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
                        if (bIsNeighbourHydrophobic && currentType == PType.Hydrophobic) {
                            cNeighbour++;
                        }
                    }
                }

                Orientation = ( ( iDirection - 2 ) + Orientation ).mod(4);
                lastPoint = cP;
            }
            //check last elem
            /*{
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
                        if (bIsNeighbourHydrophobic && currentType == PType.Hydrophobic) {
                            cNeighbour++;
                        }
                    }
                }
            }*/
            //Console.WriteLine(string.Format("N: {0}", cNeighbour));
            //Console.WriteLine(string.Format("O: {0}", cOverlapp));

            var dBase = 1000.0;
            if (cOverlapp > 0) {
                return ( dBase / ( ( cOverlapp + 1 ) * 3 ) ) + cNeighbour;
            }
            return dBase + cNeighbour * 100;
            //return cNeighbour / ( cOverlapp + 1.0 );
        }
    }
}