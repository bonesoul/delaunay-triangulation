using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;


namespace delaunay
{
    public class Vertices
    {
        public List<_Point> vertice_list;

        public Vertices(int num_vertices,int plane_x, int plane_y)
        {
            _Point p;
            double x=0,y=0;
            Random rnd = new Random();
            vertice_list = new List<_Point>();            

            for(int i=0;i<num_vertices;i++)
            {
                x=(double)rnd.Next(plane_x);
                y=(double)rnd.Next(plane_y);
                p = new _Point(x, y);
                vertice_list.Add(p);
            }

        }

        public Vertices(Vertices v)
        {
            vertice_list = new List<_Point>();  
            for (int i = 0; i < v.vertice_list.Count; i++)
                vertice_list.Add(v.vertice_list[i]);
        }

        public Vertices()
        {
            vertice_list = new List<_Point>();
        }
    }
}
