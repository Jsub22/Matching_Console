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
    public partial class ID : Form
    {
        Login login;
        public ID(Login form)//아이디 찾는 폼이다.
        {
            InitializeComponent();
            login = form;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name, phone;
            name = textBox1.Text.Trim();
            phone = textBox2.Text.Trim() + textBox3.Text.Trim() + textBox4.Text.Trim();

            string id_find = "id_find;" + name + ";" + phone;

            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MsgBoxHelper.Warn("이름을 입력해주세요");
                textBox1.Focus();
                return;
            }
            else if (string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox3.Text) || string.IsNullOrEmpty(textBox4.Text))
            {
                MsgBoxHelper.Warn("휴대폰 번호를 입력해주세요");
                textBox2.Focus();
                return;
            }
            login.textBox5.Text = id_find;
            login.OnSendData(sender, e);
        }
        public void ID_find(string id)
        {
            if (id == "none")
                MessageBox.Show("아이디가 없거나 이름 또는 휴대폰번호를 잘못 입력했습니다.");
            else
            {
                MessageBox.Show("아이디 : " + id);
                this.Close();
            }
            return;
        }
    }
}
