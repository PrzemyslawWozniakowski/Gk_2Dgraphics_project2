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

        public (double, double, double) lightSourceVector = (0, 0, 1);
        public (double, double, double) observerVector = (0, 0, 1);

        public List<Vertice> vertices = new List<Vertice>();
        public List<Triangle> triangles = new List<Triangle>();

        public List<Color> colorList = new List<Color>(new Color[] { Color.Red, Color.Blue });

        public Color fillColor = Color.Red;
     //   public bool useMap = true;

        public List<Color> lightColorList = new List<Color>(new Color[] { Color.FromArgb(1, 1, 2), Color.FromArgb(2, 1, 2) });

        public Color lightColor = Color.FromArgb(1, 1, 1);

       // public bool vectorUseMap = true;

        public int calculateColorType = 0;
        public double k_s = 0.5;
        public double k_d = 0.5;

        public int m = 10;

        public bool useGlobalKMValues = true;
        public Random rnd = new Random();

        public bool isAnimation = false;
        public (double, double, double) lightSource = (0, 0, 1);
        public double LSdy = 0.1;
        public double LSdx = 0.2;

        public Color[,] colorMap;
        public Color[,] vectorMap;

        public (double, double, double) NormalizeVector((double, double, double) vector)
        {
            double length = Math.Pow(vector.Item1, 2) + Math.Pow(vector.Item2, 2) + Math.Pow(vector.Item3, 2);
            length = Math.Sqrt(length);
            return (vector.Item1 / length, vector.Item2 / length, vector.Item3 / length);
        }

    }
}
