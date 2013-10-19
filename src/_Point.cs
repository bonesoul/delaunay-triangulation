using System;
using System.Collections.Generic;
using System.Text;

namespace delaunay
{

    public enum CLASSIFY
    {
        LEFT,
        RIGHT,
        BEYOND,
        BEHIND,
        BETWEEN,
        ORIGIN,
        DESTINATION
    }


    public class _Point
    {
        public double X;
        public double Y;

        public _Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double lenght()
        {
            return Math.Sqrt((this.X * this.X) + (this.Y * this.Y));
        }

        public static double dot_product(_Point p, _Point q)
        {
            return (p.X * q.X + p.Y * q.Y);
        }

        public CLASSIFY classify(_Point p0, _Point p1)
        {
            _Point p2 = this;
            _Point a = new _Point(p1.X - p0.X, p1.Y - p0.Y);
            _Point b = new _Point(p2.X - p0.X, p2.Y - p0.Y);

            double sa = a.X * b.Y - b.X * a.Y;

            if (sa > 0.0)
                return CLASSIFY.LEFT;
            if (sa < 0.0)
                return CLASSIFY.RIGHT;
            if ((a.X * b.X < 0.0) || (a.Y * b.Y < 0.0))
                return CLASSIFY.BEHIND;
            if (a.lenght() < b.lenght())
                return CLASSIFY.BEYOND;
            if (p0 == p2)
                return CLASSIFY.ORIGIN;
            if (p1 == p2)
                return CLASSIFY.DESTINATION;

            return CLASSIFY.BETWEEN;
        }
    }
}
