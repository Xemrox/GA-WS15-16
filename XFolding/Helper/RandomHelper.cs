﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;

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

        public static IEnumerable<double> GetNextDoubleList(int size) {
            for (int i = 0; i < size; i++) {
                yield return GetNextDouble();
            }
        }

        public static Task<double> GetAsyncNextDouble() {
            return Task.Run(() => GetNextDouble());
        }

        public static int GetNextInteger(int maxValue) {
            return (int) Math.Floor(GetNextDouble() * maxValue);
        }

        public static Task<int> GetAsyncNextInteger(int maxValue) {
            return Task.Run(() => GetNextInteger(maxValue));
        }

        public static int PoissonInt(double ex) {
            var x = 0;
            var lambda = ex;
            var p = Math.Pow(Math.E, -( lambda ));
            var s = p;
            var u = GetNextDouble();

            while (u > s) {
                x++;
                p = p * lambda / x;
                s += p;
            }
            return x;
        }

        public static double ExponentialDouble(double ex) {
            return Math.Log(1 - GetNextDouble()) / ( -1 / ex );
        }

        public static int ExponentialInt(double ex) {
            return (int) Math.Floor(ExponentialDouble(ex));
        }
    }
}