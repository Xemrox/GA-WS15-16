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

        public static readonly string SEQ02 = "0001101";
        public static readonly string FOL02 = "FFLLLF";

        public static readonly string SEQ03 = "0000000000111111";
        public static readonly string FOL03 = "FFFFFFLLLRFFFFR";

        public static void Main(string[] args) {
            Folding F1 = new Folding(FOL01);
            //Console.WriteLine(F1.CalculateFitness(SEQ01));

            Folding F2 = new Folding(FOL02);
            //Console.WriteLine(F2.CalculateFitness(SEQ02));

            Folding F3 = new Folding(FOL03);
            //Console.WriteLine(F3.CalculateFitness(SEQ03));

            Population p = new Population(SEQ01, 50, 500);
            for (int i = 0; i < 50; i++) {
                p.Run();

                Console.WriteLine("------------");
                var fMax = p.Fitness.Max();

                Console.WriteLine("Min: {0} Max: {1} AVG: {2} E: {3}", p.Fitness.Min(), fMax, p.Fitness.Average(), (fMax -1000.0d) / 10.0d);

                var maxFolds = Enumerable.Range(0, p.Fitness.Count()).Where(x => p.Fitness[x] == fMax).Select(x => p.Foldings[x]).ToHashSet(x => x.foldSeq);

                foreach (var fold in maxFolds) {
                    var f = new Folding(fold);
                    f.print(SEQ01);
                }

                p.CurrentGeneration = 0;
            }

            Console.ReadKey();
        }
    }
}