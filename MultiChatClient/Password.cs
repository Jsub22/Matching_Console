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
    public partial class Password : Form
    {
        Login login;
        public Password(Login form)//비밀번호 찾는 폼
        {
            InitializeComponent();
            login = form;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string id, name, phone;
            id = textBox5.Text.Trim();
            name = textBox1.Text.Trim();
            phone = textBox2.Text.Trim() + textBox3.Text.Trim() + textBox4.Text.Trim();

            string passwd_find = "passwd_find;" + id + ";" + name + ";" + phone;

            if (string.IsNullOrEmpty(textBox5.Text))
            {
                MsgBoxHelper.Warn("아이디를 입력해주세요");
                textBox5.Focus();
                return;
            }
            else if (string.IsNullOrEmpty(textBox1.Text))
            {
                MsgBoxHelper.Warn("이름을 입력해주세요");
                textBox1.Focus();
                return;
            }
            else if (string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox3.Text) || string.IsNullOrEmpty(textBox4.Text))
            {
                MsgBoxHelper.Warn("휴대폰번호를 입력해주세요");
                textBox2.Focus();
                return;
            }
            login.textBox5.Text = passwd_find;
            login.OnSendData(sender, e);
        }
        public void Passwd_find(string passwd)
        {
            if (passwd == "none")
                MessageBox.Show("비밀번호가 없거나 아이디, 이름 또는 휴대폰번호를 잘못 입력했습니다.");
            else
            {
                MessageBox.Show("비밀번호 : " + passwd);
                this.Close();
            }
            return;
        }
    }
}
