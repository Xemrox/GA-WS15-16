using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XAnalysis {

    internal class Program {

        private struct DataTouple {
            public double CrossOver { get; set; }
            public double Mutate { get; set; }

            //public List<int> Fitness { get; set; }
            public int Fitness { get; set; }

            public override int GetHashCode() {
                int hash = 17;
                hash = hash * 23 + CrossOver.GetHashCode();
                hash = hash * 23 + Mutate.GetHashCode();
                return hash;
            }

            public override bool Equals(object obj) {
                return ( (DataTouple) obj ).CrossOver == ( this.CrossOver ) &&
                    ( (DataTouple) obj ).Mutate == ( this.Mutate );
            }
        }

        private struct DataGroup {
            public double Crossover { get; internal set; }

            public List<DataTouple> items {
                get; set;
            }

            public Object key { get; set; }
            public double Mutate { get; internal set; }
        }

        private static void Main(string[] args) {
            var rawData = new List<DataTouple>();

            Console.WriteLine("Analyzing data");
            int iFiles = 0;

            var zipFolder = new DirectoryInfo("data");
            foreach (var zFile in zipFolder.EnumerateFiles()) {
                using (ZipFile zip = ZipFile.Read(zFile.FullName)) {
                    foreach (var e in zip.Entries) {
                        var f = e.FileName.Split('-');
                        using (var ms = new MemoryStream()) {
                            e.Extract(ms);
                            ms.Position = 0;
                            using (var s = new StreamReader(ms)) {
                                var raw = new DataTouple { CrossOver = double.Parse(f[2]), Mutate = double.Parse(f[3]) };
                                var sb = new StringBuilder();
                                string line;
                                while (( line = s.ReadLine() ) != null) {
                                    /*if (raw.Fitness == null) {
                                        raw.Fitness = new List<int>();
                                    }*/

                                    if (line.StartsWith("F")) {
                                        //raw.Fitness.Add(int.Parse(line.Split(' ')[1]));
                                        raw.Fitness = int.Parse(line.Split(' ')[1]);
                                        break;
                                    }
                                }
                                //raw.data = sb.ToString();
                                rawData.Add(raw);
                            }
                        }
                        //Console.WriteLine(e.FileName);
                    }
                    Console.WriteLine("{0}: {1}", zFile.Name, zip.Entries.Count);
                    iFiles += zip.Entries.Count;
                }
            }

            Console.WriteLine("Got {0} entrys", iFiles);

            var group = rawData.GroupBy(x => new { x.CrossOver, x.Mutate }).Select(x => new DataGroup { key = x.Key, Crossover = x.Key.CrossOver, Mutate = x.Key.Mutate, items = x.ToList() });
            var map = new Dictionary<double, Dictionary<double, int>>();
            foreach (var g in group) {
                //Console.WriteLine("{0}-{1}", g.key, g.items.Select(x => x.Fitness).Median());
                Dictionary<double, int> cont;
                if (map.TryGetValue(g.Crossover, out cont)) {
                    cont.Add(g.Mutate, g.items.Select(x => x.Fitness).Median());
                } else {
                    var inner = new Dictionary<double, int>();
                    inner.Add(g.Mutate, g.items.Select(x => x.Fitness).Median());
                    map.Add(g.Crossover, inner);
                }
            }
            group = null;

            using (var wr = new StreamWriter("output.txt")) {
                var line = new StringBuilder();
                /*for (int i = 1; i <= 100; i++) {
                    line.Append(' ');
                    line.Append(i);
                }
                wr.WriteLine(line.ToString());
                line.Clear();*/
                for (double mut = 0.01; mut <= 1.01; mut += 0.01) {
                    //line.Append((int) Math.Round(mut * 100));
                    for (double cross = 0.01; cross <= 1.01; cross += 0.01) {
                        line.Append(' ');
                        line.Append(map[Math.Round(mut, 2)][Math.Round(cross, 2)]);
                    }
                    wr.WriteLine(line.ToString());
                    line.Clear();
                }
                wr.Flush();
            }

            /*foreach (var d in rawData) {
                Console.WriteLine("{0}-{1}", d.CrossOver, d.Mutate);
            }*/

            Console.ReadKey();
        }
    }
}