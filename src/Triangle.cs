using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace delaunay
{
    class Triangle
    {
        public _Point[] points;

        public Triangle(_Point p1, _Point p2, _Point p3)
        {
            points = new _Point[3];
            points[0] = p1;
            points[1] = p2;
            points[2] = p3;
        }
    }
}
