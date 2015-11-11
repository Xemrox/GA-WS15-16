﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XGA.Config;
using XGA.Helper;

namespace XGA.Folding {

    public class Folding : IFitnessMeasured<char> {

        public char[] BaseType {
            get; set;
        }

        public Folding() {
        }

        protected static readonly int[][][] OrientMapping = {
            /*X*/new int[][] {
                              /*U   L   F   R*/
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

        public Folding(Folding folding) {
            this.BaseType = new char[folding.BaseType.Length];
            Buffer.BlockCopy(folding.BaseType, 0, this.BaseType, 0, folding.BaseType.Length * sizeof(char));
        }

        protected struct printhelper {
            public bool isFirst { get; set; }
            public Direction dir { get; set; }
            public FoldType type { get; set; }
        }

        protected static Dictionary<char, Direction> cD = new Dictionary<char, Direction>();
        protected static Dictionary<char, FoldType> cT = new Dictionary<char, FoldType>();

        static Folding() {
            cD.Add('L', Direction.L);
            cD.Add('F', Direction.F);
            cD.Add('R', Direction.R);
            cD.Add('U', Direction.U);

            // 0 = hydrophil, "white"
            // 1 = hydrophob, "black"
            cT.Add('0', FoldType.Hydrophilic);
            cT.Add('1', FoldType.Hydrophobic);
        }

        public virtual double CalculateFitness(char[] reference) {
            int Orientation = 0;
            var Field = new Dictionary<Point, bool>();
            var currentPoint = new Point { X = 0, Y = 0 };

            double cNeighbour = 0;
            double cOverlapp = 0;

            int seqLength = this.BaseType.Length;
            if (seqLength != reference.Length - 1) {
                throw new Exception("Sequenzelengths do not match");
            }
            // Create Field
            for (int i = 0; i < reference.Length; i++) {
                Direction currentDirection;
                if (i == reference.Length - 1) {
                    currentDirection = Direction.U;
                } else {
                    currentDirection = cD[this.BaseType[i]];
                }
                var currentType = cT[reference[i]];

                var iDirection = (int) currentDirection;

                bool bUsedByHydrophobic = true;
                if (Field.TryGetValue(currentPoint, out bUsedByHydrophobic)) {
                    //overlapp
                    cOverlapp++;

                    //hydrophil wins
                    Field[currentPoint] = bUsedByHydrophobic && currentType == FoldType.Hydrophobic;
                } else {
                    Field[currentPoint] = currentType == FoldType.Hydrophobic;
                }
                if (currentType == FoldType.Hydrophobic) {
                    //check neighbours
                    List<Direction> dirs = currentDirection.GetNeighbours();
                    foreach (Direction dir in dirs) {
                        var neighbour = new Point();
                        neighbour.X = currentPoint.X + OrientMapping[0][Orientation][(int) dir];
                        neighbour.Y = currentPoint.Y + OrientMapping[1][Orientation][(int) dir];

                        bool bIsNeighbourHydrophobic = true;
                        if (Field.TryGetValue(neighbour, out bIsNeighbourHydrophobic)) {
                            if (bIsNeighbourHydrophobic) {
                                cNeighbour++;
                            }
                        }
                    }
                }

                currentPoint.X += OrientMapping[0][Orientation][iDirection];
                currentPoint.Y += OrientMapping[1][Orientation][iDirection];

                Orientation = ( ( iDirection - 2 ) + Orientation ).mod(4);
            }

            return Folding.ScaleFitness(cOverlapp, cNeighbour);
        }

        protected static double fBase = 1000.0d;
        protected static double NeighbourScale = 50.0d;

        public static double ScaleFitness(double Overlapps, double Neighbours) {
            if (Overlapps > 0) {
                return ( fBase / ( ( Overlapps + 1 ) * 3 ) ) + Neighbours;
            }
            return fBase + Neighbours * NeighbourScale;
        }

        public static double Neighbours(double Fitness) {
            if (Fitness < 1000) {
                return 0; //no approx solution here! its bad it has overlapps discard by all means!
            }
            return ( ( Fitness - fBase ) / NeighbourScale );
        }

        public Task<double> CalculateFitnessAsync(char[] reference) {
            return Task.Run(() => CalculateFitness(reference));
        }

        public void print(char[] reference, Logger log) {
            int Orientation = 0;
            var Field = new Dictionary<Point, List<printhelper>>();
            var currentPoint = new Point { X = 0, Y = 0 };

            var MaxP = new Point { X = 0, Y = 0 };
            var MinP = new Point { X = 0, Y = 0 };

            double cNeighbour = 0;
            double cOverlapp = 0;

            bool bFirst = true;

            int seqLength = this.BaseType.Length;
            if (seqLength != reference.Length - 1) {
                throw new Exception("Sequenzelengths do not match");
            }
            // Create Field
            for (int i = 0; i < reference.Length; i++) {
                Direction currentDirection;
                if (i == reference.Length - 1) {
                    currentDirection = Direction.U;
                } else {
                    currentDirection = cD[this.BaseType[i]];
                }
                var currentType = cT[reference[i]];

                var iDirection = (int) currentDirection;

                List<printhelper> elems;
                if (Field.TryGetValue(currentPoint, out elems)) {
                    //overlapp
                    cOverlapp++;

                    Field[currentPoint].Add(new printhelper { isFirst = false, dir = currentDirection, type = currentType });
                } else {
                    var p = new List<printhelper>();
                    p.Add(new printhelper { isFirst = bFirst, dir = currentDirection, type = currentType });
                    Field[currentPoint] = p;
                    if (bFirst) {
                        bFirst = false;
                    }
                }

                //check neighbours
                if (currentType == FoldType.Hydrophobic) {
                    List<Direction> dirs = currentDirection.GetNeighbours();
                    foreach (Direction dir in dirs) {
                        var neighbour = new Point();
                        neighbour.X = currentPoint.X + OrientMapping[0][Orientation][(int) dir];
                        neighbour.Y = currentPoint.Y + OrientMapping[1][Orientation][(int) dir];

                        if (Field.TryGetValue(neighbour, out elems)) {
                            cNeighbour += elems.Count(x => x.type == FoldType.Hydrophobic);
                        }
                    }
                }

                currentPoint.X += OrientMapping[0][Orientation][iDirection];
                currentPoint.Y += OrientMapping[1][Orientation][iDirection];

                MinP.X = Math.Min(currentPoint.X, MinP.X);
                MinP.Y = Math.Min(currentPoint.Y, MinP.Y);

                MaxP.X = Math.Max(currentPoint.X, MaxP.X);
                MaxP.Y = Math.Max(currentPoint.Y, MaxP.Y);

                Orientation = ( ( iDirection - 2 ) + Orientation ).mod(4);
            }

            var Fitness = Folding.ScaleFitness(cOverlapp, cNeighbour);

            log.Write("\n");

            var cP = new Point { X = 0, Y = 0 };
            for (int y = MaxP.Y + 1; y >= MinP.Y - 1; y--) {
                for (int x = MinP.X - 1; x <= MaxP.X + 1; x++) {
                    cP.X = x; cP.Y = y;

                    List<printhelper> elems;
                    if (Field.TryGetValue(cP, out elems)) {
                        var last = elems.Last();
                        if (elems.Count > 1) {
                            var msg = string.Format(" X{0}{1}", last.type.Print(), last.dir.ToString(true));
                            log.Write(msg);
                        } else if (last.isFirst) {
                            var msg = string.Format(" !{0}{1}", last.type.Print(), last.dir.ToString(true));
                            log.Write(msg);
                        } else {
                            var msg = string.Format("  {0}{1}", last.type.Print(), last.dir.ToString(true));
                            log.Write(msg);
                        }
                    } else {
                        log.Write("  - ");
                    }
                }
                log.Write(Environment.NewLine);
            }
            var result = string.Format("F: {0} N: {1} O: {2} S: {3}{4}", Fitness, cNeighbour, cOverlapp, new string(this.BaseType), Environment.NewLine);
            log.Write(result);
            log.Write(new string('-', ( ( -MinP.X ) + MaxP.X + 3 ) * 4) + Environment.NewLine);
        }

        public char[] GenerateRandom(int length) {
            char[] ALP = { 'L', 'F', 'R' };
            var rnd = new char[length];

            for (int l = 0; l < length; l++) {
                rnd[l] = ALP[RandomHelper.GetNextInteger(3)];
            }
            return rnd;
        }

        public object Clone() {
            return new Folding(this);
        }
    }
}