using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using XGA.Config;
using XGA.Helper;

namespace XGA.Folding {

    public class FoldingWorkingSet : WorkingSet<char, CalculationMode<char>> {

        public FoldingWorkingSet(string Name, GeneticAlgorithmConfig<char> GAC, Func<GeneticAlgorithm<char>, CalculationMode<char>> CM) :
            base(Name, GAC, new FoldingDefaultOperatorProvider(), CM) {
        }

        public FoldingWorkingSet(string Name,
            GeneticAlgorithmConfig<char> GAC,
            IGeneticOperatorProvider<char> Provider,
            Func<GeneticAlgorithm<char>, CalculationMode<char>> CM) :
            base(Name, GAC, Provider, CM) {
        }

        private readonly HashSet<string> Masters = new HashSet<string>();
        private double MasterFitness = 0.0d;

        protected static int HammingDistance(char[] baseFold, char[] compareFold) {
            //assume from fitness function that both sequence lengths equal
            int cHemming = 0;
            for (int i = 0; i < baseFold.Length; i++) {
                //char calculation
                cHemming += Math.Abs(Folding.MapDir(baseFold[i]) - Folding.MapDir(compareFold[i]));
            }
            return cHemming;
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

            var hammingSize = GA.GAC.PopulationSize;
            var hamminglist = new double[hammingSize];

            //create permutations
            for (int start = 0; start < GA.GAC.PopulationSize; start++) {
                for (int combination = 0; combination < GA.GAC.PopulationSize; combination++) {
                    if (start == combination) continue;
                    //skip distance to self

                    var baseBla = GA.Cache[start].GAElement.BaseType;
                    var distanceBla = GA.Cache[combination].GAElement.BaseType;

                    hamminglist[start] += (double) HammingDistance(baseBla, distanceBla) / (double) GA.GAC.PopulationSize;
                }
            }
            var hammingAvg = hamminglist.Average();
            for (int i = 0; i < hamminglist.Length; i++) {
                hamminglist[i] = Math.Pow(hamminglist[i] - hammingAvg, 2);
            }
            var hammingVar = Math.Sqrt(hamminglist.Average());

            var msg = string.Format("[{0}] Max: {1} MaxF: {2} Avg: {3} AvgF: {4} Hamming: {5} HV: {6}", GA.CurrentGeneration, MaxFitness, Folding.Neighbours(MaxFitness), AvgFitness, Folding.Neighbours(AvgFitness), GAC.Hamming ? hammingAvg : 0.0, hammingVar);
            Console.WriteLine(msg);
            Log.Log(msg);
        }

        protected override void Finished() {
            foreach (var x in Masters) {
                var f = new Folding();
                f.BaseType = x.ToCharArray();
                f.print(this.GAC.Sequence, this.Log);
                f.print(this.GAC.Sequence, new ConsoleLogger());
            }

            var msg = String.Format("S: {0} G: {1} P: {2} M: {3} C: {4}{5}", new string(GAC.Sequence), GA.CurrentGeneration, GAC.PopulationSize, GAC.MutationRate, GAC.CrossoverRate, Environment.NewLine);
            Console.WriteLine(msg);
            this.Log.Write(msg);
        }
    }
}