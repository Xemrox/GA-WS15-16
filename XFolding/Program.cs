using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Folding {

    public class Point {
        public int X { get; set; }
        public int Y { get; set; }

        public override int GetHashCode() {

            return ( X << 16 ) ^ Y;
        }
    }

    public class Program {

        public static readonly string SEQ20 = "10100110100101100101";
        public static readonly string SEQ24 = "110010010010010010010011";
        public static readonly string SEQ25 = "0010011000011000011000011";
        public static readonly string SEQ36 = "000110011000001111111001100001100100";
        public static readonly string SEQ48 = "001001100110000011111111110000001100110010011111";
        public static readonly string SEQ50 = "11010101011110100010001000010001000101111010101011";


        public static readonly string SEQ01 = "01011001011010011010";
        public static readonly string FOL01 = "FRFRRLLRFRRLRLLRRFR";
        public static readonly string FS01 = "0F1R0F1R1R0L0L1R0F1R1R0L1R0L0L1R1R0F1R0";

        public static readonly string SEQ02 = "1101101";
        public static readonly string FOL02 = "FFLLLF";

        private static string MergeSeqAndFold(string Seq, string Fold) {
            StringBuilder merge = new StringBuilder();

            for (int i = 0; i < Fold.Length; i++) {
                merge.Append(Seq[i]);
                merge.Append(Fold[i]);
            }
            merge.Append(Seq.Last());

            return merge.ToString();
        }

        public static void Main(string[] args) {
            /*NodeChain nc = SeqToNodes(FS01);

            Console.WriteLine(nc.ToString());
            Console.WriteLine(FS01);*/

            string chain = MergeSeqAndFold(SEQ01, FOL01);
            NodeChain nc = SeqToNodes(chain);

            Console.WriteLine(CalculateFitness(ref nc));

            chain = MergeSeqAndFold(SEQ02, FOL02);
            nc = SeqToNodes(chain);

            Console.WriteLine(CalculateFitness(ref nc));

            Console.ReadKey();
        }

        static int[][][] OrientMapping = { 
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

        public static double CalculateFitness(ref NodeChain nc) {
            int Orientation = 0;

            Dictionary<int, List<Node>> Field = new Dictionary<int, List<Node>>();

            Point lastPoint = new Point() { X = 0, Y = 0 };

            Point Min = new Point() { X = 0, Y = 0};
            Point Max = new Point() { X = 0, Y = 0 };

            // Create Field
            Node current = nc.First;

            while (current != null) {
                int iDirection = (int) current.Direction;

                Point cP = new Point();
                cP.X = lastPoint.X + OrientMapping[0][Orientation][iDirection];
                cP.Y = lastPoint.Y + OrientMapping[1][Orientation][iDirection];

                Orientation = ( ( iDirection - 2 ) + Orientation ).mod(4);

                List<Node> FieldElem;
                if(!Field.TryGetValue(lastPoint.GetHashCode(), out FieldElem)) {
                    Field[lastPoint.GetHashCode()] = new List<Node>();
                }

                Field[lastPoint.GetHashCode()].Add(current);

                lastPoint = cP;

                current = current.Next;
            }

            Orientation = 0;
            lastPoint = new Point() { X = 0, Y = 0 };
            current = nc.First;

            double cNeighbour = 0;
            double cOverlapp = 0;

            while (current != null) {
                //Overlapp check
                List<Node> currentNodes;
                if (Field.TryGetValue(lastPoint.GetHashCode(), out currentNodes)) {
                    cOverlapp += currentNodes.Count - 1;
                }

                //Neighbour check
                int iDirection = (int) current.Direction;

                List<Node.NodeDirection> dirs = current.GetNeighbours();
                foreach(Node.NodeDirection dir in dirs) {
                    Point neighbour = new Point();
                    neighbour.X = lastPoint.X + OrientMapping[0][Orientation][(int)dir];
                    neighbour.Y = lastPoint.Y + OrientMapping[1][Orientation][(int)dir];
                    List<Node> neighbours;
                    if (Field.TryGetValue(neighbour.GetHashCode(), out neighbours)) {
                        ///? cOverlapp += n.Count - 1;
                        /*if (n.Type == Node.NodeType.Hydrophobic)
                            cNeighbour++;*/
                        foreach(var n in neighbours) {
                            if (n.Type == Node.NodeType.Hydrophobic)
                                cNeighbour++;
                        }
                    }
                }

                if(current == nc.First) {
                    Point neighbour = new Point();
                    //Navigate Backwards for the first node
                    neighbour.X = lastPoint.X - OrientMapping[0][Orientation][(int) Node.NodeDirection.F];
                    neighbour.Y = lastPoint.Y - OrientMapping[1][Orientation][(int) Node.NodeDirection.F];

                    List<Node> neighbours;
                    if (Field.TryGetValue(neighbour.GetHashCode(), out neighbours)) {
                        ///? cOverlapp += n.Count - 1;
                        /*if (n.Type == Node.NodeType.Hydrophobic)
                            cNeighbour++;*/
                        foreach (var n in neighbours) {
                            if (n.Type == Node.NodeType.Hydrophobic)
                                cNeighbour++;
                        }
                    }
                }

                Point cP = new Point();
                cP.X = lastPoint.X + OrientMapping[0][Orientation][iDirection];
                cP.Y = lastPoint.Y + OrientMapping[1][Orientation][iDirection];

                //Find Min/Max Corners
                Min.X = Math.Min(cP.X, Min.X);
                Min.Y = Math.Min(cP.Y, Min.Y);

                Max.X = Math.Max(cP.X, Max.X);
                Max.Y = Math.Max(cP.Y, Max.Y);

                Orientation = ( ( iDirection - 2 ) + Orientation ).mod(4);

                lastPoint = cP;
                current = current.Next;
            }

            for (int y = Max.Y + 1; y >= Min.Y - 1; y--) {
                for (int x = Min.X - 1; x <= Max.X + 1; x++) {
                    Point p = new Point() {
                        X = x,
                        Y = y
                    };

                    List<Node> n;
                    if (Field.TryGetValue(p.GetHashCode(), out n)) {
                        Console.Write(" " + n.Last().ToString(true));
                    } else {
                        Console.Write(" - ");
                    }
                }
                Console.WriteLine();
            }

            Console.WriteLine(string.Format("N: {0}", cNeighbour));
            Console.WriteLine(string.Format("O: {0}", cOverlapp));

            return cNeighbour / 2.0 / Math.Max(cOverlapp, 1);
        }

        /// <summary>
        /// returns the First Node
        /// </summary>
        /// <param name="seq"></param>
        /// <returns></returns>
        private static NodeChain SeqToNodes(string seq) {
            NodeChain nc = new NodeChain();

            Node last = null;

            for (int i = 0; i < ( seq.Length - 1 ); i += 2) {
                Node n = new Node() {
                    Type = (Node.NodeType) Enum.Parse(typeof(Node.NodeType), seq[i].ToString()),
                    Direction = (Node.NodeDirection) Enum.Parse(typeof(Node.NodeDirection), seq[i + 1].ToString()),
                    Previous = last
                };

                if (nc.First == null) {
                    nc.First = n;
                }
                if (last != null) {
                    last.Next = n;
                }

                last = n;
            }


            Node final = new Node() {
                Type = (Node.NodeType) Enum.Parse(typeof(Node.NodeType), seq.Last().ToString()),
                Previous = last
            };
            last.Next = final;
            nc.Last = final;

            return nc;
        }
    }

    public class NodeChain {
        public Node First { get; set; }
        public Node Last { get; set; }
        public override string ToString() {
            StringBuilder sb = new StringBuilder();

            Node current = this.First;

            while (current != null) {
                sb.Append(current.ToString());
                current = current.Next;
            }

            return sb.ToString();
        }
    }
    public class Node {
        public enum NodeDirection {
            U = 0,
            L,
            F,
            R,
        }

        public enum NodeType {
            Hydrophobic,
            Hydrophilic
        }

        /// <summary>
        /// Align of the Next Node
        /// </summary>
        public NodeDirection Direction {
            get; set;
        }

        /// <summary>
        /// Previous Node in the Chain
        /// </summary>
        public Node Previous {
            get; set;
        }

        /// <summary>
        /// Next Node in the Chain
        /// </summary>
        public Node Next {
            get; set;
        }

        public NodeType Type {
            get; set;
        }

        private static Dictionary<NodeDirection, List<NodeDirection>> NLists; 

        static Node() {
            NLists = new Dictionary<NodeDirection, List<NodeDirection>>();
            NLists.Add(NodeDirection.F, new List<NodeDirection>() { NodeDirection.L, NodeDirection.R });
            NLists.Add(NodeDirection.L, new List<NodeDirection>() { NodeDirection.F, NodeDirection.R });
            NLists.Add(NodeDirection.R, new List<NodeDirection>() { NodeDirection.F, NodeDirection.L });
            NLists.Add(NodeDirection.U, new List<NodeDirection>() { NodeDirection.L, NodeDirection.F, NodeDirection.R });
        }

        public List<NodeDirection> GetNeighbours() {
            return NLists[this.Direction];
        }

        public string ToString(bool ShowUnknown = false) {
            string Dir = this.Direction.ToString();
            return ( (int) this.Type ).ToString() + (!ShowUnknown ? ( Dir == "U" ? "" : Dir ) : Dir);
        }
        public override string ToString() {
            return this.ToString(false);
        }

    }
}