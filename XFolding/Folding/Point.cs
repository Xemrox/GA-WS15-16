using System;

namespace XGA.Folding {

    public struct Point {
        public int X { get; set; }
        public int Y { get; set; }

        public int distance(Point point) {
            return (int) Math.Ceiling(Math.Sqrt(Math.Pow(point.X - this.X, 2) + Math.Pow(point.Y - this.Y, 2)));
        }
    }
}