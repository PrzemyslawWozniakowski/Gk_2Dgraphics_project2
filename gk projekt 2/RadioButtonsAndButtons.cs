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
            drawMap = (Color[,])colorMap.Clone();
            pictureBox1.Invalidate();
        }



        private void LightColor_radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (LightColor_radioButton1.Checked) lightColor = new Vec3(1, 1, 1);
            else
            {
                Color col = (Color)LightColor_comboBox1.SelectedValue;
                lightColor = new Vec3(col.R, col.G, col.B);
                pictureBox1.Invalidate();
            }
            pictureBox1.Invalidate();
        }

        private void N_numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            N = (int)N_numericUpDown.Value;
            pictureBox1.Invalidate();
        }

        private void M_numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            M = (int)M_numericUpDown.Value;
            pictureBox1.Invalidate();
        }

        private void LightColor_comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            if (LightColor_radioButton2.Checked)
            {
                Color col = (Color)LightColor_comboBox1.SelectedValue;
                lightColor = new Vec3(col.R,col.G,col.B);
                pictureBox1.Invalidate();
            }
        }

        private void NormalVector_radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (NormalVector_radioButton2.Checked)
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
            pictureBox1.Invalidate();
        }

        private void Calculatecolor_radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (Calculatecolor_radioButton1.Checked) calculateColorType = 0;

            if (Calculatecolor_radioButton2.Checked) calculateColorType = 1;

            if (Calculatecolor_radioButton3.Checked) calculateColorType = 2;

            pictureBox1.Invalidate();
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            k_s = ((double)trackBar1.Value / 100.0);
            ks_label.Text = $"k_s={k_s}";
            pictureBox1.Invalidate();
        }

        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            k_d = ((double)trackBar2.Value / 100.0);
            kd_label.Text = $"k_d={k_d}";
            pictureBox1.Invalidate();
        }

        private void trackBar3_ValueChanged(object sender, EventArgs e)
        {
            m = trackBar3.Value;
            label1.Text = $"m={m}";
            pictureBox1.Invalidate();
        }

        private void kAndmValues_radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (kAndmValues_radioButton1.Checked) useGlobalKMValues = false;
            else useGlobalKMValues = true;
            pictureBox1.Invalidate();
        }

        private void LightSource_radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (LightSource_radioButton1.Checked)
            {
                lightSource = new Vec3(pictureBox1.Width / 2, pictureBox1.Height / 2, 10000);
                isAnimation = false;
                timer1.Enabled = false;
                useReflectors = false;
            }
            else
                if (LightSource_radioButton2.Checked)
                {
                A = pictureBox1.Width / 2;
                B = pictureBox1.Height / 2;
                lightSource = new Vec3(pictureBox1.Width / 2 + A * Math.Sin(a * t + gamma), pictureBox1.Height / 2 + B * Math.Sin(b * t), (double)numericUpDown1.Value);
            
                isAnimation = true;
                timer1.Enabled = true;
                useReflectors = false;
                }
                else
                {
                useReflectors = true;
                isAnimation = true;
                timer1.Enabled = true;
                }
            pictureBox1.Invalidate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (isAnimation&& LightSource_radioButton2.Checked)
            {
                int z = (int)numericUpDown1.Value;
                t += Math.PI / 180;
                lightSource = new Vec3(pictureBox1.Width / 2 + A * Math.Sin(a * t + gamma), pictureBox1.Height / 2 + B * Math.Sin(b * t), z);
                pictureBox1.Invalidate();
            }
            if (isAnimation && reflectors_radiobutton.Checked)
            {

                RedReflectorPosition.x = (int)Math.Ceiling(reflectorCenter.Item1 + reflectorR * Math.Sin(((double)Angle / 180) * Math.PI));
                RedReflectorPosition.y = (int)Math.Ceiling(reflectorCenter.Item2 + reflectorR * Math.Cos(((double)Angle / 180) * Math.PI));
                RedReflectorPosition.z = reflectorHeight;
                GreenReflectorPosition.x = (int)Math.Ceiling(reflectorCenter.Item1 + reflectorR * Math.Sin(((double)Angle1 / 180) * Math.PI));
                GreenReflectorPosition.y = (int)Math.Ceiling(reflectorCenter.Item2 + reflectorR * Math.Cos(((double)Angle1 / 180) * Math.PI));
                GreenReflectorPosition.z = reflectorHeight;
                BlueReflectorPosition.x = (int)Math.Ceiling(reflectorCenter.Item1 + reflectorR * Math.Sin(((double)Angle2 / 180) * Math.PI));
                BlueReflectorPosition.y = (int)Math.Ceiling(reflectorCenter.Item2 + reflectorR * Math.Cos(((double)Angle2 / 180) * Math.PI));
                BlueReflectorPosition.z = reflectorHeight;
                if (Angle < 360) Angle += 5;
                else Angle = 0;
                if (Angle1 < 360) Angle1 += 5;
                else Angle1 = 0;
                if (Angle2 < 360) Angle2 += 5;
                else Angle2 = 0;
                pictureBox1.Invalidate();


            }
        }

        private void ChooseButton_Click(object sender, EventArgs e)
        {
            ColorDialog MyDialog = new ColorDialog();
            MyDialog.AllowFullOpen = false;
            MyDialog.ShowHelp = true;
            if (MyDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ChooseButton.BackColor = MyDialog.Color;
                fillColor = MyDialog.Color;

                if (!Color_radioButton1.Checked)
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
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            dialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            dialog.Title = "Please select a bitmap.";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                colorbitmap = new Bitmap(@dialog.FileName);
                if (Color_radioButton1.Checked)
                    for (int i = 0; i < pictureBox1.Width; i++)
                    {
                        for (int j = 0; j < pictureBox1.Height; j++)
                        {
                            colorMap[i, j] = colorbitmap.GetPixel(i % colorbitmap.Width, j % colorbitmap.Height);
                        }
                    }
            }
            drawMap = (Color[,])colorMap.Clone();
            pictureBox1.Invalidate();

        }

        private void button2_Click(object sender, EventArgs e)
        {

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            dialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            dialog.Title = "Please select a normal map.";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                vectorbitmap = new Bitmap(@dialog.FileName);
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
            }

            pictureBox1.Invalidate();
        }
    }
}