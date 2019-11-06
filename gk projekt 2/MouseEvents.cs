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
                moving.X = e.X;
                moving.Y = e.Y;
                drawMap = (Color[,])colorMap.Clone();
                pictureBox1.Invalidate();
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

    }
}
