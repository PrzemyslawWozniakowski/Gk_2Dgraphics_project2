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
        public Vertice moving=null;
        bool isMoving = false;


        public Form1()
        {
            colorbitmap = new Bitmap("../../Bitmaps/img1.jpg");
            vectorbitmap = new Bitmap("../../NormalMaps/normalMap2.jpg");
            InitializeComponent();

            lightSource = new Vec3(pictureBox1.Width / 2, pictureBox1.Height / 2, 10000);
            timer1.Interval = 20;

            Init_LightColor_ComboBox(LightColor_comboBox1);
            SetUpNet();
            drawMap = new Color[pictureBox1.Width, pictureBox1.Height];
            colorMap = new Color[pictureBox1.Width, pictureBox1.Height];
            vectorMap = new Color[pictureBox1.Width, pictureBox1.Height];
            RedReflectorPosition = new Vec3(400,400,reflectorR);
            GreenReflectorPosition = new Vec3(400, 400, reflectorR);
            BlueReflectorPosition = new Vec3(400, 400, reflectorR);

            RedReflectorPosition.x = (int)Math.Ceiling(reflectorCenter.Item1 + reflectorR * Math.Sin(((double)Angle / 180) * Math.PI));
            RedReflectorPosition.y = (int)Math.Ceiling(reflectorCenter.Item2 + reflectorR * Math.Cos(((double)Angle / 180) * Math.PI));
            GreenReflectorPosition.x = (int)Math.Ceiling(reflectorCenter.Item1 + reflectorR * Math.Sin(((double)Angle1 / 180) * Math.PI));
            GreenReflectorPosition.y = (int)Math.Ceiling(reflectorCenter.Item2 + reflectorR * Math.Cos(((double)Angle1 / 180) * Math.PI));
            BlueReflectorPosition.x = (int)Math.Ceiling(reflectorCenter.Item1 + reflectorR * Math.Sin(((double)Angle2 / 180) * Math.PI));
            BlueReflectorPosition.y = (int)Math.Ceiling(reflectorCenter.Item2 + reflectorR * Math.Cos(((double)Angle2 / 180) * Math.PI));
            for (int i = 0; i < pictureBox1.Width; i++)
            {
                for (int j = 0; j < pictureBox1.Height; j++)
                {
                    vectorMap[i, j] = vectorbitmap.GetPixel(i % vectorbitmap.Width, j % vectorbitmap.Height);
                }
            }
            for (int i = 0; i < pictureBox1.Width; i++)
            {
                for (int j = 0; j < pictureBox1.Height; j++)
                {
                    colorMap[i, j] = colorbitmap.GetPixel(i % colorbitmap.Width, j % colorbitmap.Height);
                }
            }
            drawMap = (Color[,])colorMap.Clone();
            pictureBox1.Invalidate();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            reflectorHeight = (int)numericUpDown2.Value;
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            reflectorR = (int)numericUpDown3.Value;
        }
    }

}
