using System;
using XGA.Config;
using XGA.Helper;

namespace XGA.Folding {

    public class FoldingWorkingSet<C> : WorkingSet<Folding, string, C>
        where C : ICalculationMode<Folding, string> {

        public FoldingWorkingSet(string Name, GeneticAlgorithmConfig GAC, Func<GeneticAlgorithm<Folding, string>, C> CM) : base(Name, GAC, new FoldingDefaultOperatorProvider(), CM) {
        }

        public FoldingWorkingSet(string Name,
            GeneticAlgorithmConfig GAC,
            IGeneticOperatorProvider<Folding, string> Provider,
            Func<GeneticAlgorithm<Folding, string>, C> CM) :
            base(Name, GAC, Provider, CM) {
        }

        protected override void Evaluate(GeneticAlgorithm<Folding, string> GA) {
        }
    }
}