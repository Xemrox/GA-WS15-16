using System;
using System.Threading;
using XGA.Config;

namespace XGA.Helper {

    public abstract class WorkingSet<T, C>
        where T : new()
        where C : ICalculationMode<T> {
        public ManualResetEvent Lock { get; private set; }

        public string Name { get; private set; }

        public C CalculationMode { get; set; }

        public GeneticAlgorithm<T> GA { get; private set; }

        public GeneticAlgorithmConfig<T> GAC { get; private set; }

        public Logger Log { get; protected set; }

        protected WorkingSet(string Name, GeneticAlgorithmConfig<T> GAC, IGeneticOperatorProvider<T> Provider, IFitnessMeasuredCreator<T> Creator, Func<GeneticAlgorithm<T>, C> CM) {
            this.Lock = new ManualResetEvent(false);
            this.Name = Name;
            this.Log = new EmptyLogger();
            //this.Log = new StreamLogger(string.Format("{0}-{1}-{2}.log", Name, GAC.PopulationSize, DateTime.Now.ToString("yyyy-MM-dd-HH-mm")));
            this.GA = new GeneticAlgorithm<T>(GAC, Provider, Creator, Log);
            this.CalculationMode = CM(this.GA);
            this.GAC = GAC;
        }

        public void Run(Object Context = null) {
            this.CalculationMode.Run(this.Evaluate);

            this.Finished();

            this.Log.Finish();
            this.Lock.Set();
        }

        protected abstract void Evaluate();

        protected abstract void Finished();
    }
}