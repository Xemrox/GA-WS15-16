using System;
using XGA.Helper;

namespace XGA.Config {

    public interface IFitnessMeasured<T> : ICloneable {

        double CalculateFitness(T[] reference);

        void print(T[] reference, Logger log);

        T[] GenerateRandom(int length);

        T[] BaseType { get; set; }
    }
}