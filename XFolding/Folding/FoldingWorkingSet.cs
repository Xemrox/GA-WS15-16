using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using XGA.Config;
using XGA.Helper;

namespace XGA.Folding {

    public class FoldingWorkingSet : WorkingSet<char, CalculationMode<char>> {
        public bool CalculateHamming { get; set; }

        public FoldingWorkingSet(string Name, bool Hamming, GeneticAlgorithmConfig<char> GAC, Func<GeneticAlgorithm<char>, CalculationMode<char>> CM) :
            base(Name, GAC, new FoldingDefaultOperatorProvider(), new FoldingCreator(), CM) {
            this.CalculateHamming = Hamming;
            this.Log = new ConsoleLogger();
            //this.Log = new StreamLogger(string.Format("{0}-{1}-{2}.log", Name, GAC.PopulationSize, DateTime.Now.ToString("yyyy-MM-dd-HH-mm")));
        }

        public FoldingWorkingSet(string Name,
            bool Hamming,
            GeneticAlgorithmConfig<char> GAC,
            IGeneticOperatorProvider<char> Provider,
            Func<GeneticAlgorithm<char>, CalculationMode<char>> CM) :
            base(Name, GAC, Provider, new FoldingCreator(), CM) {
            //this.Log = new StreamLogger(string.Format("{0}-{1}-{2}.log", Name, GAC.PopulationSize, DateTime.Now.ToString("yyyy-MM-dd-HH-mm")));
            this.Log = new ConsoleLogger();
            this.CalculateHamming = Hamming;
        }

        private readonly HashSet<string> Masters = new HashSet<string>();
        private double MasterFitness = 0.0d;

        protected int HammingDistance(char[] baseFold, char[] compareFold) {
            //assume from fitness function that both sequence lengths equal
            int cHemming = 0;
            for (int i = 0; i < baseFold.Length; i++) {
                //char calculation
                if (baseFold[i] != compareFold[i]) cHemming++;
                //cHemming += Math.Abs(Folding.MapDir(baseFold[i]) - Folding.MapDir(compareFold[i]));
            }
            return cHemming;
        }

        protected double CalculateHemming(char[] baseFold, IEnumerable<char[]> compareFold) {
            var hammingSize = compareFold.Count();
            double hamming = 0.0d;
            for (int combination = 0; combination < hammingSize; combination++) {
                var elem = compareFold.ElementAt(combination);
                if (elem.SequenceEqual(baseFold)) continue;
                //skip distance to self

                hamming += (double) HammingDistance(baseFold, elem) / (double) ( hammingSize - 1.0d );
            }
            return hamming;
        }

        protected string CalculateAVGHemming(IEnumerable<char[]> GA) {
            var hammingSize = GA.Count();
            var hamminglist = new double[hammingSize];

            //create permutations
            for (int start = 0; start < hammingSize; start++) {
                var baseElement = GA.ElementAt(start);
                for (int combination = 0; combination < hammingSize; combination++) {
                    if (start == combination) continue; //skip distance to self
                    var distanceElement = GA.ElementAt(combination);

                    hamminglist[start] += (double) HammingDistance(baseElement, distanceElement) / (double) ( hammingSize - 1.0d );
                }
            }
            var hammingAvg = hamminglist.Average();
            for (int i = 0; i < hamminglist.Length; i++) {
                hamminglist[i] = Math.Pow(hamminglist[i] - hammingAvg, 2);
            }
            var hammingVar = Math.Sqrt(hamminglist.Average());
            return string.Format("Hamming: {0} HV: {1}", hammingAvg, hammingVar);
        }

        protected override void Evaluate() {
            double MaxFitness = GA.MaxFitness;
            double AvgFitness = GA.AvgFitness;
            if (MaxFitness > MasterFitness) {
                MasterFitness = MaxFitness;
                Masters.Clear();
                Masters.AddRange(GA.Cache.Where(x => x.Fitness >= MaxFitness).Select(x => new string(x.GAElement.BaseType)));
            } else if (MaxFitness >= MasterFitness) {
                Masters.AddRange(GA.Cache.Where(x => x.Fitness >= MaxFitness).Select(x => new string(x.GAElement.BaseType)));
            }
            string msg;
            if (this.CalculateHamming) {
                msg = string.Format("[{0}] Max: {1} MaxF: {2} Avg: {3} AvgF: {4} {5}", GA.CurrentGeneration, MaxFitness, Folding.Neighbours(MaxFitness), AvgFitness, Folding.Neighbours(AvgFitness), GA.Cache, CalculateAVGHemming(GA.Cache.Select(x => x.GAElement.BaseType)));
            } else {
                msg = string.Format("[{0}] Max: {1} MaxF: {2} Avg: {3} AvgF: {4}", GA.CurrentGeneration, MaxFitness, Folding.Neighbours(MaxFitness), AvgFitness, Folding.Neighbours(AvgFitness));
            }
            Log.Log(msg);
        }

        protected override void Finished() {
            List<char[]> cMasters = Masters.Select(x => x.ToCharArray()).ToList();
            foreach (var x in cMasters) {
                var f = new Folding();
                f.BaseType = x;
                f.print(this.GAC.Sequence, this.Log);
                //f.print(this.GAC.Sequence, new ConsoleLogger());
                Console.WriteLine(CalculateHemming(f.BaseType, cMasters));
            }
            Console.WriteLine(CalculateAVGHemming(Masters.Select(y => y.ToCharArray())));

            var msg = String.Format("S: {0} G: {1} P: {2} M: {3} C: {4}{5}", new string(GAC.Sequence), GA.CurrentGeneration, GAC.PopulationSize, GAC.MutationRate, GAC.CrossoverRate, Environment.NewLine);
            this.Log.Write(msg);
        }
    }
}