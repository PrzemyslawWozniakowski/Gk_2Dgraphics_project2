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
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Pen p = new Pen(new SolidBrush(Color.Black));
            var graphics = e.Graphics;


            if (useGlobalKMValues)
            {
                Parallel.ForEach(triangles, (triangle) =>
                {
                    triangle.FillTriangle(ref colorMap, ref vectorMap, ref drawMap, observerVector, lightSource, lightColor, k_s, k_d, m, calculateColorType);
                });
            }
            else
            {
                Parallel.ForEach(triangles, (triangle) =>
                {
                    triangle.FillTriangle(ref colorMap, ref vectorMap, ref drawMap, observerVector, lightSource, lightColor, triangle.ks, triangle.kd, triangle.m, calculateColorType);
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
    }
}