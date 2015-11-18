using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using XGA.Config;
using XGA.Folding;
using XGA.Helper;

namespace XGA {

    public class Program {
        // benchmark sequences for the 2d HP model
        // 0 = hydrophil, "white"
        // 1 = hydrophob, "black"
        // source: Ron Unger, John Moult: Genetic Algorithms for Protein Folding Simulations,
        //         Journal of Molecular Biology, Vol. 231, No. 1, May 1993

        public static readonly string SEQ20 = "10100110100101100101";   //9
        public static readonly string SEQ24 = "110010010010010010010011";   //9
        public static readonly string SEQ25 = "0010011000011000011000011";  //8
        public static readonly string SEQ36 = "000110011000001111111001100001100100";   //14
        public static readonly string SEQ48 = "001001100110000011111111110000001100110010011111";   //22
        public static readonly string SEQ50 = "11010101011110100010001000010001000101111010101011"; //21
        public static readonly string SEQ60 = "001110111111110001111111111010001111111111110000111111011010";   //34

        public static readonly string SEQ64 = "1111111111110101001100110010011001100100110011001010111111111111";   //42

        public static readonly string SEQ01 = "10100110100101100101"; //9
        public static readonly string FOL01 = "FRFRRLLRFRRLRLLRRFR";

        public static readonly string SEQ02 = "0001101";
        public static readonly string FOL02 = "FFLLLF";

        public static readonly string SEQ03 = "0000000000111111";
        public static readonly string FOL03 = "FFFFFFLLLRFFFFR";

        public static void Main(string[] args) {
            Console.Title = "GA-2D-HP Modell";
            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) {
                /*mut.WaitOne();
                Console.WriteLine("-----------------------");
                foreach (var fold in maxFoldings) {
                    var f = new Folding(fold);
                    f.print(p.Sequence);
                }
                Console.WriteLine("-----------------------");

                string x = Console.ReadLine();

                if (x.ToLower().Equals("save")) {
                    var defOut = Console.Out;
                    var newOut = new StreamWriter("SEQ01-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + ".txt", false);
                    Console.SetOut(newOut);

                    foreach (var fold in maxFoldings) {
                        var f = new Folding(fold);
                        f.print(p.Sequence);
                    }
                    newOut.Flush();
                    Console.SetOut(defOut);
                    newOut.Close();
                }

                e.Cancel = !bFinished && !x.ToLower().Equals("exit");
                mut.ReleaseMutex();*/
            };

            /*var f = new Folding.Folding();
            f.BaseType = FOL01.ToCharArray();
            var F = f.CalculateFitness(SEQ01.ToCharArray());
            Console.WriteLine(Folding.Folding.Neighbours(F));*/

            /*var s = new Logger("test.txt");
            var F = new Folding.Folding();
            F.BaseType = FOL01.ToCharArray();
            F.print(SEQ01.ToCharArray(), s);
            s.Finish();*/

            /*var x = new FoldingWorkingSet<CalculationMode<Folding.Folding, string>>("SEQ01",
                new GeneticAlgorithmConfig() { Sequence = SEQ01 },
                (GA) => new FiniteCalculation<Folding.Folding, string>(GA, 100));*/

            var WS = new List<FoldingWorkingSet>();

            /*WS.Add(new FoldingWorkingSet("SEQ01",
                new GeneticAlgorithmConfig<char> { Sequence = SEQ01.ToCharArray(), PopulationSize = 1000 },
                new GenericGeneticOperatorProvider<Folding.Folding, char>(() =>
                {
                    return new List<IGeneticOperator<Folding.Folding, char>> {
                    new FoldingSelectOperator(),
                    new FoldingMutateOperator(),
                    new FoldingCrossoverOperator()
                };
                }),
                (GA) => new FiniteCalculation<Folding.Folding, char>(GA, 2000)));

            WS.Add(new FoldingWorkingSet("SEQ64",
                new GeneticAlgorithmConfig<char> { Sequence = SEQ64.ToCharArray(), PopulationSize = 500, MutationRate = 0.4, CrossoverRate = 0.3 },
                new GenericGeneticOperatorProvider<Folding.Folding, char>(() =>
                {
                    return new List<IGeneticOperator<Folding.Folding, char>> {
                    new FoldingSelectOperator(),
                    new FoldingMutateOperator(),
                    new FoldingCrossoverOperator()
                };
                }),
                (GA) => new FiniteCalculation<Folding.Folding, char>(GA, 2000)));*/

