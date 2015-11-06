using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using XGA.Config;
using XGA.Helper;

namespace XGA.Folding {

    public class FoldingWorkingSet : WorkingSet<Folding, char, CalculationMode<Folding, char>> {

        public FoldingWorkingSet(string Name, GeneticAlgorithmConfig<char> GAC, Func<GeneticAlgorithm<Folding, char>, CalculationMode<Folding, char>> CM) :
            base(Name, GAC, new FoldingDefaultOperatorProvider(), CM) {
        }

        public FoldingWorkingSet(string Name,
            GeneticAlgorithmConfig<char> GAC,
            IGeneticOperatorProvider<Folding, char> Provider,
            Func<GeneticAlgorithm<Folding, char>, CalculationMode<Folding, char>> CM) :
            base(Name, GAC, Provider, CM) {
        }

        private HashSet<string> Masters = new HashSet<string>();
        private double MasterFitness = 0.0d;

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

            var msg = string.Format("[{0}] Max: {1} MaxF: {2} Avg: {3} AvgF: {4}", GA.CurrentGeneration, MaxFitness, Folding.Neighbours(MaxFitness), AvgFitness, Folding.Neighbours(AvgFitness));
            Console.WriteLine(msg);
            Log.Log(msg);
        }

        protected override void Finished() {
            foreach (var x in Masters) {
                var f = new Folding();
                f.BaseType = x.ToCharArray();
                f.print(this.GAC.Sequence, this.Log);
            }

            var msg = String.Format("S: {0} P: {1} M: {2} C: {3}{4}", new string(GAC.Sequence), GA.CurrentGeneration, GAC.PopulationSize, GAC.MutationRate, GAC.CrossoverRate, Environment.NewLine);
            Console.WriteLine(msg);
            this.Log.Write(msg);
        }
    }
}