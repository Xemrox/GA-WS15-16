using System;

namespace XGA.Config {

    public interface ICalculationMode<T> where T : new() {
        int CurrentGeneration { get; set; }

        string getName();

        void Run(Action Evaluate);
    }

    public abstract class CalculationMode<T> : ICalculationMode<T> where T : new() {

        protected CalculationMode(GeneticAlgorithm<T> GA) {
            this.GA = GA;
        }

        public int CurrentGeneration {
            get { return this.GA.CurrentGeneration; }
            set { this.GA.CurrentGeneration = value; }
        }

        protected GeneticAlgorithm<T> GA {
            get; set;
        }

        public abstract string getName();

        public abstract void Run(Action Evaluate);
    }

    public class FiniteCalculation<T> : CalculationMode<T> where T : new() {
        public int MaxGeneration { get; private set; }

        public FiniteCalculation(GeneticAlgorithm<T> GA, int MaxGeneration) : base(GA) {
            this.MaxGeneration = MaxGeneration;
        }

        public override string getName() {
            return "Finite";
        }

        public override void Run(Action Evaluate) {
            while (this.CurrentGeneration < this.MaxGeneration) {
                GA.GenerationStep();
                Evaluate();
            }
        }
    }
}