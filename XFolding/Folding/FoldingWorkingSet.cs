using System;
using System.Linq;
using XGA.Config;
using XGA.Helper;

namespace XGA.Folding {

    public class FoldingWorkingSet : WorkingSet<Folding, char, CalculationMode<Folding, char>>
        /*where C : ICalculationMode<Folding, char>*/ {

        public FoldingWorkingSet(string Name, GeneticAlgorithmConfig<char> GAC, Func<GeneticAlgorithm<Folding, char>, CalculationMode<Folding, char>> CM) :
            base(Name, GAC, new FoldingDefaultOperatorProvider(), CM) {
        }

        public FoldingWorkingSet(string Name,
            GeneticAlgorithmConfig<char> GAC,
            IGeneticOperatorProvider<Folding, char> Provider,
            Func<GeneticAlgorithm<Folding, char>, CalculationMode<Folding, char>> CM) :
            base(Name, GAC, Provider, CM) {
        }

        protected override void Evaluate(GeneticAlgorithm<Folding, char> GA) {
            Console.WriteLine("[{0}] Max: {1} Avg: {2}", GA.CurrentGeneration, Folding.Neighbours(GA.MaxFitness), Folding.Neighbours(GA.AvgFitness));
        }
    }
}