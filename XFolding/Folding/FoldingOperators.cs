﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using XGA.Config;
using XGA.Helper;

namespace XGA.Folding {

    public class FoldingSelectOperator : IGeneticOperator<char> {

        public void Operate(GeneticAlgorithm<char> GA, Logger LOG) {
            var TotalFitness = GA.TotalFitness;
            var relFitness = GA.Fitness.Select(x => x / TotalFitness).ToArray();

            var cumulatives = new double[GA.GAC.PopulationSize];
            var cumulative = 0.0d;

            //Console.WriteLine("Highest: {0}[{1}]", relFitness.Max(), Enumerable.Range(0, relFitness.Length - 1).Select(x => relFitness.Max() == relFitness[x]).First());

            for (int i = 0; i < GA.GAC.PopulationSize; i++) {
                cumulatives[i] = cumulative + relFitness[i];
                cumulative += relFitness[i];
            }

            var Selection = new List<GeneticAlgorithm<char>.GACache>();

            foreach (var rnd in RandomHelper.GetNextDoubleList(GA.GAC.PopulationSize)) {
                //for (int i = 0; i < GA.GAC.PopulationSize; i++) {
                var iIndex = Array.BinarySearch<double>(cumulatives, rnd);
                if (iIndex < 0) iIndex = ~iIndex;

                //Console.WriteLine("Took: {0}", iIndex);

                Selection.Add(new GeneticAlgorithm<char>.GACache(GA.Cache[iIndex]));
            }

            GA.Cache = Selection;
        }
    }

    public class FoldingLinearRankSelectOperator : IGeneticOperator<char> {

        public void Operate(GeneticAlgorithm<char> GA, Logger LOG) {
            var sortedCache = GA.Cache.OrderByDescending(x => x.Fitness).ToList();

            const double iMax = 1; //1<=max<=2
            const double iMin = 2 - iMax;
            double n = GA.GAC.PopulationSize;

            Func<int, int> ExpVal = (rank) =>
            {
                return (int) Math.Round(( iMin + iMax ) * ( ( n - (double) rank ) / ( n - 1 ) ));
            };
            var newCache = new List<GeneticAlgorithm<char>.GACache>(GA.GAC.PopulationSize);
            for (int i = 0; i < GA.GAC.PopulationSize; ++i) {
                for (int selCount = 0; selCount < ExpVal(i + 1); ++selCount) {
                    newCache.Add(new GeneticAlgorithm<char>.GACache(sortedCache.ElementAt(i)));
                }
            }
            GA.Cache = newCache;
        }
    }

    public class FoldingMutateOperator : IGeneticOperator<char> {
        private static readonly Dictionary<char, char[]> MutationTable = new Dictionary<char, char[]>();

        static FoldingMutateOperator() {
            MutationTable.Add('L', new char[] { 'F', 'R' });
            MutationTable.Add('F', new char[] { 'L', 'R' });
            MutationTable.Add('R', new char[] { 'F', 'L' });
        }

        public void Operate(GeneticAlgorithm<char> GA, Logger LOG) {
            var mutations = RandomHelper.PoissonInt(GA.GAC.MutationRate * GA.GAC.PopulationSize);
            var chosen = new Dictionary<GeneticAlgorithm<char>.GACache, int>();

            /*int iHits = 0;
            for (int i = 0; i < GA.GAC.PopulationSize; i++) {
                if (RandomHelper.GetNextDouble() <= GA.GAC.MutationRate) {
                    iHits++;
                }
            }

            mutations = iHits;*/

            for (var i = 0; i < mutations; i++) {
                var idx = RandomHelper.GetNextInteger(GA.GAC.PopulationSize);
                int Entry;
                if (chosen.TryGetValue(GA.Cache[idx], out Entry)) {
                    chosen[GA.Cache[idx]] = Entry + 1;
                } else {
                    chosen.Add(GA.Cache[idx], 1);
                }
            }

            foreach (var Entry in chosen) {
                for (int i = 0; i < Entry.Value; i++) {
                    Mutate(GA, Entry.Key, LOG);
                }
            }
        }

        private void Mutate(GeneticAlgorithm<char> GA, GeneticAlgorithm<char>.GACache entry, Logger LOG) {
            var MutationType = RandomHelper.GetAsyncNextInteger(1);
            var Index = RandomHelper.GetNextInteger(entry.GAElement.BaseType.Length);

            entry.GAElement.BaseType[Index] = MutationTable[entry.GAElement.BaseType[Index]][MutationType.Result];
            entry.Fitness = entry.GAElement.CalculateFitness(GA.GAC.Sequence);
        }
    }

    public class FoldingCrossoverOperator : IGeneticOperator<char> {

        public void Operate(GeneticAlgorithm<char> GA, Logger LOG) {
            var crossovers = RandomHelper.ExponentialInt(GA.GAC.CrossoverRate * GA.GAC.PopulationSize);

            /*int iHits = 0;
            for (int i = 0; i < GA.GAC.PopulationSize; i++) {
                if (RandomHelper.GetNextDouble() <= GA.GAC.CrossoverRate) {
                    iHits++;
                }
            }

            crossovers = iHits;*/

            if (crossovers % 2 != 0) {
                crossovers += 1;
            }

            for (int i = 0; i < crossovers; i += 2) {
                var index1 = RandomHelper.GetNextInteger(GA.GAC.PopulationSize);
                var f1 = GA.Cache[index1];
                var cutIndexTask = RandomHelper.GetAsyncNextInteger(f1.GAElement.BaseType.Length - 1);

                var index2 = RandomHelper.GetNextInteger(GA.GAC.PopulationSize);
                while (index1 == index2)    //improve number select
                    index2 = RandomHelper.GetNextInteger(GA.GAC.PopulationSize);

                var f2 = GA.Cache[index2];

                var code1 = new char[f1.GAElement.BaseType.Length];
                var code2 = new char[f1.GAElement.BaseType.Length];
                var cutIndex = cutIndexTask.Result;

                //copy first part of elems to code elems
                Buffer.BlockCopy(f1.GAElement.BaseType, 0, code2, 0, cutIndex * sizeof(char));
                Buffer.BlockCopy(f2.GAElement.BaseType, 0, code1, 0, cutIndex * sizeof(char));
                //copy second part of elems to code elems
                Buffer.BlockCopy(f1.GAElement.BaseType, cutIndex * sizeof(char), code1, cutIndex * sizeof(char), ( code1.Length - cutIndex ) * sizeof(char));
                Buffer.BlockCopy(f2.GAElement.BaseType, cutIndex * sizeof(char), code2, cutIndex * sizeof(char), ( code2.Length - cutIndex ) * sizeof(char));

                f1.GAElement.BaseType = code1;
                var f1Fitness = f1.GAElement.CalculateFitnessAsync(GA.GAC.Sequence);
                f2.GAElement.BaseType = code2;
                var f2Fitness = f2.GAElement.CalculateFitnessAsync(GA.GAC.Sequence);
                f1.Fitness = f1Fitness.Result;
                f2.Fitness = f2Fitness.Result;
            }
        }
    }
}