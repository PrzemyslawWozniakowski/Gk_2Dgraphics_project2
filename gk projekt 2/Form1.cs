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
            colorbitmap = new Bitmap("C:/Users/Przem/source/repos/gk projekt 2/gk projekt 2/demonzz.jpg");
            vectorbitmap = new Bitmap("C:/Users/Przem/source/repos/gk projekt 2/gk projekt 2/skala.jpg");

           

            InitializeComponent();

            timer1.Interval = 100;
            Init_Color_ComboBox(Color_comboBox1);
            Init_LightColor_ComboBox(LightColor_comboBox1);
           // UpdateValues();
            SetUpNet();
            colorMap = new Color[pictureBox1.Width, pictureBox1.Height];
            vectorMap = new Color[pictureBox1.Width, pictureBox1.Height];
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
            pictureBox1.Invalidate();
        }


    

        private void Color_radioButton1_CheckedChanged(object sender, EventArgs e)
        {
           
            if (Color_radioButton1.Checked)
            {
                for (int i = 0; i < pictureBox1.Width; i++)
                {
                    for (int j = 0; j < pictureBox1.Height; j++)
                    {
                        colorMap[i, j] = colorbitmap.GetPixel(i % colorbitmap.Width, j % colorbitmap.Height);
                    }
                }
            }
            else
            {
                for (int i = 0; i < pictureBox1.Width; i++)
                {
                    for (int j = 0; j < pictureBox1.Height; j++)
                    {
                        colorMap[i, j] = fillColor;
                    }
                }
            }
        }

        private void Color_comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            fillColor =(Color)Color_comboBox1.SelectedValue;
        }


        private void LightColor_radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (LightColor_radioButton1.Checked) lightColor = Color.FromArgb(1, 1, 1);
            else lightColor = (Color)LightColor_comboBox1.SelectedValue;
        }

        private void N_numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            N= (int)N_numericUpDown.Value;
        }

        private void M_numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            M = (int)M_numericUpDown.Value;
        }

        private void LightColor_comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            if(LightColor_radioButton2.Checked)
                lightColor = (Color)LightColor_comboBox1.SelectedValue;
        }

        private void NormalVector_radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (!NormalVector_radioButton1.Checked)
            {
                for (int i = 0; i < pictureBox1.Width; i++)
                {
                    for (int j = 0; j < pictureBox1.Height; j++)
                    {
                        vectorMap[i, j] = vectorbitmap.GetPixel(i % vectorbitmap.Width, j % vectorbitmap.Height);
                    }
                }
            }
            else
            {
                for (int i = 0; i < pictureBox1.Width; i++)
                {
                    for (int j = 0; j < pictureBox1.Height; j++)
                    {
                        vectorMap[i, j] = Color.FromArgb(127, 127, 254);
                    }
                }
            }
        }

        private void Calculatecolor_radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (Calculatecolor_radioButton1.Checked) calculateColorType = 0;

            if (Calculatecolor_radioButton2.Checked) calculateColorType = 1;

            if (Calculatecolor_radioButton3.Checked) calculateColorType = 2;
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            k_d = ((double)trackBar1.Value / 100.0);
            k_s = 1 - k_d;
            ks_label.Text = $"k_s={k_s}";
            kd_label.Text = $"k_d={k_d}";
        }

        private void trackBar3_ValueChanged(object sender, EventArgs e)
        {
            m = trackBar3.Value;
            label1.Text= $"m={m}";
        }

        private void kAndmValues_radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (kAndmValues_radioButton1.Checked) useGlobalKMValues = false;
            else useGlobalKMValues = true;
        }

        private void LightSource_radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if(LightSource_radioButton1.Checked)
            {
                lightSourceVector = (0, 0, 1);
                isAnimation = false;
                timer1.Enabled = false;
            }
            else
            {
                isAnimation = true;
                timer1.Enabled = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (isAnimation)
            {
                var vl = lightSource;
                lightSource = (vl.Item1 + LSdx, vl.Item2 + LSdy, vl.Item3);
                if (lightSource.Item1 <= -1 || lightSource.Item1 >= 1) LSdx *= -1;
                if (lightSource.Item2 <= -1 || lightSource.Item2 >= 1) LSdy *= -1;
                lightSourceVector = NormalizeVector(lightSource);
               // ks_label.Text = Values.LightSourceVector.ToString();
                pictureBox1.Invalidate();   
            }
        }
    }

}
