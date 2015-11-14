using XGA.Helper;

namespace XGA.Config {

    public interface IGeneticOperator<T> {

        void Operate(GeneticAlgorithm<T> GA, Logger LOG);
    }
}