using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XGA.Config;

namespace XGA.Folding {

    public class FoldingCreator : IFitnessMeasuredCreator<char> {

        public IFitnessMeasured<char> CreateNew(GeneticAlgorithmConfig<char> GAC) {
            return new Folding(GAC.Sequence.Length - 1);
        }
    }
}