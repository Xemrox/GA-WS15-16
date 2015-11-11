using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XGA.Config;
using XGA.Helper;

namespace XGA.Folding {

    public class FoldingPlus : Folding {

        public override double CalculateFitness(char[] reference) {
            int Orientation = 0;
            var positions = new Point[reference.Length];

            //var Field = new Dictionary<Point, bool>();
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

                /*bool bUsedByHydrophobic = true;
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
                }*/

                positions[i] = currentPoint;

                currentPoint.X += OrientMapping[0][Orientation][iDirection];
                currentPoint.Y += OrientMapping[1][Orientation][iDirection];

                Orientation = ( ( iDirection - 2 ) + Orientation ).mod(4);
            }

            for (int leftI = 0; leftI < reference.Length; leftI++) {
                for (int rightI = reference.Length - 1; rightI > leftI; rightI--) {
                    var distance = positions[rightI].distance(positions[leftI]);
                    Console.WriteLine("{0},{1} -> {2},{3} {4},{5} D: {6}", leftI, rightI, positions[leftI].X, positions[leftI].Y, positions[rightI].X, positions[rightI].Y, distance);
                    if (distance == 0) {
                        //overlapp?
                        Console.WriteLine("Overlapp");
                        cOverlapp++;
                    } else if (distance == 1) {
                        Console.WriteLine("{0},{1} near {2},{3}", positions[leftI].X, positions[leftI].Y, positions[rightI].X, positions[rightI].Y);
                        //Console.WriteLine("Near!");
                    } else {
                        rightI -= ( distance - 2 );
                    }
                }
            }

            return Folding.ScaleFitness(cOverlapp, cNeighbour);
        }
    }
}