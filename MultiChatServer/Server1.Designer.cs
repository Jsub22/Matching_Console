
namespace MultiChatServer
{
    partial class Server1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.connect = new System.Windows.Forms.ListBox();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.listView3 = new System.Windows.Forms.ListView();
            this.login_id = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.login_nick = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.login_client = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listView2 = new System.Windows.Forms.ListView();
            this.id = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pw = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.birth = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gender = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.phone = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listView1 = new System.Windows.Forms.ListView();
            this.check_id = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.check_pw = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.check_cnt = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.listView4 = new System.Windows.Forms.ListView();
            this.p_id = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.p_image = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.p_name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.p_nickname = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.p_age = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.p_birth = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.p_gender = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.p_local = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.p_interest = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(757, 146);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(151, 160);
            this.listBox1.TabIndex = 22;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(688, 5);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(108, 49);
            this.button2.TabIndex = 21;
            this.button2.Text = "서버 시작";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(687, 117);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(111, 21);
            this.textBox3.TabIndex = 20;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(687, 89);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(111, 21);
            this.textBox2.TabIndex = 19;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(687, 61);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(111, 21);
            this.textBox1.TabIndex = 18;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(607, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 50);
            this.button1.TabIndex = 17;
            this.button1.Text = "채팅서버 열기";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // connect
            // 
            this.connect.FormattingEnabled = true;
            this.connect.ItemHeight = 12;
            this.connect.Location = new System.Drawing.Point(500, 69);
            this.connect.Name = "connect";
            this.connect.Size = new System.Drawing.Size(182, 100);
            this.connect.TabIndex = 16;
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(500, 35);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(100, 21);
            this.txtPort.TabIndex = 15;
            this.txtPort.Text = "9999";
            // 
            // txtAddress
            // 
            this.txtAddress.Location = new System.Drawing.Point(500, 7);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.ReadOnly = true;
            this.txtAddress.Size = new System.Drawing.Size(100, 21);
            this.txtAddress.TabIndex = 13;
            // 
            // listView3
            // 
            this.listView3.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.login_id,
            this.login_nick,
            this.login_client});
            this.listView3.FullRowSelect = true;
            this.listView3.GridLines = true;
            this.listView3.HideSelection = false;
            this.listView3.Location = new System.Drawing.Point(275, 6);
            this.listView3.Name = "listView3";
            this.listView3.Size = new System.Drawing.Size(219, 162);
            this.listView3.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listView3.TabIndex = 14;
            this.listView3.UseCompatibleStateImageBehavior = false;
            this.listView3.View = System.Windows.Forms.View.Details;
            // 
            // login_id
            // 
            this.login_id.Text = "아이디";
            // 
            // login_nick
            // 
            this.login_nick.Text = "닉네임";
            // 
            // login_client
            // 
            this.login_client.Text = "클라이언트";
            this.login_client.Width = 89;
            // 
            // listView2
            // 
            this.listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.id,
            this.pw,
            this.name,
            this.birth,
            this.gender,
            this.phone});
            this.listView2.FullRowSelect = true;
            this.listView2.GridLines = true;
            this.listView2.HideSelection = false;
            this.listView2.Location = new System.Drawing.Point(12, 175);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(369, 204);
            this.listView2.TabIndex = 11;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.View = System.Windows.Forms.View.Details;
            // 
            // id
            // 
            this.id.Text = "아이디";
            // 
            // pw
            // 
            this.pw.Text = "비밀번호";
            // 
            // name
            // 
            this.name.Text = "이름";
            // 
            // birth
            // 
            this.birth.Text = "생년월일";
            // 
            // gender
            // 
            this.gender.Text = "성별";
            // 
            // phone
            // 
            this.phone.Text = "휴대전화";
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.check_id,
            this.check_pw,
            this.check_cnt});
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(11, 5);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(257, 163);
            this.listView1.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listView1.TabIndex = 12;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // check_id
            // 
            this.check_id.Text = "아이디";
            this.check_id.Width = 61;
            // 
            // check_pw
            // 
            this.check_pw.Text = "비밀번호";
            this.check_pw.Width = 69;
            // 
            // check_cnt
            // 
            this.check_cnt.Text = "로그인 실패 카운트";
            this.check_cnt.Width = 116;
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(803, 5);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(106, 21);
            this.textBox4.TabIndex = 23;
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(803, 32);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(106, 21);
            this.textBox5.TabIndex = 24;
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(803, 61);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(106, 21);
            this.textBox6.TabIndex = 25;
            // 
            // textBox7
            // 
            this.textBox7.Location = new System.Drawing.Point(803, 89);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(106, 21);
            this.textBox7.TabIndex = 26;
            // 
            // listView4
            // 
            this.listView4.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.p_id,
            this.p_image,
            this.p_name,
            this.p_nickname,
            this.p_age,
            this.p_birth,
            this.p_gender,
            this.p_local,
            this.p_interest});
            this.listView4.FullRowSelect = true;
            this.listView4.GridLines = true;
            this.listView4.HideSelection = false;
            this.listView4.Location = new System.Drawing.Point(388, 175);
            this.listView4.Name = "listView4";
            this.listView4.Size = new System.Drawing.Size(363, 203);
            this.listView4.TabIndex = 27;
            this.listView4.UseCompatibleStateImageBehavior = false;
            this.listView4.View = System.Windows.Forms.View.Details;
            // 
            // p_id
            // 
            this.p_id.Text = "아이디";
            // 
            // p_image
            // 
            this.p_image.Text = "이미지";
            // 
            // p_name
            // 
            this.p_name.Text = "이름";
            // 
            // p_nickname
            // 
            this.p_nickname.Text = "닉네임";
            // 
            // p_age
            // 
            this.p_age.Text = "나이";
            // 
            // p_birth
            // 
            this.p_birth.DisplayIndex = 6;
            this.p_birth.Text = "생년월일";
            // 
            // p_gender
            // 
            this.p_gender.DisplayIndex = 5;
            this.p_gender.Text = "성별";
            // 
            // p_local
            // 
            this.p_local.Text = "지역";
            // 
            // p_interest
            // 
            this.p_interest.Text = "흥미";
            // 
            // listBox2
            // 
            this.listBox2.FormattingEnabled = true;
            this.listBox2.ItemHeight = 12;
            this.listBox2.Location = new System.Drawing.Point(757, 313);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(151, 64);
            this.listBox2.TabIndex = 28;
            // 
            // Server1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(921, 391);
            this.Controls.Add(this.listBox2);
            this.Controls.Add(this.listView4);
            this.Controls.Add(this.textBox7);
            this.Controls.Add(this.textBox6);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.connect);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.txtAddress);
            this.Controls.Add(this.listView3);
            this.Controls.Add(this.listView2);
            this.Controls.Add(this.listView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Server1";
            this.Text = " ";
            this.Load += new System.EventHandler(this.Server1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox connect;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.ListView listView3;
        private System.Windows.Forms.ColumnHeader login_id;
        private System.Windows.Forms.ColumnHeader login_nick;
        public System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.ColumnHeader id;
        private System.Windows.Forms.ColumnHeader pw;
        private System.Windows.Forms.ColumnHeader name;
        private System.Windows.Forms.ColumnHeader birth;
        private System.Windows.Forms.ColumnHeader gender;
        private System.Windows.Forms.ColumnHeader phone;
        public System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader check_id;
        private System.Windows.Forms.ColumnHeader check_pw;
        private System.Windows.Forms.ColumnHeader check_cnt;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.TextBox textBox7;
        public System.Windows.Forms.ListView listView4;
        private System.Windows.Forms.ColumnHeader p_id;
        private System.Windows.Forms.ColumnHeader p_image;
        private System.Windows.Forms.ColumnHeader p_name;
        private System.Windows.Forms.ColumnHeader p_nickname;
        private System.Windows.Forms.ColumnHeader p_age;
        private System.Windows.Forms.ColumnHeader p_gender;
        private System.Windows.Forms.ColumnHeader p_birth;
        private System.Windows.Forms.ColumnHeader p_local;
        private System.Windows.Forms.ColumnHeader p_interest;
        private System.Windows.Forms.ColumnHeader login_client;
        private System.Windows.Forms.ListBox listBox2;
    }
}