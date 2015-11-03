using System;
using System.Security.Cryptography;

namespace XGA {

    public static class RandomHelper {

        // Step 1: fill an array with 8 random bytes
        private static RNGCryptoServiceProvider gen = new RNGCryptoServiceProvider();

        public static double GetNextDouble() {
            var bytes = new Byte[8];
            gen.GetBytes(bytes);
            // Step 2: bit-shift 11 and 53 based on double's mantissa bits
            var ul = BitConverter.ToUInt64(bytes, 0) / ( 1 << 11 );
            return ul / (double) ( 1UL << 53 );
        }

        public static int GetNextInteger(int maxValue) {
            return (int) Math.Floor(GetNextDouble() * maxValue);
        }

        public static double Exponential(double ex) {
            return Math.Log(1 - GetNextDouble()) / ( -ex );
        }
    }
}