            WS.Add(new FoldingWorkingSet("SEQ24-1",
                new GeneticAlgorithmConfig<char> { Sequence = SEQ24.ToCharArray(), PopulationSize = 200, MutationRate = 0.03, CrossoverRate = 0.2 },
                new GenericGeneticOperatorProvider<char>(() =>
                {
                    return new List<IGeneticOperator<char>> {
                    new FoldingSelectOperator(),
                    new FoldingMutateOperator(),
                    new FoldingCrossoverOperator()
                };
                }),
                (GA) => new FiniteCalculation<char>(GA, 100)));

            /*WS.Add(new FoldingWorkingSet("SEQ24-2",
                new GeneticAlgorithmConfig<char> { Sequence = SEQ24.ToCharArray(), PopulationSize = 200, MutationRate = 0.03, CrossoverRate = 0.2 },
                new GenericGeneticOperatorProvider<Folding.Folding, char>(() =>
                {
                    return new List<IGeneticOperator<Folding.Folding, char>> {
                    new FoldingSelectOperator(),
                    new FoldingMutateOperator(),
                    new FoldingCrossoverOperator()
                };
                }),
                (GA) => new FiniteCalculation<Folding.Folding, char>(GA, 100)));

            WS.Add(new FoldingWorkingSet("SEQ24-3",
                new GeneticAlgorithmConfig<char> { Sequence = SEQ24.ToCharArray(), PopulationSize = 200, MutationRate = 0.03, CrossoverRate = 0.2 },
                new GenericGeneticOperatorProvider<Folding.Folding, char>(() =>
                {
                    return new List<IGeneticOperator<Folding.Folding, char>> {
                    new FoldingSelectOperator(),
                    new FoldingMutateOperator(),
                    new FoldingCrossoverOperator()
                };
                }),
                (GA) => new FiniteCalculation<Folding.Folding, char>(GA, 100)));
                */
            foreach (var ws in WS) {
                Console.WriteLine("Started: {0}", ws.Name);
                ThreadPool.QueueUserWorkItem(ws.Run);
            }

            WaitHandle.WaitAll(WS.Select(x => x.Lock).ToArray());
            /**/

            /*var testSeq = new List<string>();
            testSeq.Add("LLFLLRRLFLLRLRRLLFL");
            testSeq.Add("FLFLLRRLFLLRLRRLLFL");
            testSeq.Add("RLFLLRRLFLLRLRRLLFL");
            testSeq.Add("LLFLLRRLRLLFLRRLLFL");
            testSeq.Add("FLFLLRRLRLLFLRRLLFL");
            testSeq.Add("RLFLLRRLRLLFLRRLLFL");

            var fp = new FoldingPlus();
            fp.BaseType = FOL01.ToCharArray();
            Console.WriteLine(fp.CalculateFitness(SEQ01.ToCharArray()));
            fp.print(SEQ01.ToCharArray(), new ConsoleLogger());

            foreach (var s in testSeq) {
                fp.BaseType = s.ToCharArray();
                Console.WriteLine(fp.CalculateFitness(SEQ01.ToCharArray()));
            }

            fp.BaseType = "FFLLFRFRRLLFLFFLFFFFRRFFFFLFFFFRRFFFFFFFFFFLFFLFRLFLFRFLLFFFLLR".ToCharArray();
            Console.WriteLine(fp.CalculateFitness(SEQ64.ToCharArray()));*/

            //fp.BaseType = FOL02.ToCharArray();
            //fp.CalculateFitness(SEQ02.ToCharArray());
            //fp.print(SEQ02.ToCharArray(), new ConsoleLogger());

            Console.WriteLine("Finished");

            Console.ReadKey();
        }
    }
}