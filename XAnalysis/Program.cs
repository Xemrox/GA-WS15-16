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
            public string CrossOver { get; set; }
            public string Mutate { get; set; }

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
            public string Crossover { get; set; }

            public List<DataTouple> items {
                get; set;
            }

            public Object key { get; set; }
            public string Mutate { get; set; }
        }

        private static void AnalyzeZip() {
            Console.WriteLine("Analyzing data");
            int iFiles = 0;

            var zipFolder = new DirectoryInfo("data");
            List<Task> tasks = new List<Task>();
            foreach (var zFile in zipFolder.EnumerateFiles("*.zip")) {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    var rawData = new List<DataTouple>();
                    using (ZipFile zip = ZipFile.Read(zFile.FullName)) {
                        foreach (var e in zip.Entries) {
                            var f = e.FileName.Split('-');
                            using (var ms = new MemoryStream()) {
                                e.Extract(ms);
                                ms.Position = 0;
                                using (var s = new StreamReader(ms)) {
                                    var raw = new DataTouple { CrossOver = f[2].Replace(',', '.'), Mutate = f[3].Replace(',', '.') };
                                    var sb = new StringBuilder();
                                    string line;
                                    while (( line = s.ReadLine() ) != null) {
                                        if (line.StartsWith("F")) {
                                            raw.Fitness = int.Parse(line.Split(' ')[1]);
                                            break;
                                        }
                                    }
                                    rawData.Add(raw);
                                }
                            }
                            //Console.WriteLine(e.FileName);
                        }
                        Console.WriteLine("{0}: {1}", zFile.Name, zip.Entries.Count);
                        iFiles += zip.Entries.Count;
                    }

                    var group = rawData.GroupBy(x => new { x.CrossOver, x.Mutate }).Select(x => new DataGroup { key = x.Key, Crossover = x.Key.CrossOver, Mutate = x.Key.Mutate, items = x.ToList() });
                    using (var wr = new StreamWriter("data\\" + zFile.Name + ".txt")) {
                        foreach (var g in group) {
                            foreach (var itm in g.items) {
                                wr.WriteLine("{0}|{1}-{2}", g.Crossover, g.Mutate, itm.Fitness);
                            }
                        }
                        wr.Flush();
                        wr.Close();
                    }
                }));
            }
            Task.WaitAll(tasks.ToArray());

            Console.WriteLine("Got {0} entrys", iFiles);
        }

        private static void PlotData(Dictionary<string, List<int>> plots, Func<IEnumerable<int>, double> OP, string type) {
            var stats = plots.Select(x => new { x.Key, Val = OP(x.Value) });

            var max = stats.Max(x => x.Val);
            var EPSILON = 0.01d;
            var maxVals = stats.Where(x => System.Math.Abs(x.Val - max) < EPSILON);
            Console.WriteLine("{0}:", type);
            foreach (var m in maxVals) {
                Console.WriteLine("{0}: {1}", m.Key, m.Val);
            }

            string cur = null;
            bool bBreak = true;

            int root = (int) Math.Sqrt(stats.Count());

            //Console.WriteLine("Got: {0} stats rooted: {1}", stats.Count(), root);

            using (var wr = new StreamWriter(string.Format("data\\output-{0}.txt", type))) {
                wr.Write("0 ".Repeat(root));
                wr.WriteLine("0");
                foreach (var elem in stats) {
                    var f = elem.Key.Split('|')[0];
                    if (cur == null) {
                        cur = f;
                        wr.Write("0 ");
                    } else if (f != cur) {
                        cur = f;
                        wr.WriteLine();
                        wr.Write("0 ");
                        bBreak = true;
                    }
                    if (bBreak) {
                        wr.Write("{0}", elem.Val);
                        bBreak = false;
                    } else {
                        wr.Write(" {0}", elem.Val);
                    }
                }
                wr.Flush();
                wr.Close();
            }

            var ifo = new System.Diagnostics.ProcessStartInfo("C:\\gnuplot\\gnuplot.exe", string.Format("-e \"infile='data\\output-{0}.txt'; outfile='Heat{0}.png'; name='{0}'\" heat.plot", type));
            ifo.CreateNoWindow = true;
            ifo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            System.Diagnostics.Process.Start(ifo);

            Console.WriteLine("Generated plotdata");
        }

        private static void Main(string[] args) {
            AnalyzeZip();

            Console.WriteLine("Plotting data");
            var runPlots = new DirectoryInfo("data").GetFiles("run*.zip.txt").Select(x => x.FullName);
            var plots = new Dictionary<string, List<int>>();
            foreach (var run in runPlots) {
                using (var r = new StreamReader(run)) {
                    string line;
                    while (( line = r.ReadLine() ) != null) {
                        var parts = line.Split('-');
                        if (parts.Length > 0) {
                            List<int> itms;
                            if (plots.TryGetValue(parts[0], out itms)) {
                                itms.Add(int.Parse(parts[1]));
                            } else {
                                itms = new List<int>();
                                itms.Add(int.Parse(parts[1]));
                                plots.Add(parts[0], itms);
                            }
                        } else {
                            break;
                        }
                    }
                    r.Close();
                }
            }

            Console.WriteLine("Got {0} plot entrys", plots.Count);

            PlotData(plots, x => x.OrderBy(y => y).Median(), "median");
            PlotData(plots, x => x.Max(), "max");
            //PlotData(plots, x => x.Min(), "min");
            PlotData(plots, x => x.Average(), "average");
            //PlotData(plots, x => x.Sum(), "sum");

            //System.Diagnostics.Process.Start("CreateMap.bat", "");

            Console.ReadKey();
        }
    }
}