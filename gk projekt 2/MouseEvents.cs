using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace gk_projekt_2
{
    public partial class Form1 : Form
    {
        private void Redraw_Button_Click(object sender, EventArgs e)
        {

            SetUpNet();
            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Pen p = new Pen(new SolidBrush(Color.Black));
            var graphics = e.Graphics;
            // e.Graphics.DrawImage(Values.vectormap, 0, 0);
            // Values.triangles[1].FillTriangle(graphics);


            var drawMap = new Color[pictureBox1.Width, pictureBox1.Height];

            for (int i = 0; i < pictureBox1.Width; i++)
            {
                for (int j = 0; j < pictureBox1.Height; j++)
                {
                    drawMap[i, j] = Color.White;
                }
            }


            if (useGlobalKMValues)
            {
                Parallel.ForEach(triangles, (triangle) =>
                {
                    triangle.FillTriangle(ref colorMap, ref vectorMap, ref drawMap, observerVector, lightSourceVector, lightColor, k_s, k_d, m, calculateColorType);
                });
            }
            else
            {
                Parallel.ForEach(triangles, (triangle) =>
                {
                    triangle.FillTriangle(ref colorMap, ref vectorMap, ref drawMap, observerVector, lightSourceVector, lightColor, triangle.ks, triangle.kd, triangle.m, calculateColorType);
                });
            }


            using (Bitmap canvas = new Bitmap(pictureBox1.Width, pictureBox1.Height))
            {
                unsafe
                {
                    BitmapData bitmapData = canvas.LockBits(new Rectangle(0, 0, canvas.Width, canvas.Height), ImageLockMode.ReadWrite, canvas.PixelFormat);

                    int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(canvas.PixelFormat) / 8;
                    int heightInPixels = bitmapData.Height;
                    int widthInBytes = bitmapData.Width * bytesPerPixel;
                    byte* PtrFirstPixel = (byte*)bitmapData.Scan0;

                    Parallel.For(0, heightInPixels, y =>
                    {
                        byte* currentLine = PtrFirstPixel + (y * bitmapData.Stride);
                        for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                        {
                            currentLine[x] = drawMap[x / 4, y].B;
                            currentLine[x + 1] = drawMap[x / 4, y].G;
                            currentLine[x + 2] = drawMap[x / 4, y].R;
                            currentLine[x + 3] = drawMap[x / 4, y].A;
                        }
                    });
                    canvas.UnlockBits(bitmapData);
                }

                graphics.DrawImage(canvas, 0, 0);
            }

            foreach (var t in triangles)
            {
                graphics.DrawLine(p, t.vertices[0].X, t.vertices[0].Y, t.vertices[1].X, t.vertices[1].Y);
                graphics.DrawLine(p, t.vertices[1].X, t.vertices[1].Y, t.vertices[2].X, t.vertices[2].Y);
                graphics.DrawLine(p, t.vertices[0].X, t.vertices[0].Y, t.vertices[2].X, t.vertices[2].Y);
            }

        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (moveVertice_checkBox1.Checked)
            {
                foreach (var el in vertices)
                {
                    if (Math.Sqrt(Math.Pow(e.X - el.X, 2) + Math.Pow(e.Y - el.Y, 2)) <= 5)
                    {
                        moving = el;
                        isMoving = true;
                        System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Hand;
                    }
                }
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {

            if (moveVertice_checkBox1.Checked && isMoving)
            {
                if (e.X < 0 || e.Y < 0) return;
                if (Math.Abs(moving.X - e.X) > 10 && Math.Abs(moving.X - e.X) > 10)
                {
                    moving.X = e.X;
                    moving.Y = e.Y;
                    pictureBox1.Invalidate();
                }
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (moveVertice_checkBox1.Checked && isMoving)
            {
                isMoving = false;
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;

            }
        }

        public void SetUpNet()
        {
            vertices = new List<Vertice>();
            triangles = new List<Triangle>();
            int maxx = pictureBox1.Width;
            int basex = 100;
            int maxy = pictureBox1.Height;
            int basey = 50;
            int delx = (maxx - 2 * basex) / N;
            int dely = (maxy - 2 * basey) / M;
            for (int j = 0; j <= M; j++)
            {
                for (int i = 0; i <= N; i++)
                {
                    Vertice p = new Vertice(basex + delx * i, basey + dely * j);
                    vertices.Add(p);
                }
            }
            for (int i = 0; i < N * M * 2; i++)
            {
                var t = new Triangle();
                t.m = rnd.Next(1, 100);
                t.ks = (double)rnd.Next(1, 100) / 100.0;
                t.kd = 1 - t.ks;
                triangles.Add(new Triangle());
            }
            for (int j = 0; j < M; j++)
                for (int i = 0; i < N; i++)
                {
                    triangles[(i + M * j) * 2].vertices.Add(vertices[i + (M + 1) * j]);
                    triangles[(i + M * j) * 2].vertices.Add(vertices[i + (M + 1) * j + 1]);
                    triangles[(i + M * j) * 2].vertices.Add(vertices[i + (M + 1) * (j + 1)]);


                    triangles[(i + M * j) * 2 + 1].vertices.Add(vertices[i + (M + 1) * (j + 1) + 1]);
                    triangles[(i + M * j) * 2 + 1].vertices.Add(vertices[i + (M + 1) * j + 1]);
                    triangles[(i + M * j) * 2 + 1].vertices.Add(vertices[i + (M + 1) * (j + 1)]);
                }
        }


        private void Init_Color_ComboBox(ComboBox cmbbox)
        {
            cmbbox.DataSource = colorList;
        }


        private void Init_LightColor_ComboBox(ComboBox cmbbox)
        {
            cmbbox.DataSource = lightColorList;
        }
    }
}
