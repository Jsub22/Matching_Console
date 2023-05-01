using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace MultiChatClient
{
    public partial class ProfileImage : Form
    {
        public int select;

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            select = 4;
            Invalidate();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            select = 5;
            Invalidate();
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            select = 6;
            Invalidate();
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            select = 7;
            Invalidate();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            select = 3;
            Invalidate();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            select = 2;
            Invalidate();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            select = 1;
            Invalidate();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            select = 0;
            Invalidate();
        }

        public ProfileImage(int select_image)
        {
            InitializeComponent();
            select = select_image;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void ProfileImage_Paint(object sender, PaintEventArgs e)
        {
            Pen myPen = new Pen(Color.Blue, 3);
            Rectangle myRectangle;

            switch (select)
            {
                case 0:
                    Graphics g = e.Graphics;
                    myRectangle = new Rectangle(pictureBox1.Location.X, pictureBox1.Location.Y, pictureBox1.Width, pictureBox1.Height);
                    g.DrawRectangle(myPen, myRectangle);
                    break;
                case 1:
                    g = e.Graphics;
                    myRectangle = new Rectangle(pictureBox2.Location.X, pictureBox2.Location.Y, pictureBox2.Width, pictureBox2.Height);
                    g.DrawRectangle(myPen, myRectangle);
                    break;
                case 2:
                    g = e.Graphics;
                    myRectangle = new Rectangle(pictureBox3.Location.X, pictureBox3.Location.Y, pictureBox3.Width, pictureBox3.Height);
                    g.DrawRectangle(myPen, myRectangle);
                    break;
                case 3:
                    g = e.Graphics;
                    myRectangle = new Rectangle(pictureBox4.Location.X, pictureBox4.Location.Y, pictureBox4.Width, pictureBox4.Height);
                    g.DrawRectangle(myPen, myRectangle);
                    break;
                case 4:
                    g = e.Graphics;
                    myRectangle = new Rectangle(pictureBox5.Location.X, pictureBox5.Location.Y, pictureBox5.Width, pictureBox5.Height);
                    g.DrawRectangle(myPen, myRectangle);
                    break;
                case 5:
                    g = e.Graphics;
                    myRectangle = new Rectangle(pictureBox6.Location.X, pictureBox6.Location.Y, pictureBox6.Width, pictureBox6.Height);
                    g.DrawRectangle(myPen, myRectangle);
                    break;
                case 6:
                    g = e.Graphics;
                    myRectangle = new Rectangle(pictureBox7.Location.X, pictureBox7.Location.Y, pictureBox7.Width, pictureBox7.Height);
                    g.DrawRectangle(myPen, myRectangle);
                    break;
                case 7:
                    g = e.Graphics;
                    myRectangle = new Rectangle(pictureBox8.Location.X, pictureBox8.Location.Y, pictureBox8.Width, pictureBox8.Height);
                    g.DrawRectangle(myPen, myRectangle);
                    break;
            }
        }
    }
}