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
        public int N = 5;
        public int M = 5;
        public Bitmap vectorbitmap;
        public Bitmap colorbitmap;

        public Vec3 observerVector = new Vec3 (0, 0, 1);

        public List<Vertice> vertices = new List<Vertice>();
        public List<Triangle> triangles = new List<Triangle>();

        public Color fillColor = Color.Red;

        public List<Color> lightColorList = new List<Color>(new Color[] { Color.FromArgb(1, 1, 2), Color.FromArgb(2, 1, 2), Color.FromArgb(1, 2, 1), });

        public Color lightColor = Color.FromArgb(1, 1, 1);

        public int calculateColorType = 0;
        public double k_s = 0.5;
        public double k_d = 0.5;

        public int m = 10;

        public bool useGlobalKMValues = true;
        public Random rnd = new Random();

        public bool isAnimation = false;
        public Vec3 lightSource;
        public double t = 0;
        public double a = 3;
        public double b = 2;
        public double A=0;
        public double B=0;
        public double gamma = Math.PI / 2;

        public Color[,] drawMap;
        public Color[,] colorMap;
        public Color[,] vectorMap;

        public int Clamp(int val, int from, int to)
        {
            if (to < from) return -1;
            if (val > to) val = to;
            if (val < from) val = from;
            return val;
        }
    }
}
