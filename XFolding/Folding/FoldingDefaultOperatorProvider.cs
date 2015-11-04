using System.Collections.Generic;
using XGA.Config;

namespace XGA.Folding {

    public class FoldingDefaultOperatorProvider : IGeneticOperatorProvider<Folding, char> {
        public static List<IGeneticOperator<Folding, char>> Operators = new List<IGeneticOperator<Folding, char>>();

        static FoldingDefaultOperatorProvider() {
            Operators.Add(new FoldingSelectOperator());
            Operators.Add(new FoldingMutateOperator());
            Operators.Add(new FoldingCrossoverOperator());
        }

        IEnumerable<IGeneticOperator<Folding, char>> IGeneticOperatorProvider<Folding, char>.GetOperators() {
            return Operators;
        }
    }
}