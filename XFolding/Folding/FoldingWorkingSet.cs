using System;
using XGA.Config;
using XGA.Helper;

namespace XGA.Folding {

    public class FoldingWorkingSet : WorkingSet<Folding, string, CalculationMode<Folding, string>>
        /*where C : ICalculationMode<Folding, string>*/ {

        public FoldingWorkingSet(string Name, GeneticAlgorithmConfig GAC, Func<GeneticAlgorithm<Folding, string>, CalculationMode<Folding, string>> CM) :
            base(Name, GAC, new FoldingDefaultOperatorProvider(), CM) {
        }

        public FoldingWorkingSet(string Name,
            GeneticAlgorithmConfig GAC,
            IGeneticOperatorProvider<Folding, string> Provider,
            Func<GeneticAlgorithm<Folding, string>, CalculationMode<Folding, string>> CM) :
            base(Name, GAC, Provider, CM) {
        }

        protected override void Evaluate(GeneticAlgorithm<Folding, string> GA) {
        }
    }
}