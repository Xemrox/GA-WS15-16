using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XGA.Config;
using XGA.Folding;
using XGA.Helper;
using XGA.SilentStatistics;

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

		private struct ParamTuple {
			public double CrossoverRate { get; set; }
			public double MutationRate { get; set; }
		}

		public static void Main(string[] args) {
            if (args.Length > 0 && !string.IsNullOrEmpty(args[0])) {
                System.Environment.CurrentDirectory = args[0];
            }

			Console.Title = "GA-2D-HP Modell";

			var WS = new List<WorkingSet<char, CalculationMode<char>>>();

			/*var Work = new Stack<ParamTuple>();

            var Tasks = new List<Task>();

            for (double CrossoverRate = 1.0; CrossoverRate >= 0.0; CrossoverRate -= 0.01) {
                for (double MutationRate = 1.0; MutationRate >= 0.0; MutationRate -= 0.01) {
                    Work.Push(new ParamTuple() { CrossoverRate = CrossoverRate, MutationRate = MutationRate });
                }
            }
            int iCurrentWork = 1;
            while (Tasks.Count > 0 || Work.Count > 0) {
                var min = Math.Min(15, Work.Count);
                for (int i = Tasks.Count; i < min; i++) {
                    var tuple = Work.Pop();
                    Tasks.Add(Task.Factory.StartNew(() =>
                    {
                        var WS = new SilentWorkingSet("SEQ36",
                                new GeneticAlgorithmConfig<char> { Sequence = SEQ36.ToCharArray(), PopulationSize = 200, MutationRate = tuple.MutationRate, CrossoverRate = tuple.CrossoverRate },
                                new FoldingDefaultOperatorProvider(),
                                (GA) => new FiniteCalculation<char>(GA, 100));
                        WS.Run();
                    }));
                    iCurrentWork++;
                }
                var x = Task.WhenAny(Tasks);
                Tasks.Remove(x.Result);
            }*/
			WS.Add(new FoldingWorkingSet("SE20", false,
				new GeneticAlgorithmConfig<char> { Sequence = SEQ20.ToCharArray(), PopulationSize = 500, MutationRate = 0.03, CrossoverRate = 0.5 },
				new GenericGeneticOperatorProvider<char>(() => {
					return new List<IGeneticOperator<char>> {
					new FoldingSelectOperator(),
                    //new FoldingLinearRankSelectOperator(),
                    new FoldingMutateOperator(),
					new FoldingCrossoverOperator()
				};
				}),
				(GA) => new FiniteCalculation<char>(GA, 100)));

			foreach (var ws in WS) {
				Console.WriteLine("Started: {0}", ws.Name);
				ThreadPool.QueueUserWorkItem(ws.Run);
			}

			WaitHandle.WaitAll(WS.Select(x => x.Lock).ToArray());
			//Console.ReadLine();

			Console.WriteLine("Finished");

			Console.ReadKey();
		}
	}
}