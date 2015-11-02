using System;
using XGA.Config;
using XGA.Helper;

namespace XGA.Folding {

    public class FoldingSelectOperator : IGeneticSelect<Folding, string> {

        void IGeneticOperator<Folding, string>.Operate(GeneticAlgorithm<Folding, string> GA, Logger LOG) {
            throw new NotImplementedException();
        }
    }

    public class FoldingMutateOperator : IGeneticMutate<Folding, string> {

        void IGeneticOperator<Folding, string>.Operate(GeneticAlgorithm<Folding, string> GA, Logger LOG) {
            throw new NotImplementedException();
        }
    }

    public class FoldingCrossoverOperator : IGeneticCrossover<Folding, string> {

        void IGeneticOperator<Folding, string>.Operate(GeneticAlgorithm<Folding, string> GA, Logger LOG) {
            throw new NotImplementedException();
        }
    }
}