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
            foreach (var zFile in zipFolder.EnumerateFiles()) {
                var rawData = new List<DataTouple>();
                using (ZipFile zip = ZipFile.Read(zFile.FullName)) {
                    foreach (var e in zip.Entries) {
                        var f = e.FileName.Split('-');
                        using (var ms = new MemoryStream()) {
                            e.Extract(ms);
                            ms.Position = 0;
                            using (var s = new StreamReader(ms)) {
                                var raw = new DataTouple { CrossOver = f[2], Mutate = f[3] };
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
                using (var wr = new StreamWriter(zFile.Name + ".txt")) {
                    foreach (var g in group) {
                        foreach (var itm in g.items) {
                            wr.WriteLine("{0}|{1}-{2}", g.Crossover, g.Mutate, itm.Fitness);
                        }
                    }
                    wr.Flush();
                    wr.Close();
                }
            }

            Console.WriteLine("Got {0} entrys", iFiles);
        }

        private static void Main(string[] args) {
            AnalyzeZip();

            Console.WriteLine("Plotting data");
            var runPlots = new DirectoryInfo(".").GetFiles("run*.zip.txt").Select(x => x.FullName);
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

            var stats = plots.Select(x => new { x.Key, Median = x.Value.Median() });
            var max = stats.Max(x => x.Median);
            var maxMeds = stats.Where(x => x.Median == max);
            Console.WriteLine("Maximum Medians:");
            foreach (var m in maxMeds) {
                Console.WriteLine("{0}: {1}", m.Key, m.Median);
            }

            string cur = null;
            bool bBreak = true;
            using (var wr = new StreamWriter("output.txt")) {
                wr.Write("0 ".Repeat((int) Math.Sqrt(stats.Count())));
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
                        wr.Write("{0}", elem.Median);
                        bBreak = false;
                    } else {
                        wr.Write(" {0}", elem.Median);
                    }
                }
                wr.Flush();
                wr.Close();
            }

            Console.WriteLine("Generated plotdata");

            System.Diagnostics.Process.Start("CreateMap.bat", "");

            Console.ReadKey();
        }
    }
}