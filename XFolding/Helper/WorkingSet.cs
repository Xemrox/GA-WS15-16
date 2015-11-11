using System;
using System.Threading;
using XGA.Config;

namespace XGA.Helper {

    public abstract class WorkingSet<GAT, S, C>
        where C : ICalculationMode<GAT, S>
        where GAT : IFitnessMeasured<S>, new() {
        public ManualResetEvent Lock { get; private set; }

        public string Name { get; private set; }

        public C CalculationMode { get; set; }

        public GeneticAlgorithm<GAT, S> GA { get; private set; }

        public GeneticAlgorithmConfig<S> GAC { get; private set; }

        public Logger Log { get; private set; }

        protected WorkingSet(string Name, GeneticAlgorithmConfig<S> GAC, IGeneticOperatorProvider<GAT, S> Provider, Func<GeneticAlgorithm<GAT, S>, C> CM) {
            this.Lock = new ManualResetEvent(false);
            this.Name = Name;
            this.Log = new StreamLogger(string.Format("{0}-{1}-{2}.log", Name, GAC.PopulationSize, DateTime.Now.ToString("yyyy-MM-dd-HH-mm")));
            this.GA = new GeneticAlgorithm<GAT, S>(GAC, Provider, Log);
            this.CalculationMode = CM(this.GA);
            this.GAC = GAC;
        }

        public void Run(Object Context) {
            this.CalculationMode.Run(this.Evaluate);

            this.Finished();

            this.Log.Finish();
            this.Lock.Set();
        }

        protected abstract void Evaluate();

        protected abstract void Finished();
    }
}