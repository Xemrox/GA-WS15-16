namespace XGA {

    public class GeneticAlgorithmConfig<T> {
        public int PopulationSize { get; set; }
        public double MutationRate { get; set; }
        public double CrossoverRate { get; set; }
        public T[] Sequence { get; set; }

        public GeneticAlgorithmConfig() {
            this.PopulationSize = 100;
            this.MutationRate = 0.01d;
            this.CrossoverRate = 0.25d;
        }
    }
}