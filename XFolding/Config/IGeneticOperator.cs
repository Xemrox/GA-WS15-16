using XGA.Helper;

namespace XGA.Config {

    public interface IGeneticOperator<T, S>
        where T : IFitnessMeasured<S>, new() {

        void Operate(GeneticAlgorithm<T, S> GA, Logger LOG);
    }

    public interface IGeneticSelect<T, S> : IGeneticOperator<T, S>
        where T : IFitnessMeasured<S>, new() {
    }

    public interface IGeneticMutate<T, S> : IGeneticOperator<T, S>
        where T : IFitnessMeasured<S>, new() {
    }

    public interface IGeneticCrossover<T, S> : IGeneticOperator<T, S>
        where T : IFitnessMeasured<S>, new() {
    }
}