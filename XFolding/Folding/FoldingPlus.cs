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
            var types = new FoldType[reference.Length];

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
                var currentType = types[i] = cT[reference[i]];

                var iDirection = (int) currentDirection;

                positions[i] = currentPoint;

                currentPoint.X += OrientMapping[0][Orientation][iDirection];
                currentPoint.Y += OrientMapping[1][Orientation][iDirection];

                Orientation = ( ( iDirection - 2 ) + Orientation ).mod(4);
            }

            int refL = reference.Length - 1;

            for (int leftI = 0; leftI < refL; leftI++) {
                for (int rightI = reference.Length - 1; rightI > leftI; rightI--) {
                    var posRight = positions[rightI];
                    var posLeft = positions[leftI];
                    var distance = posRight.distance(positions[leftI]);
                    //Console.WriteLine("{0},{1} -> {2},{3} {4},{5} D: {6}", leftI, rightI, positions[leftI].X, positions[leftI].Y, positions[rightI].X, positions[rightI].Y, distance);
                    if (distance == 0) {
                        //overlapp
                        //Console.WriteLine("Overlapp");
                        cOverlapp++;
                    } else if (distance == 1) {
                        if (types[leftI] == FoldType.Hydrophobic && types[rightI] == FoldType.Hydrophobic) {
                            if (positions[leftI + 1].distance(posRight) != 0) { //nextPoint is not the next Point in chain
                                //Console.WriteLine("{0},{1},{2}", nextX, nextY, nextPos.distance(posRight));
                                //Console.WriteLine("{0},{1} near {2},{3}", positions[leftI].X, positions[leftI].Y, positions[rightI].X, positions[rightI].Y);
                                cNeighbour++;
                            }
                        }
                    } else {
                        rightI -= ( distance - 2 );
                    }
                }
            }

            return Folding.ScaleFitness(cOverlapp, cNeighbour);
        }
    }
}