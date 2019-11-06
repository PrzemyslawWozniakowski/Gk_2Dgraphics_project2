using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gk_projekt_2
{
    public partial class Form1 : Form
    {
        private void Init_LightColor_ComboBox(ComboBox cmbbox)
        {
            cmbbox.DataSource = lightColorList;
        }

        public void SetUpNet()
        {
            vertices = new List<Vertice>();
            triangles = new List<Triangle>();
            int maxx = pictureBox1.Width;
            int basex = 50;
            int maxy = pictureBox1.Height;
            int basey = 50;
            double delx = (double)(maxx - 2 * basex) / N;
            double dely = (double)(maxy - 2 * basey) / M;
            for (int j = 0; j <= M; j++)
            {
                for (int i = 0; i <= N; i++)
                {
                    Vertice p = new Vertice((int)(basex + delx * i), (int)(basey + dely * j));
                    vertices.Add(p);
                }
            }
            for (int i = 0; i < N * M * 2; i++)
            {
                var t = new Triangle();
                t.m = rnd.Next(1, 100);
                t.ks = (double)rnd.Next(1, 100) / 100.0;
                t.kd = (double)rnd.Next(1, 100) / 100.0;
                triangles.Add(t);
            }
            for (int j = 0; j < M; j++)
                for (int i = 0; i < N; i++)
                {
                    triangles[(i + N * j) * 2].vertices.Add(vertices[i + (N + 1) * j]);
                    triangles[(i + N * j) * 2].vertices.Add(vertices[i + (N + 1) * j + 1]);
                    triangles[(i + N * j) * 2].vertices.Add(vertices[i + (N + 1) * (j + 1)]);


                    triangles[(i + N * j) * 2 + 1].vertices.Add(vertices[i + (N + 1) * (j + 1) + 1]);
                    triangles[(i + N * j) * 2 + 1].vertices.Add(vertices[i + (N + 1) * j + 1]);
                    triangles[(i + N * j) * 2 + 1].vertices.Add(vertices[i + (N + 1) * (j + 1)]);
                }
        }
    }
}