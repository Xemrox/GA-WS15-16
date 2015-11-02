using System.Collections.Generic;
using System.Linq;
using XGA.Config;
using XGA.Helper;

namespace XGA {

    public class GeneticAlgorithm<T, S> where T : IFitnessMeasured<S>, new() {

        public class GACache {

            public GACache(int length) {
                this.Fitness = 0.0;
                this.GAElement = new T();
                this.GAElement.BaseType = this.GAElement.GenerateRandom(length);
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
                return Fitness.Average();
            }
        }

        public double TotalFitness {
            get {
                return Fitness.Sum();
            }
        }

        public GeneticAlgorithmConfig GAC { get; private set; }
        public int CurrentGeneration { get; set; }

        public IEnumerable<IGeneticOperator<T, S>> Operators { get; private set; }

        private Logger Log { get; set; }

        public GeneticAlgorithm(GeneticAlgorithmConfig gac, IGeneticOperatorProvider<T, S> OperatorProvider, Logger log) {
            this.Log = log;
            this.CurrentGeneration = 0;
            this.Operators = OperatorProvider.GetOperators();

            this.GAC = gac;

            this.Cache = new GACache[this.GAC.PopulationSize];
            var SequenceLength = this.GAC.Sequence.Length;
            for (int i = 0; i < this.GAC.PopulationSize; i++) {
                this.Cache[i] = new GACache(SequenceLength);
            }
        }

        public void GenerationStep() {
            foreach (var op in this.Operators) {
                op.Operate(this, null);
            }

            this.CurrentGeneration++;
        }

        /*
        private void Select() {
            var TotalFitness = this.TotalFitness;
            var relFitness = this.Fitness.Select(x => x / TotalFitness).ToArray();

            var selAreas = new double[Size];
            var usedArea = 0.0d;
            for (int i = 0; i < Size; i++) {
                var relUsed = relFitness[i] * Size;
                selAreas[i] = usedArea + relUsed;
                usedArea += relUsed;
            }

            ///TODO
            /// replace with binsearch

            var Selection = new List<CacheFold>();

            for (int i = 0; i < Size; i++) {
                double index = RNG.GetNext() * Size;
                int iIndex = Size - 1;

                /*for (int a = 0; a < Size - 1; a++) {
                    if (selAreas[a] > index) {
                        iIndex = a;
                        break;
                    }
                }*/

        /*iIndex = -( Array.BinarySearch(selAreas, index) + 1 );

        var nEntry = new CacheFold {
            Fitness = Cache[iIndex].Fitness,
            Folding = new Folding(Cache[iIndex].Folding)
        };

        Selection.Add(nEntry);
    }

    Cache = Selection.ToArray();
}

private void CrossOver() {
    int iHits = 0;
    for (int i = 0; i < Size; i++) {
        if (RNG.GetNext() <= this.CrossoverRate) {
            iHits += 1;
        }
    }

    if (iHits % 2 == 1) {
        iHits--;
    }

    for (int i = 0; i < iHits; i += 2) {
        var index1 = (int) Math.Floor(RNG.GetNext() * ( Size - 1 ));
        var index2 = (int) Math.Floor(RNG.GetNext() * ( Size - 1 ));

        var f1 = Cache[index1].Folding;
        var f2 = Cache[index2].Folding;

        var cutIndex = (int) Math.Floor(RNG.GetNext() * ( f1.foldSeq.Length - 2 )) + 1;
        var left1 = f2.foldSeq.ToCharArray(0, cutIndex);
        var left2 = f1.foldSeq.ToCharArray(0, cutIndex);

        left1 = left1.Concat(f1.foldSeq.ToCharArray(cutIndex, f1.foldSeq.Length - cutIndex)).ToArray();
        left2 = left2.Concat(f2.foldSeq.ToCharArray(cutIndex, f2.foldSeq.Length - cutIndex)).ToArray();

        f1.foldSeq = new string(left1);
        f2.foldSeq = new string(left2);
        Cache[index1].Folding = new Folding(f1);
        Cache[index1].Fitness = f1.CalculateFitness(this.Sequence);

        Cache[index2].Folding = new Folding(f2);
        Cache[index2].Fitness = f2.CalculateFitness(this.Sequence);
    }
}

private static Dictionary<char, char[]> MutationTable = new Dictionary<char, char[]>();

static GeneticAlgorithm() {
    MutationTable.Add('F', new char[] { 'R', 'L' });
    MutationTable.Add('L', new char[] { 'R', 'F' });
    MutationTable.Add('R', new char[] { 'F', 'L' });
}

private void Mutate() {
    int iHits = 0;
    for (int i = 0; i < Size; i++) {
        if (RNG.GetNext() <= this.MutationRate) {
            iHits += 1;
        }
    }

    for (int i = 0; i < iHits; i++) {
        var iIndex = (int) Math.Floor(RNG.GetNext() * ( Size - 1 ));
        var f = Cache[iIndex].Folding;

        var fIndex = (int) Math.Floor(RNG.GetNext() * ( f.foldSeq.Length - 1 ));
        var foldChars = f.foldSeq.ToCharArray();
        foldChars[fIndex] = MutationTable[foldChars[fIndex]][(int) Math.Round(RNG.GetNext())];
        f.foldSeq = new string(foldChars);

        Cache[iIndex] = new CacheFold {
            Folding = f,
            Fitness = f.CalculateFitness(this.Sequence)
        };
    }
}
*/
    }
}