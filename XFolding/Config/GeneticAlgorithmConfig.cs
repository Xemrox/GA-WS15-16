namespace XGA {

    public class GeneticAlgorithmConfig {
        public int PopulationSize { get; set; }
        public double MutationRate { get; set; }
        public double CrossoverRate { get; set; }
        public string Sequence { get; set; }

        public GeneticAlgorithmConfig() {
            this.PopulationSize = 100;
            this.MutationRate = 0.01;
            this.CrossoverRate = 0.25;
            this.Sequence = "";
        }
    }
}