using System;
using System.Collections.Generic;
using System.Linq;
using XGA.Config;
using XGA.Helper;

namespace XGA {

    public class GeneticAlgorithm<T> {

        public class GACache {
            /*public GACache(int length, T[] seq) {
                // new IFitnessMeasured<T>?

                this.GAElement.BaseType = this.GAElement.GenerateRandom(length);
                this.Fitness = this.GAElement.CalculateFitness(seq);
            }*/

            public GACache(GACache copy) {
                this.Fitness = copy.Fitness;
                this.GAElement = (IFitnessMeasured<T>) copy.GAElement.Clone();
            }

            public GACache() {
            }

            public IFitnessMeasured<T> GAElement { get; set; }
            public double Fitness { get; set; }
        }

        public List<GACache> Cache { get; set; }

        public IEnumerable<IFitnessMeasured<T>> Elements {
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

        public IFitnessMeasuredCreator<T> Creator { get; set; }

        public GeneticAlgorithmConfig<T> GAC { get; private set; }
        public int CurrentGeneration { get; set; }

        public IEnumerable<IGeneticOperator<T>> Operators { get; private set; }

        private Logger Log { get; set; }

        public GeneticAlgorithm(GeneticAlgorithmConfig<T> gac, IGeneticOperatorProvider<T> OperatorProvider, IFitnessMeasuredCreator<T> Creator, Logger log) {
            this.Log = log;
            this.CurrentGeneration = 0;
            this.Operators = OperatorProvider.GetOperators();

            this.Creator = Creator;

            this.GAC = gac;

            this.Cache = new List<GACache>(this.GAC.PopulationSize);

            for (int i = 0; i < this.GAC.PopulationSize; i++) {
                var Elem = Creator.CreateNew(this.GAC);
                Cache.Add(new GACache() { GAElement = Elem, Fitness = Elem.CalculateFitness(this.GAC.Sequence) });
            }
        }

        public void GenerationStep() {
            foreach (var op in this.Operators) {
                op.Operate(this, Log);

                /*for (int i = 0; i < this.Cache.Length; i++) {
                    var f = this.Cache[i];
                    var calcFitness = (int) Math.Floor(f.GAElement.CalculateFitness(this.GAC.Tequence));
                    var cacheFitness = (int) Math.Floor(f.Fitness);
                    if (calcFitness != cacheFitness) {
                        Console.WriteLine("Fail: {0}", i);
                    }
                }*/
            }

            this.CurrentGeneration++;
        }
    }
}