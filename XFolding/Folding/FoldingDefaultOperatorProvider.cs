using System.Collections.Generic;
using XGA.Config;

namespace XGA.Folding {

    public class FoldingDefaultOperatorProvider : IGeneticOperatorProvider<Folding, string> {
        public static List<IGeneticOperator<Folding, string>> Operators = new List<IGeneticOperator<Folding, string>>();

        static FoldingDefaultOperatorProvider() {
            Operators.Add(new FoldingSelectOperator());
            Operators.Add(new FoldingMutateOperator());
            Operators.Add(new FoldingCrossoverOperator());
        }

        IEnumerable<IGeneticOperator<Folding, string>> IGeneticOperatorProvider<Folding, string>.GetOperators() {
            return Operators;
        }
    }
}