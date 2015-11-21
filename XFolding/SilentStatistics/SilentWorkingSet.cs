using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XGA.Config;
using XGA.Folding;
using XGA.Helper;

namespace XGA.SilentStatistics {

    public class SilentWorkingSet : WorkingSet<char, CalculationMode<char>> {

        public SilentWorkingSet(string Name, GeneticAlgorithmConfig<char> GAC, Func<GeneticAlgorithm<char>, CalculationMode<char>> CM) :
            base(Name, GAC, new FoldingDefaultOperatorProvider(), new FoldingCreator(), CM) {
            this.Log = new StreamLogger(string.Format("{0}-{1}-{2}.log", Name, GAC.PopulationSize, DateTime.Now.ToString("yyyy-MM-dd-HH-mm"))); ;
        }

        private readonly HashSet<string> Masters = new HashSet<string>();
        private double MasterFitness = 0.0d;

        public SilentWorkingSet(string Name,
            GeneticAlgorithmConfig<char> GAC,
            IGeneticOperatorProvider<char> Provider,
            Func<GeneticAlgorithm<char>, CalculationMode<char>> CM) :
            base(Name, GAC, Provider, new FoldingCreator(), CM) {
            this.Log = new StreamLogger(string.Format("{0}-{1}-{2}-{3}-{4}.log", Name, GAC.PopulationSize, GAC.CrossoverRate.ToString("F2"), GAC.MutationRate.ToString("F2"), DateTime.Now.ToString("yyyy-MM-dd-HH-mm"))); ;
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
            string msg = string.Format("[{0}] Max: {1} MaxF: {2} Avg: {3} AvgF: {4}", GA.CurrentGeneration, MaxFitness, Folding.Folding.Neighbours(MaxFitness), AvgFitness, Folding.Folding.Neighbours(AvgFitness));
            Log.Log(msg);
        }

        protected override void Finished() {
            var f = new Folding.Folding();
            foreach (var x in Masters) {
                f.BaseType = x.ToCharArray();
                f.print(this.GAC.Sequence, this.Log);
            }

            var msg = String.Format("S: {0} G: {1} P: {2} M: {3} C: {4}{5}", new string(GAC.Sequence), GA.CurrentGeneration, GAC.PopulationSize, GAC.MutationRate, GAC.CrossoverRate, Environment.NewLine);
            Console.WriteLine(msg);
            this.Log.Write(msg);
        }
    }
}