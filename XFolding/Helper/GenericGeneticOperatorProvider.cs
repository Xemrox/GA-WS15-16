using System;
using System.Collections.Generic;
using XGA.Config;

namespace XGA.Helper {

    public class GenericGeneticOperatorProvider<T, S> : IGeneticOperatorProvider<T, S>
        where T : IFitnessMeasured<S>, new() {
        private readonly Func<IEnumerable<IGeneticOperator<T, S>>> Wrapper;

        public GenericGeneticOperatorProvider(Func<IEnumerable<IGeneticOperator<T, S>>> Wrapper) {
            this.Wrapper = Wrapper;
        }

        IEnumerable<IGeneticOperator<T, S>> IGeneticOperatorProvider<T, S>.GetOperators() {
            return Wrapper();
        }
    }
}