using System;
using System.Text;
using XGA.Config;
using XGA.Helper;

namespace XGA.Folding {

    public class FoldingSelectOperator : IGeneticSelect<Folding, string> {

        void IGeneticOperator<Folding, string>.Operate(GeneticAlgorithm<Folding, string> GA, Logger LOG) {
            throw new NotImplementedException();
        }
    }

    public class FoldingMutateOperator : IGeneticMutate<Folding, string> {

        void IGeneticOperator<Folding, string>.Operate(GeneticAlgorithm<Folding, string> GA, Logger LOG) {
            throw new NotImplementedException();
        }
    }

    public class FoldingCrossoverOperator : IGeneticCrossover<Folding, string> {

        void IGeneticOperator<Folding, string>.Operate(GeneticAlgorithm<Folding, string> GA, Logger LOG) {
            int iHits = 0;
            for (int i = 0; i < GA.GAC.PopulationSize; i++) {
                if (RandomHelper.GetNextDouble() <= GA.GAC.CrossoverRate) {
                    iHits += 1;
                }
            }

            if (iHits % 2 == 1) {
                iHits--;
            }

            for (int i = 0; i < iHits; i += 2) {
                var index1 = RandomHelper.GetNextInteger(GA.GAC.PopulationSize - 1);
                var index2 = RandomHelper.GetNextInteger(GA.GAC.PopulationSize - 1);

                var f1 = GA.Cache[index1].GAElement;
                var f2 = GA.Cache[index2].GAElement;

                var cutIndex = RandomHelper.GetNextInteger(f1.BaseType.Length - 1) + 1;//(int) Math.Floor(RNG.GetNext() * ( f1.foldSeq.Length - 2 )) + 1;

                var code1 = new StringBuilder();
                var code2 = new StringBuilder();
                code1.Append(f2.BaseType.Substring(0, cutIndex)).Append(f1.BaseType.Substring(cutIndex));
                code2.Append(f1.BaseType.Substring(0, cutIndex)).Append(f2.BaseType.Substring(cutIndex));

                GA.Cache[index1].GAElement.BaseType = code1.ToString();
                GA.Cache[index1].Fitness = f1.CalculateFitness(GA.GAC.Sequence);

                GA.Cache[index2].GAElement.BaseType = code2.ToString();
                GA.Cache[index2].Fitness = f2.CalculateFitness(GA.GAC.Sequence);
            }
        }
    }
}