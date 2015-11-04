using System;
using System.Collections.Generic;
using System.Linq;
using XGA.Config;
using XGA.Helper;

namespace XGA {

    public class GeneticAlgorithm<T, S> where T : IFitnessMeasured<S>, new() {

        public class GACache {

            public GACache(int length, S[] seq) {
                this.GAElement = new T();
                this.GAElement.BaseType = this.GAElement.GenerateRandom(length);
                this.Fitness = this.GAElement.CalculateFitness(seq);
            }

            public GACache(GACache copy) {
                this.Fitness = copy.Fitness;
                this.GAElement = (T) copy.GAElement.Clone();
            }

            public T GAElement { get; set; }
            public double Fitness { get; set; }
        }

        public GACache[] Cache { get; set; }

        public IEnumerable<T> Elements {
            get {
                return Cache.Select(x => x.GAElement);
            }
        }

        public IEnumerable<double> Fitness {
            get {
                return Cache.Select(x => x.Fitness);
            }
        }

        public double AvgFitness {
            get {
                return Cache.Average(x => x.Fitness);
            }
        }

        public double MaxFitness {
            get {
                return Cache.Max(x => x.Fitness);
            }
        }

        public double TotalFitness {
            get {
                return Cache.Sum(x => x.Fitness);
            }
        }

        public GeneticAlgorithmConfig<S> GAC { get; private set; }
        public int CurrentGeneration { get; set; }

        public IEnumerable<IGeneticOperator<T, S>> Operators { get; private set; }

        private Logger Log { get; set; }

        public GeneticAlgorithm(GeneticAlgorithmConfig<S> gac, IGeneticOperatorProvider<T, S> OperatorProvider, Logger log) {
            this.Log = log;
            this.CurrentGeneration = 0;
            this.Operators = OperatorProvider.GetOperators();

            this.GAC = gac;

            this.Cache = new GACache[this.GAC.PopulationSize];
            var SequenceLength = this.GAC.Sequence.Length - 1;
            for (int i = 0; i < this.GAC.PopulationSize; i++) {
                this.Cache[i] = new GACache(SequenceLength, this.GAC.Sequence);
            }
        }

        public void GenerationStep() {
            foreach (var op in this.Operators) {
                op.Operate(this, Log);

                for (int i = 0; i < this.Cache.Length; i++) {
                    var f = this.Cache[i];
                    var calcFitness = (int) Math.Floor(f.GAElement.CalculateFitness(this.GAC.Sequence));
                    var cacheFitness = (int) Math.Floor(f.Fitness);
                    if (calcFitness != cacheFitness) {
                        Console.WriteLine("Fail: {0}", i);
                    }
                }
            }

            this.CurrentGeneration++;
        }
    }
}