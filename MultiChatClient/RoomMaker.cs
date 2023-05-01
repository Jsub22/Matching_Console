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
    public partial class RoomMaker : Form
    {
        Home home;
        public RoomMaker(Home form)
        {
            InitializeComponent();
            home = form;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string str = textBox1.Text;
            byte[] bDts = Encoding.UTF8.GetBytes("makeroom" + '\x01' + str);
            home.OnSendData(bDts, "makeroom");
            this.Close();
        }
    }
}
