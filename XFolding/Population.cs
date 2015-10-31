using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
namespace Folding {

    public static class RNG {

        // Step 1: fill an array with 8 random bytes
        private static RNGCryptoServiceProvider gen = new RNGCryptoServiceProvider();

        public static double GetNext() {
            var bytes = new Byte[8];
            gen.GetBytes(bytes);
            // Step 2: bit-shift 11 and 53 based on double's mantissa bits
            var ul = BitConverter.ToUInt64(bytes, 0) / ( 1 << 11 );
            return ul / (double) ( 1UL << 53 );
        }

       
    }

    public class Population {

        public struct CacheFold {
            public Folding Folding { get; set; }
            public double Fitness { get; set; }
        }

        public int Size { get; private set; }

        public CacheFold[] Cache { get; set; }

        public Folding[] Foldings {
            get {
                return Cache.Select(x => x.Folding).ToArray();
            }
        }

        public double[] Fitness {
            get {
                return Cache.Select(x => x.Fitness).ToArray();
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

        public double MutationRate { get; private set; }
        public double CrossoverRate { get; private set; }
        public int Generations { get; private set; }
        public int CurrentGeneration { get; set; }

        public int LeftGenerations {
            get { return Generations - CurrentGeneration; }
        }

        public void Run() {
            while (this.LeftGenerations > 0)
                this.GenerationStep();
        }

        public string Sequence { get; private set; }

        public Population(string SEQ, int Elems = 100, int Generations = 50, double MutationRate = 0.01, double CrossoverRate = 0.25) {
            this.Size = Elems;
            this.Sequence = SEQ;
            this.CurrentGeneration = 0;
            this.Generations = Generations;
            this.MutationRate = MutationRate;
            this.CrossoverRate = CrossoverRate;

            this.Cache = new CacheFold[this.Size];

            int iFoldLength = SEQ.Length - 1;

            char[] ALP = { 'L', 'F', 'R' };

            for (int i = 0; i < this.Size; i++) {
                var sb = new StringBuilder();
                for (int l = 0; l < iFoldLength; l++) {
                    sb.Append(ALP[(int) Math.Floor(RNG.GetNext() * 3)]);
                }

                var f = new Folding(sb.ToString());
                Cache[i] = new CacheFold {
                    Folding = f,
                    Fitness = f.CalculateFitness(SEQ)
                };
            }
        }

        public void GenerationStep() {
            if (LeftGenerations <= 0) return;

            //Debugger.Log(0,"inf", Cache.GroupBy(x=> x.Folding.foldSeq).Count().ToString()+'\n');
            this.Select();

            for (var index = 0; index < Cache.Length; index++) {
                var c = Cache[index];
                if (c.Fitness != c.Folding.CalculateFitness(this.Sequence)) {
                    //cache integrity failed
                    Console.WriteLine("Cache integrity failed for: {0}", index);
                }
            }


            this.CrossOver();
            var count = 0;
            for (var index = 0; index < Cache.Length; index++) {
                var c = Cache[index];
                if (c.Fitness != c.Folding.CalculateFitness(this.Sequence)) {
                    //cache integrity failed
                    Console.WriteLine("Cache integrity failed for: {0} {1}-{2}", index, c.Fitness, c.Folding.CalculateFitness(this.Sequence));
                    count++;
                }
            }
            //Console.WriteLine(count);

            this.Mutate();

            this.CurrentGeneration++;
        }

        private static Random rng = new Random();

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

                iIndex = -( Array.BinarySearch(selAreas, index) + 1 );
                /*Console.WriteLine(iIndex);
                Console.WriteLine("----");*/
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

            //Console.WriteLine(iHits);

            for (int i = 0; i < iHits; i += 2) {
                var index1 = (int) Math.Floor(RNG.GetNext() * ( Size - 1 ));
                var index2 = (int) Math.Floor(RNG.GetNext() * ( Size - 1 ));

                //Console.WriteLine("{0}-{1}",index1, index2);
                
                var f1 = Cache[index1].Folding;
                var f2 = Cache[index2].Folding;

                var cutIndex = (int) Math.Floor(RNG.GetNext() * ( f1.foldSeq.Length - 2 ))+1;
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

                /*var f = Cache[iIndex].Folding;

                var fIndex = (int) Math.Floor(RNG.GetNext() * ( f.foldSeq.Length - 1 ));
                var foldChars = f.foldSeq.ToCharArray();
                foldChars[fIndex] = MutationTable[foldChars[fIndex]][(int) Math.Round(RNG.GetNext())];
                f.foldSeq = new string(foldChars);

                Cache[iIndex].Fitness = f.CalculateFitness(this.Sequence);*/
            }
        }

        private static Dictionary<char, char[]> MutationTable = new Dictionary<char, char[]>();

        static Population() {
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
                    Folding = new Folding(f),
                    Fitness = f.CalculateFitness(this.Sequence)
                };
                //Console.WriteLine("Replaced: {0}", iIndex);
            }

            //Console.WriteLine("Mutation {0}", iHits);
        }
    }
}