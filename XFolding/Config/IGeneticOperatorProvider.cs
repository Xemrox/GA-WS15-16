using System.Collections.Generic;

namespace XGA.Config {

    public interface IGeneticOperatorProvider<T> {

        IEnumerable<IGeneticOperator<T>> GetOperators();

        /*IGeneticSelect<T, S> GetSelector();

        IGeneticMutate<T, S> GetMutator();

        IGeneticCrossover<T, S> GetCrossover();*/
    }
}