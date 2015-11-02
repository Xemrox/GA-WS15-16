using System.Collections.Generic;

namespace XGA.Config {

    public interface IGeneticOperatorProvider<T, S>
        where T : IFitnessMeasured<S>, new() {

        IEnumerable<IGeneticOperator<T, S>> GetOperators();

        /*IGeneticSelect<T, S> GetSelector();

        IGeneticMutate<T, S> GetMutator();

        IGeneticCrossover<T, S> GetCrossover();*/
    }
}