using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace delaunay
{
    public partial class frm_main : Form
    {
        public Vertices v_list; /* vertices list */
        Delaunay d1,d2;
        Thread t_d1, t_d2;

        public frm_main()
        {
            InitializeComponent();
        }

        private void frm_main_Load(object sender, EventArgs e)
        {


        }


        private void cmd_delaunay_Click(object sender, EventArgs e)
        {
            v_list = new Vertices(Int32.Parse(numericUpDown1.Value.ToString()), panel1.Width, panel1.Height);
            List<_Point> hull = Delaunay.gift_wrap(v_list);
            
            d1 = new Delaunay();
            d1.g = panel1.CreateGraphics();
            d1.g.Clear(Color.White);
            d1.panel_width = panel1.Width;
            d1.panel_height = panel1.Height;
            d1.use_alternative = true;
            d1.convex_hull = hull;

            d2 = new Delaunay();
            d2.g = panel2.CreateGraphics();
            d2.g.Clear(Color.White);
            d2.panel_width = panel2.Width;
            d2.panel_height = panel2.Height;
            d2.use_alternative = false;
            d2.convex_hull = hull;



            t_d1 = new Thread(new ParameterizedThreadStart(d1.triangulate));
            t_d2 = new Thread(new ParameterizedThreadStart(d2.triangulate));

            t_d1.Start(v_list);
            t_d2.Start(v_list);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (t_d1.ThreadState == ThreadState.Running)
                t_d1.Abort();
            if (t_d2.ThreadState == ThreadState.Running)
                t_d2.Abort();

            

            cmd_delaunay_Click(this,null);
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            label1.Text = panel1.PointToClient(Cursor.Position).ToString();
        }

        Boolean triangles_equal(Triangle t1, Triangle t2)
        {
            _Point p;
            Boolean found = true;
            for (int i = 0; i < 3; i++)
            {
                p = t1.points[i];
                if (!((p.X == t2.points[0].X || p.X == t2.points[1].X || p.X == t2.points[2].X) && (p.Y == t2.points[0].Y || p.Y == t2.points[1].Y || p.Y == t2.points[2].Y)))
                {
                    found = false;
                }
            }
            return found;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Graphics g1 = panel1.CreateGraphics();
            Graphics g2 = panel2.CreateGraphics();
            g1.Clear(Color.White);
            g2.Clear(Color.White);
            colorize_triangles(d1.triangles, d2.triangles, d1.g);
            colorize_triangles(d2.triangles, d1.triangles, d2.g);


        }

        void colorize_triangles(List<Triangle> triangles, List<Triangle> compare,Graphics g)
        {
            Boolean equal;
            for (int i = 0; i < triangles.Count; i++)
            {
                equal = false;
                for (int j = 0; j < compare.Count; j++)
                {
                    if (triangles_equal(triangles[i], compare[j]) == true)
                    {
                        equal = true;
                        break;
                    }
                }

                // draw triangle
                Point[] pnts = new Point[3];
                for (int k = 0; k < 3; k++)
                {
                    pnts[k].X = (int)triangles[i].points[k].X;
                    pnts[k].Y = (int)triangles[i].points[k].Y;
                }

                if (equal == false)
                    g.FillPolygon(Brushes.Pink, pnts);
                else
                    g.FillPolygon(Brushes.GreenYellow, pnts);

                g.DrawPolygon(new Pen(Brushes.Black,1), pnts);
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (d1 != null && t_d1.ThreadState== ThreadState.Running )
            {
                try
                {
                    t_d1.Suspend();
                    d1.draw_points();
                    t_d1.Resume();
                }
                finally
                {

                }
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            if (d2 != null && t_d2.ThreadState == ThreadState.Running)
            {

                try
                {
                    t_d2.Suspend();
                    d2.draw_points();
                    t_d2.Resume();
                }
                finally
                {

                }
            }
        }
    }
}