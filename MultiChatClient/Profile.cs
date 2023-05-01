using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Resources;

namespace MultiChatClient
{
    public partial class Profile : Form
    {
        int select_image;
        string myclient;
        delegate void AppendTextDelegate(Control ctrl, string s);
        Home home;

        public Profile(string ID, string client, Home h)
        {
            // 홈에서 프로필 설정
            InitializeComponent();

            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;

            home = h;
            myclient = client;
            textBox6.Text = ID;
        }

        public void SetProfile(string text)
        {
            // 텍스트로 변환한다.
            string[] recievetext = text.Split(';');
            //MsgBoxHelper.Warn(str);
            if (recievetext[1] == textBox6.Text.Trim()) // 아이디
            {
                textBox1.Text = recievetext[3]; // 이름
                textBox2.Text = recievetext[4]; // 닉네임
                textBox3.Text = recievetext[5]; // 나이
                textBox4.Text = recievetext[6]; // 생일
                textBox5.Text = recievetext[7]; // 성별
                comboBox1.Text = recievetext[8]; // 지역

                string[] spinterest = recievetext[9].Split(',');

                comboBox2.Text = spinterest[0]; // 관심사
                comboBox3.Text = spinterest[1];
                comboBox4.Text = spinterest[2];

                select_image = int.Parse(recievetext[2]); // 프로필 이미지 초기

                switch (select_image)
                {
                    case 0:
                        pictureBox1.Image = Properties.Resources.profile0;
                        break;
                    case 1:
                        pictureBox1.Image = Properties.Resources.profile1;
                        break;
                    case 2:
                        pictureBox1.Image = Properties.Resources.profile2;
                        break;
                    case 3:
                        pictureBox1.Image = Properties.Resources.profile3;
                        break;
                    case 4:
                        pictureBox1.Image = Properties.Resources.profile4;
                        break;
                    case 5:
                        pictureBox1.Image = Properties.Resources.profile5;
                        break;
                    case 6:
                        pictureBox1.Image = Properties.Resources.profile6;
                        break;
                    case 7:
                        pictureBox1.Image = Properties.Resources.profile7;
                        break;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // 닉네임 입력
            if (this.textBox2.Text.Length < 1 || this.textBox2.TextLength > 11)
            {
                MessageBox.Show("닉네임은 1~11자 이내로 입력하세요.");
                this.textBox2.Focus();

                return;
            }

            // 지역 입력
            if (comboBox1.SelectedIndex < 0)
            {
                MessageBox.Show("지역를 한 개 이상 선택해 주세요.");
                this.comboBox1.Focus();

                return;
            }
            // 관심사 입력
            if (comboBox2.SelectedIndex <= 0 && comboBox3.SelectedIndex <= 0 && comboBox4.SelectedIndex <= 0)
            {
                MessageBox.Show("관심사를 한 개 이상 선택해 주세요.");
                this.comboBox2.Focus();

                return;
            }
            // 보낼 텍스트
            string id, name, nickname, age, birth, gender, area, interest; // + select_image
            id = textBox6.Text.Trim();
            name = textBox1.Text.Trim();
            nickname = textBox2.Text.Trim();
            age = textBox3.Text.Trim();
            birth = textBox4.Text.Trim();
            gender = textBox5.Text.Trim();
            area = comboBox1.Text.Trim();
            interest = comboBox2.Text + "," + comboBox3.Text + "," + comboBox4.Text;

            string prof_info = id + ";" + select_image.ToString() + ";" + name + ";" + nickname + ";" + age + ";" + birth + ";" + gender + ";" + area + ";" + interest + ";" + myclient;

            if (string.IsNullOrEmpty(prof_info))
            {
                MsgBoxHelper.Warn("텍스트가 입력되지 않았습니다!");
                textBox1.Focus();
                return;
            }

            // 문자열을 utf8 형식의 바이트로 변환한다.
            byte[] bDts = Encoding.UTF8.GetBytes("prof" + '\x01' + prof_info);
            //MsgBoxHelper.Warn(prof_info + " (Prof:prof_info)");

            //MessageBox.Show(Encoding.UTF8.GetString(bDts));
            // 서버에 전송한다.
            home.OnSendData(bDts, "prof");
            MessageBox.Show("저장되었습니다.");
            Invalidate();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ProfileImage pi = new ProfileImage(select_image);
            if (pi.ShowDialog() == DialogResult.OK)
            {
                select_image = pi.select;

                switch (select_image)
                {
                    case 0:
                        pictureBox1.Image = Properties.Resources.profile0;
                        break;
                    case 1:
                        pictureBox1.Image = Properties.Resources.profile1;
                        break;
                    case 2:
                        pictureBox1.Image = Properties.Resources.profile2;
                        break;
                    case 3:
                        pictureBox1.Image = Properties.Resources.profile3;
                        break;
                    case 4:
                        pictureBox1.Image = Properties.Resources.profile4;
                        break;
                    case 5:
                        pictureBox1.Image = Properties.Resources.profile5;
                        break;
                    case 6:
                        pictureBox1.Image = Properties.Resources.profile6;
                        break;
                    case 7:
                        pictureBox1.Image = Properties.Resources.profile7;
                        break;
                }
            }
        }
    }
}
