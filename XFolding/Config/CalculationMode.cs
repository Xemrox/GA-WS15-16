using System;

namespace XGA.Config {

    public interface ICalculationMode<T, S>
        where T : IFitnessMeasured<S>, new() {
        GeneticAlgorithm<T, S> GA { get; set; }
        int CurrentGeneration { get; set; }

        string getName();

        void Run(Action<GeneticAlgorithm<T, S>> Evaluate);
    }

    public abstract class CalculationMode<T, S> : ICalculationMode<T, S>
        where T : IFitnessMeasured<S>, new() {

        protected CalculationMode(GeneticAlgorithm<T, S> GA) {
            this.GA = GA;
        }

        public int CurrentGeneration {
            get { return GA.CurrentGeneration; }
            set { GA.CurrentGeneration = value; }
        }

        public GeneticAlgorithm<T, S> GA {
            get; set;
        }

        public abstract string getName();

        public abstract void Run(Action<GeneticAlgorithm<T, S>> Evaluate);
    }

    public class FiniteCalculation<T, S> :
        CalculationMode<T, S> where T : IFitnessMeasured<S>, new() {
        public int MaxGeneration { get; private set; }

        public FiniteCalculation(GeneticAlgorithm<T, S> GA, int MaxGeneration) : base(GA) {
            this.MaxGeneration = MaxGeneration;
        }

        public override string getName() {
            return "Finite";
        }

        public override void Run(Action<GeneticAlgorithm<T, S>> Evaluate) {
            while (this.CurrentGeneration < this.MaxGeneration) {
                GA.GenerationStep();
                Evaluate(GA);
            }
        }
    }
}