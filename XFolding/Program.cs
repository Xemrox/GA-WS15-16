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

            NodeChain nc = SeqToNodes(FS01);

            Console.WriteLine(nc.ToString());
            Console.WriteLine(FS01);

            Console.WriteLine(CalculateEnergy(SEQ01, FOL01));

            Console.ReadKey();
        }

        static int[][][] OrientMapping = { 
            /*X*/new int[][] {
                     new int[] {-1,  0,  1},
                     new int[] { 0,  1,  0},
                     new int[] { 1,  0, -1},
                     new int[] { 0, -1,  0}
            }, 
            /*Y*/new int[][] {
                     new int[] { 0,  1,  0},
                     new int[] { 1,  0, -1},
                     new int[] { 0, -1,  0},
                     new int[] {-1,  0,  1}
                 } };

        public static double CalculateEnergy(string Seq, string Fold) {
            int Orientation = 0;

            Dictionary<int, Node> Field = new Dictionary<int, Node>();

            Point lastPoint = new Point() { X = 0, Y = 0 };

            for (int i = 0; i < Fold.Length; i++) {

                Node.NodeDirection Direction = (Node.NodeDirection) Enum.Parse(typeof(Node.NodeDirection), Fold[i].ToString());

                int iDirection = (int) Direction - 1;

                Point cP = new Point();
                cP.X = lastPoint.X + OrientMapping[0][Orientation][iDirection];
                cP.Y = lastPoint.Y + OrientMapping[1][Orientation][iDirection];

                Orientation += ( iDirection - 1 );

                if (Orientation < 0) Orientation = 3;
                if (Orientation > 3) Orientation = 0;

                Field[lastPoint.GetHashCode()] = new Node() {
                    Type = (Node.NodeType) Enum.Parse(typeof(Node.NodeType), Seq[i].ToString()),
                    Direction = Node.NodeDirection.U
                };

                lastPoint = cP;

                //convert
                //Node.NodeType Type = (Node.NodeType)Enum.Parse(typeof(Node.NodeType), Seq[i].ToString());
            }

            Field[lastPoint.GetHashCode()] = new Node() {
                Type = (Node.NodeType) Enum.Parse(typeof(Node.NodeType), Seq.Last().ToString()),
                Direction = Node.NodeDirection.U
            };


            for (int y = 2; y > -5; y--) {
                for (int x = -3; x <= 3; x++) {
                    Point p = new Point() {
                        X = x,
                        Y = y
                    };

                    Node n;
                    if (Field.TryGetValue(p.GetHashCode(), out n)) {
                        Console.Write(" " + n.ToString());
                    } else {
                        Console.Write(" x");
                    }

                    /*if(Field.ContainsKey(p)) {
                        Node n = Field[p];
                        Console.Write(" "+n.ToString());
                    } else {
                        Console.Write(" x");
                    }*/

                }
                Console.WriteLine();
            }


            return 0;
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

        public override string ToString() {
            string Dir = this.Direction.ToString();
            return ( (int) this.Type ).ToString() + ( Dir == "U" ? "" : Dir );
        }

    }
}
