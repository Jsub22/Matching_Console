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
    public partial class RandomMatching : Form
    {
        Home home;
        string myrole;
        int select_image;
        public RandomMatching(Home form, string text)
        {
            InitializeComponent();
            this.home = form;
            if (text != "")
            {
                SetRandMatching(text);
                //MessageBox.Show(text);
            }
        }

        public void SetRandMatching(string text)
        {
            //MessageBox.Show("들어오는 함수 작동 함");
            // 텍스트로 변환한다.
            string[] recievetext = text.Split(';'); // ~자, 랜덤, 프로필, 닉네임, 나이, 성별, 지역, 관심사, 주소
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

            textBox1.Text = recievetext[3];
            textBox3.Text = recievetext[4];
            textBox4.Text = recievetext[5];
            textBox5.Text = recievetext[6];
            textBox2.Text = recievetext[7];

            this.myrole = recievetext[0].Trim();
            if (myrole == "송신자")
            {
                button1.Text = "매칭 신청";
                button2.Text = "다시 매칭";
            }
            else // myrole == 수신자
            {
                button1.Text = "매칭 승락";
                button2.Text = "매칭 거절";
            }
        }

        private void button2_Click(object sender, EventArgs e)//다시 매칭
        {
            if (myrole=="송신자")
            {
                home.ReMatching(); // 다시 매칭
                this.Close();
            }
            else // myrole == 수신자
            {
                home.Matching("No"); // 거절
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)//매칭 신청
        {
            if (myrole == "송신자")
            {
                home.MatchingPut(); // 매칭 신청
            }
            else // myrole == 수신자
            {
                home.Matching("Yes"); // 실제 매칭 시작
            }
        }
    }
}
