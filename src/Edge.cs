using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace delaunay
{
    public enum INTERSECT
    {
        COLLINEAR,
        PARALLEL,
        SKEW,
        SKEW_CROSS,
        SKEWE_NO_CROSS
    }

    public class Edge
    {
        public _Point org;
        public _Point dest;

        public Edge(_Point p1, _Point p2)
        {
            org = new _Point(p1.X, p1.Y);
            dest = new _Point(p2.X, p2.Y);
        }

        public Edge(Edge e)
        {
            org = new _Point(e.org.X, e.org.Y);
            dest = new _Point(e.dest.X, e.dest.Y);
        }

        public void rot()
        {
            _Point m = new _Point((org.X + dest.X) * 0.5, (org.Y + dest.Y) * 0.5);
            _Point v = new _Point(dest.X - org.X, dest.Y - org.Y);
            _Point n = new _Point(v.Y, -v.X);
            org.X = m.X - 0.5 * n.X;
            org.Y = m.Y - 0.5 * n.Y;
            dest.X = m.X + 0.5 * n.X;
            dest.Y = m.Y + 0.5 * n.Y;
        }

        public void flip()
        {
            this.rot();
            this.rot();
        }

        public INTERSECT intersect(Edge e, ref double t)
        {
            _Point a = org;
            _Point b = dest;
            _Point c = e.org;
            _Point d = e.dest;
            _Point n = new _Point((d.Y - c.Y), (c.X - d.X));
            double denom = _Point.dot_product(n, new _Point(b.X - a.X, b.Y - a.Y));
            if (denom == 0.0)
            {
                _Point p = org;
                CLASSIFY cf = p.classify(e.org, e.dest);
                if ((cf == CLASSIFY.LEFT) || (cf == CLASSIFY.RIGHT))
                    return INTERSECT.PARALLEL;
                else
                    return INTERSECT.COLLINEAR;
            }
            double num = _Point.dot_product(n, new _Point(a.X - c.X, a.Y - c.Y));
            t = -num / denom;
            return INTERSECT.SKEW;
        }

        public static _Point intersect_point(Edge e1, Edge e2)
        {
            double x1, x2, x3, x4, y1, y2, y3, y4,ua,ub,x,y;
            x1 = e1.org.X;
            y1 = e1.org.Y;
            x2 = e1.dest.X;
            y2 = e1.dest.Y;
            x3 = e2.org.X;
            y3 = e2.org.Y;
            x4 = e2.dest.X;
            y4 = e2.dest.Y;

            ua = ((x4 - x3)*(y1 - y3) - (y4 - y3)*(x1 - x3)) / ((y4 - y3)*(x2 - x1) - (x4 - x3)*(y2 - y1));
            ub = ((x2 - x1)*(y1 - y3) - (y2 - y1)*(x1 - x3)) / ((y4 - y3)*(x2 - x1) - (x4 - x3)*(y2 - y1));

            x = x1 + ua * (x2 - x1);
            y = y1 + ua * (y2 - y1);
            return new _Point(x, y);
        }

        public double lenght()
        {
            return Math.Sqrt(((dest.X - org.X) * (dest.X - org.X)) + ((dest.Y - org.Y) * (dest.Y - org.Y)));

        }

        public static Edge hull_edge(Vertices v_list)
        {
            int m = 0;
            _Point tmp;
            for (int i = 1; i < v_list.vertice_list.Count; i++)
            {
                if (v_list.vertice_list[i].X < v_list.vertice_list[m].X)
                {
                    m = i;
                }
            }

            // swap point[0] with point[m]
            tmp = v_list.vertice_list[0];
            v_list.vertice_list[0] = v_list.vertice_list[m];
            v_list.vertice_list[m] = tmp;

            m = 1;
            for (int i = 2; i < v_list.vertice_list.Count; i++)
            {
                CLASSIFY c = v_list.vertice_list[i].classify(v_list.vertice_list[0], v_list.vertice_list[m]);
                if ((c == CLASSIFY.LEFT) || (c == CLASSIFY.BETWEEN))
                    m = i;
            }

            return new Edge(v_list.vertice_list[0], v_list.vertice_list[m]);
        }
    }

}