using System;
using System.Collections.Generic;
using XGA.Config;

namespace XGA.Folding {

    public class FoldingDefaultOperatorProvider : IGeneticOperatorProvider<char> {
        public static List<IGeneticOperator<char>> Operators = new List<IGeneticOperator<char>>();

        static FoldingDefaultOperatorProvider() {
            Operators.Add(new FoldingSelectOperator());
            Operators.Add(new FoldingMutateOperator());
            Operators.Add(new FoldingCrossoverOperator());
        }

        IEnumerable<IGeneticOperator<char>> IGeneticOperatorProvider<char>.GetOperators() {
            return Operators;
        }
    }
}