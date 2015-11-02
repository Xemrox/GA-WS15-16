using System;
using System.Threading;
using XGA.Config;

namespace XGA.Helper {

    public abstract class WorkingSet<GAT, S, C>
        where C : ICalculationMode<GAT, S>
        where GAT : IFitnessMeasured<S>, new() {
        public ManualResetEvent Finished { get; private set; }

        public string Name { get; private set; }

        public C CalculationMode { get; set; }

        public GeneticAlgorithm<GAT, S> GA { get; private set; }

        public WorkingSet(string Name, GeneticAlgorithmConfig GAC, IGeneticOperatorProvider<GAT, S> Provider, Func<GeneticAlgorithm<GAT, S>, C> CM) {
            this.Finished = new ManualResetEvent(false);
            this.Name = Name;
            this.GA = new GeneticAlgorithm<GAT, S>(GAC, Provider, null);
            this.CalculationMode = CM(this.GA);
        }

        public void Run(Object Context) {
            this.CalculationMode.Run(this.Evaluate);

            this.Finished.Set();
        }

        protected abstract void Evaluate(GeneticAlgorithm<GAT, S> GA);
    }
}