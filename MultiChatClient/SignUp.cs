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
    public partial class SignUp : Form
    {
        Login login;
        public SignUp(Login form)
        {
            InitializeComponent();
            login = form;
        }

        private void txtTTS_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(sender, e);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                textBox1.Focus();
                MessageBox.Show("아이디를 입력하지 않았습니다.");
                return;
            }
            else if (string.IsNullOrEmpty(textBox2.Text))
            {
                textBox2.Focus();
                MessageBox.Show("비밀번호를 입력하지 않았습니다.");
                return;
            }
            else if (string.IsNullOrEmpty(textBox3.Text))
            {
                textBox3.Focus();
                MessageBox.Show("비밀번호 확인을 하지 않았습니다.");
                return;
            }
            else if (textBox3.Text != textBox2.Text)
            {
                textBox3.Focus();
                MessageBox.Show("비밀번호를 다시 확인해주세요.");
                return;
            }
            else if (string.IsNullOrEmpty(textBox4.Text))
            {
                textBox4.Focus();
                MessageBox.Show("이름을 입력하지 않았습니다.");
                return;
            }
            else if (string.IsNullOrEmpty(textBox5.Text) || textBox5.Text == "년(4자)" || string.IsNullOrEmpty(textBox6.Text) || textBox6.Text == "일" || comboBox2.SelectedIndex == -1)
            {
                textBox5.Focus();
                MessageBox.Show("생일을 입력하지 않았습니다.");
                return;
            }
            else if (comboBox1.SelectedIndex == -1)
            {
                comboBox1.Focus();
                MessageBox.Show("성별을 선택하지 않았습니다.");
                return;
            }
            else if (string.IsNullOrEmpty(textBox7.Text) || string.IsNullOrEmpty(textBox8.Text) || string.IsNullOrEmpty(textBox9.Text))
            {
                textBox7.Focus();
                MessageBox.Show("휴대폰 번호를 입력하지 않았습니다.");
                return;
            }
            else if (checkBox1.Checked == false)
            {
                checkBox1.Focus();
                MessageBox.Show("개인정보 수집을 동의하지 않았습니다.");
                return;
            }

            // 보낼 텍스트
            string id, passwd, name, birth, gender, phonenum;
            id = textBox1.Text.Trim();
            passwd = textBox2.Text.Trim();
            name = textBox4.Text.Trim();
            birth = textBox5.Text.Trim() + comboBox2.SelectedItem + textBox6.Text.Trim();
            gender = comboBox1.Text.Trim();
            phonenum = textBox7.Text.Trim() + textBox8.Text.Trim() + textBox9.Text.Trim();

            string sign_info = "sign;" + id + ";" + passwd + ";" + name + ";" + birth + ";" + gender + ";" + phonenum;

            string nickname, birth2, age, area, interest, select_image;
            nickname = id;
            age = (int.Parse(DateTime.Now.ToString("yyyy")) - int.Parse(textBox5.Text.Trim()) + 1).ToString();
            birth2 = comboBox2.SelectedItem + "." + textBox6.Text.Trim();
            area = "-";
            interest = "-,-,-";
            select_image = "0";

            string prof_info = ":" + id + ";" + select_image.ToString() + ";" + name + ";" + nickname + ";" + age + ";" + birth2 + ";" + gender + ";" + area + ";" + interest + ";";

            login.textBox5.Text = sign_info + prof_info;
            string str = sign_info + prof_info;

            byte[] bDts = Encoding.UTF8.GetBytes("sign" + '\x01' + str);

            login.textBox5.Text = sign_info + prof_info;
            login.OnSendData(sender, e); ;

            this.Close();
        }
    }
}