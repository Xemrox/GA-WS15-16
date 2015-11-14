using System;
using System.Collections.Generic;
using XGA.Config;

namespace XGA.Helper {

    public class GenericGeneticOperatorProvider<T> : IGeneticOperatorProvider<T> where T : new() {
        private readonly Func<IEnumerable<IGeneticOperator<T>>> Wrapper;

        public GenericGeneticOperatorProvider(Func<IEnumerable<IGeneticOperator<T>>> Wrapper) {
            this.Wrapper = Wrapper;
        }

        public IEnumerable<IGeneticOperator<T>> GetOperators() {
            return Wrapper();
        }
    }
}