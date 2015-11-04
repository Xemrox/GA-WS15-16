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

        public static readonly string SEQ20 = "10100110100101100101";
        public static readonly string SEQ24 = "110010010010010010010011";   //12
        public static readonly string SEQ25 = "0010011000011000011000011";  //12?
        public static readonly string SEQ36 = "000110011000001111111001100001100100";
        public static readonly string SEQ48 = "001001100110000011111111110000001100110010011111";
        public static readonly string SEQ50 = "11010101011110100010001000010001000101111010101011";
        public static readonly string SEQ64 = "1111111111110101001100110010011001100100110011001010111111111111";

        public static readonly string SEQ01 = "10100110100101100101"; //9
        public static readonly string FOL01 = "FRFRRLLRFRRLRLLRRFR";

        public static readonly string SEQ02 = "0001101";
        public static readonly string FOL02 = "FFLLLF";

        public static readonly string SEQ03 = "0000000000111111";
        public static readonly string FOL03 = "FFFFFFLLLRFFFFR";

        /*private class WorkingSet<T> where T : CalculationMode {
            public string Name { get; set; }

            //public GeneticAlgorithm Population { get; set; }
            public double maxFitness { get; set; }

            public HashSet<string> maxFoldings { get; private set; }
            public Mutex StopMutex { get; private set; }
            public ManualResetEvent Finished { get; private set; }
            public StreamWriter Output { get; private set; }
            public Action<string> Log { get; private set; }
            public CalculationMode CalcMode { get; private set; }

            public WorkingSet(string Name, T cm) {
                this.Name = Name;
                this.CalcMode = cm;
                this.Population = p;
                this.Output = Output;
                this.Population.Output = new StreamWriter(Name + DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + ".txt");
                this.maxFitness = 0.0;
                this.StopMutex = new Mutex(false);
                this.maxFoldings = new HashSet<string>();
                this.Finished = new ManualResetEvent(false);
            }

            public void Run(object Context) {
                /*while (this.Population.LeftGenerations > 0) {
                    this.StopMutex.WaitOne();

                    this.Population.GenerationStep();
                    var fMax = this.Population.Fitness.Max();
                    if (fMax > maxFitness) {
                        maxFitness = fMax;
                        maxFoldings.Clear();
                    }
                    var TrueNeighbours = ( fMax - 1000.0d ) / 100.0d;
                    Output.WriteLine("G: {4} E: {3} Max: {1} Min: {0} AVG: {2}", Population.Fitness.Min(), fMax, Population.Fitness.Average(), TrueNeighbours, Population.CurrentGeneration);
                    var maxFolds = Enumerable.Range(0, this.Population.Fitness.Count()).Where(x => Population.Fitness[x] == fMax).Select(x => Population.Foldings[x]).ToHashSet(x => x.foldSeq);

                    foreach (var fold in maxFolds) {
                        var f = new Folding(fold);
                        if (f.CalculateFitness(Population.Sequence) >= maxFitness) {
                            maxFoldings.Add(fold);
                        }
                    }

                    this.StopMutex.ReleaseMutex();
                }

                foreach (var fold in this.maxFoldings) {
                    new Folding(fold).print(this.Population.Sequence, this.Output);
                }

                this.Output.Flush();
                this.Output.Close();

                this.Finished.Set();
            }*/
        /*
    CalcMode.Run(this.Population, this.Evaluate);
        }

private void Evaluate(GeneticAlgorithm p) {
}
}*/

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

            var f = new Folding.Folding();
            f.BaseType = FOL01.ToCharArray();
            var F = f.CalculateFitness(SEQ01.ToCharArray());
            Console.WriteLine(Folding.Folding.Neighbours(F));

            //var s = new StreamWriter("test.txt");
            //f.print(SEQ01, s);
            //s.Flush();
            //s.Close();

            /*var x = new FoldingWorkingSet<CalculationMode<Folding.Folding, string>>("SEQ01",
                new GeneticAlgorithmConfig() { Sequence = SEQ01 },
                (GA) => new FiniteCalculation<Folding.Folding, string>(GA, 100));*/

            ///TODO
            /// Add Logger
            /// move Algorithm operations

            var x = new FoldingWorkingSet("SEQ01",
                new GeneticAlgorithmConfig<char> { Sequence = SEQ01.ToCharArray(), PopulationSize = 1000 },
                new GenericGeneticOperatorProvider<Folding.Folding, char>(() =>
                {
                    return new List<IGeneticOperator<Folding.Folding, char>> {
                    new FoldingSelectOperator(),
                    new FoldingMutateOperator(),
                    new FoldingCrossoverOperator()
                };
                }),
                (GA) => new FiniteCalculation<Folding.Folding, char>(GA, 500));

            var WS = new List<FoldingWorkingSet>();

            WS.Add(x);

            //WS.Add(new WorkingSet("SEQ01", new Population(SEQ01, 200, 10000, 0.20, 0.30), new StreamWriter("SEQ01-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + ".txt")));
            //WS.Add(new WorkingSet("SEQ50", new Population(SEQ50, 200, 10000, 0.10, 0.30), new StreamWriter("SEQ50-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + ".txt")));
            //WS.Add(new WorkingSet("SEQ48", new Population(SEQ48, 200, 10000, 0.40, 0.45), new StreamWriter("SEQ48-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + ".txt")));
            //WS.Add(new WorkingSet("SEQ64", new Population(SEQ64, 200, 10000, 0.30, 0.45), new StreamWriter("SEQ64-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + ".txt")));

            foreach (var ws in WS) {
                Console.WriteLine("Started: {0}", ws.Name);
                ThreadPool.QueueUserWorkItem(ws.Run);
            }

            /*Random rnd = new Random(42);
            var nums = new double[100];
            for (int i = 0; i < 100; i++) {
                nums[i] = rnd.NextDouble();
            }

            var total = nums.Sum();
            var rel = nums.Select(d => d / total).ToArray();
            var cumulatives = new double[100];
            var cumulative = 0.0d;

            for (int i = 0; i < 100; i++) {
                cumulatives[i] = cumulative + rel[i];
                cumulative += rel[i];
            }*/

            WaitHandle.WaitAll(WS.Select(y => y.Finished).ToArray());

            //Console.WriteLine("{0}", Math.Log(1 - 0.5) / -( 0.10 * 1000 ));

            Console.WriteLine("Finished");

            Console.ReadKey();
        }
    }
}