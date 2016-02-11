using System;
using System.Diagnostics;

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

        public int CurrentGeneration
        {
            get { return this.GA.CurrentGeneration; }
            set { this.GA.CurrentGeneration = value; }
        }

        protected GeneticAlgorithm<T> GA
        {
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

    public class TimedCalculation<T> : CalculationMode<T> where T : new() {
        public int MaxSeconds { get; private set; }
        private Stopwatch Watch = new Stopwatch();

        public TimedCalculation(GeneticAlgorithm<T> GA, int MaxSeconds) : base(GA) {
            this.MaxSeconds = MaxSeconds;
        }

        public override string getName() {
            return "Timed";
        }

        public override void Run(Action Evaluate) {
            int iRuns = 1;
            Watch.Reset();
            Watch.Start();
            while (( Watch.ElapsedMilliseconds ) + ( Watch.ElapsedMilliseconds / iRuns ) * 1.3 < this.MaxSeconds * 1000.0d) {
                GA.GenerationStep();
                Evaluate();
                iRuns++;
                //Console.WriteLine("AVGGTime: {0}", ( Watch.ElapsedMilliseconds / iRuns ));
            }
            Watch.Stop();
            GA.Log.Write(string.Format("T: {0} AVGT: {1}", Watch.ElapsedMilliseconds, Watch.ElapsedMilliseconds / iRuns));
        }
    }
}