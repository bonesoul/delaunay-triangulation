using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace delaunay
{
    class Delaunay
    {
        public List<Triangle> triangles;
        public List<Edge> frontier;
        public List<_Point> convex_hull;
        public Graphics g;
        private Vertices verts;
        public Boolean use_alternative = false;
        public int panel_width;
        public int panel_height;


        public void draw_points()
        {
            for (int i = 0; i < verts.vertice_list.Count; i++)
            {
                try
                {
                    g.FillEllipse(Brushes.Blue, (float)verts.vertice_list[i].X - 3, (float)verts.vertice_list[i].Y - 3, 6, 6);
                }
                catch (Exception e)
                {
                }
                finally
                {

                }
            }
        }

        public void draw_edge(Edge e)
        {
            g.Clear(Color.White);
            draw_points();
            draw_triangles();
            g.DrawLine(new Pen(Brushes.Red, 3), (float)e.org.X, (float)e.org.Y, (float)e.dest.X, (float)e.dest.Y);
            Application.DoEvents();
            System.Threading.Thread.Sleep(50);
        }

        public void draw_triangles()
        {
            for (int i = 0; i < triangles.Count; i++)
            {
                Point[] pnts = new Point[3];
                for (int j = 0; j < 3; j++)
                {
                    pnts[j].X = (int)triangles[i].points[j].X;
                    pnts[j].Y = (int)triangles[i].points[j].Y;
                }
                g.FillPolygon(Brushes.LightSteelBlue, pnts);
                g.DrawPolygon(new Pen(Brushes.Black,1), pnts);
            }
            Application.DoEvents();
            System.Threading.Thread.Sleep(50);
        }

        public void draw_circle(_Point center, double r)
        {
            g.DrawEllipse(new Pen(Brushes.Green,2), (float)(center.X-r/2), (float)(center.Y-r/2), (float)r, (float)r);
            Application.DoEvents();
            System.Threading.Thread.Sleep(50);
        }

        public _Point find_next_mate(Edge e, ref Vertices v)
        {
            _Point best_point=null;
            _Point intersect;
            _Point next_mate=null;
            double radius;
            Edge g;
            Edge f = new Edge(e);
            Edge radius_edge;
            if (mate(e, v, ref best_point) == true)
            {
                f.rot();
                g = new Edge(e.dest, best_point);
                g.rot();

                // find the circle
                intersect = Edge.intersect_point(f, g);
                radius_edge = new Edge(intersect, best_point);
                radius = radius_edge.lenght() * 2;

                // find a point on C which is visible in view panel
                //(x - h)^2 + (y - k)^2 = r^2
                // find maximum possible y value
                double min_y = intersect.Y - radius/2;
                double max_y = intersect.Y + radius/2;

                for (int y = (int)min_y; y<(int)max_y; y++)
                {
                    double x_h = Math.Sqrt(((radius/2) * (radius/2)) - ((y - intersect.Y) * (y - intersect.Y)));
                    double x = x_h + intersect.X;
                    x = (double)(int)x;;
                    if (( (x> 0) &&(x < panel_width)) && ( y > 0  && y < panel_height)) // panel limits
                    {
                        if ( Math.Abs(x - best_point.X)>50 && Math.Abs( y - best_point.Y) >50)
                        {
                            next_mate = new _Point(x, y);
                            v.vertice_list.Add(next_mate);
                            if (use_alternative)
                                if(is_in_hull(best_point)==false)
                                    v.vertice_list.Remove(best_point);
                            break;
                        }
                    }
                }
            }

            return next_mate;
        }

        public void look_ahead(ref Vertices v, int count)
        {
            List<Edge> frontier = new List<Edge>();
            _Point pt=null;
            Edge e = Edge.hull_edge(v);
            frontier.Add(e);

            while (frontier.Count != 0 && count!=0)
            {
                e = Delaunay.remove_min(ref frontier);
                find_next_mate(e,ref v);
                if (mate(e, v, ref pt))
                {
                    update_frontier(ref frontier, pt, e.org);
                    update_frontier(ref frontier, e.dest, pt);
                }
                count--;
            }

        }

        public static List<_Point> gift_wrap(Vertices v)
        {
            List<_Point> hull = new List<_Point>();

            int a, i;
            _Point tmp;
            for (a=0,i = 1; i < v.vertice_list.Count; i++)
                if (v.vertice_list[i].X < v.vertice_list[a].X)
                    a = i;

            v.vertice_list[v.vertice_list.Count-1] = v.vertice_list[a];

            for (int m = 0; m < v.vertice_list.Count; m++)
            {
                // swap point[a] with point[m]
                tmp = v.vertice_list[a];
                v.vertice_list[a] = v.vertice_list[m];
                v.vertice_list[m] = tmp;
                hull.Add(v.vertice_list[m]);

                a = m + 1;

                for (i = m + 2; i <= v.vertice_list.Count-1; i++)
                {
                    CLASSIFY c = v.vertice_list[i].classify(v.vertice_list[m], v.vertice_list[a]);
                    if (c == CLASSIFY.LEFT || c == CLASSIFY.BEYOND)
                        a = i;
                }

                if(a==v.vertice_list.Count-1)
                    return hull;
            }

            return null;
        }

        public Boolean is_in_hull(_Point p)
        {
            for (int i = 0; i < convex_hull.Count; i++)
            {
                if (convex_hull[i].X == p.X && convex_hull[i].Y == p.Y)
                    return true;
            }

            return false;
        }

        public void triangulate(object param)
        {
            Vertices v = new Vertices((Vertices)param);
            _Point found_point=null;
            _Point choosen_point = null;
            triangles = new List<Triangle>();
            frontier = new List<Edge>();
            //debug = new List<object>();
            verts = v;

            draw_points();

            look_ahead(ref v,v.vertice_list.Count);

            Edge e = Edge.hull_edge(v);

            frontier.Add(e);

            while (frontier.Count != 0)
            {
                e = Delaunay.remove_min(ref frontier);

                if (mate(e, v, ref found_point))
                {

                    update_frontier(ref frontier, found_point, e.org);
                    update_frontier(ref frontier, e.dest, found_point);

                    triangles.Add(new Triangle(e.org, e.dest, found_point));

                    draw_triangles();
                }
            }
        }

        public void update_frontier(ref List<Edge> frontier,_Point a,_Point b)
        {
            Edge e;
            Boolean found = false;

            for (int i = 0; i < frontier.Count; i++)
            {
                if ((frontier[i].org.X == a.X) && (frontier[i].org.Y == a.Y))
                {
                    if ((frontier[i].dest.X == b.X) && (frontier[i].dest.Y == b.Y))
                    {
                        e = frontier[i];
                        frontier.Remove(e);
                        found = true;
                        break;
                    }
                }
            }
            if (found == false)
            {
                e = new Edge(a, b);
                e.flip();
                frontier.Add(e);
            }
        }

        public static Edge remove_min(ref List<Edge> l)
        {
            Edge e;
            e = l[0];

            for (int i = 1; i < l.Count; i++)
            {
                if (cmp_edge(l[i], e) < 0)
                {
                    e = l[i];
                }
            }

            l.Remove(e);
            return e;
        }

        public static int cmp_edge(Edge a, Edge b)
        {
            if ( (a.org.X<b.org.X) && (a.org.Y<b.org.Y) ) 
                return -1;
            if ( (a.org.X > b.org.X) && (a.org.Y > b.org.Y) )
                return 1;
            if ( (a.dest.X < b.dest.X) && (a.dest.Y < b.dest.Y) )
                return -1;
            if ((a.dest.X > b.dest.X) && (a.dest.Y > b.dest.Y))
                return 1;
            return 0;
        }

        public Boolean mate(Edge e, Vertices v, ref _Point p)
        {
            _Point best_point=null;
            double t=0, bestt = float.MaxValue;
            Edge f = new Edge(e);

            f.rot();
            for (int i = 0; i < v.vertice_list.Count; i++)
            {
                if (v.vertice_list[i].classify(e.org, e.dest) == CLASSIFY.RIGHT)
                {
                    Edge g = new Edge(e.dest, v.vertice_list[i]);
                    g.rot();
                    f.intersect(g, ref t);

                    if (t < bestt)
                    {
                        best_point = v.vertice_list[i];
                        bestt = t;
                    }
                }

            }
            if (best_point != null)
            {
                p = best_point;
                return true;
            }
            return false;
        }

    }
}

     