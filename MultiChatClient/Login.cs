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

namespace MultiChatClient
{
    public partial class Login : Form
    {
        delegate void AppendTextDelegate(Control ctrl, string s);
        AppendTextDelegate _textAppender;
        Socket mainSock;

        public Login()
        {
            InitializeComponent();
            mainSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            _textAppender = new AppendTextDelegate(AppendText);

            IPHostEntry he = Dns.GetHostEntry(Dns.GetHostName());

            // 처음으로 발견되는 ipv4 주소를 사용한다.
            IPAddress defaultHostAddress = null;
            foreach (IPAddress addr in he.AddressList)
            {
                if (addr.AddressFamily == AddressFamily.InterNetwork)
                {
                    defaultHostAddress = addr;
                    break;
                }
            }

            if (mainSock.Connected)
            {
                MsgBoxHelper.Error("이미 연결되어 있습니다!");
                return;
            }

            try { mainSock.Connect(defaultHostAddress, 9999); }
            catch (Exception ex)
            {
                MsgBoxHelper.Error("연결에 실패했습니다!\n오류 내용: {0}", MessageBoxButtons.OK, ex.Message);
                return;
            }
            // 연결 완료, 서버에서 데이터가 올 수 있으므로 수신 대기한다.
            AsyncObject obj = new AsyncObject(4096);
            obj.WorkingSocket = mainSock;
            mainSock.BeginReceive(obj.Buffer, 0, obj.BufferSize, 0, DataReceived, obj);


            if (!mainSock.IsBound)
            {
                MsgBoxHelper.Warn("서버가 실행되고 있지 않습니다!");
                return;
            }
        }

        void AppendText(Control ctrl, string s)
        {
            if (ctrl.InvokeRequired) ctrl.Invoke(_textAppender, ctrl, s);
            else
            {
                string source = ctrl.Text;
                ctrl.Text = source + Environment.NewLine + s;
            }
        }

        void OnFormLoaded(object sender, EventArgs e)
        {
            IPHostEntry he = Dns.GetHostEntry(Dns.GetHostName());

            // 처음으로 발견되는 ipv4 주소를 사용한다.
            IPAddress defaultHostAddress = null;
            foreach (IPAddress addr in he.AddressList)
            {
                if (addr.AddressFamily == AddressFamily.InterNetwork)
                {
                    defaultHostAddress = addr;
                    break;
                }
            }
            //txtAddress.Text = defaultHostAddress.ToString();
            string IPAddress = "defaultHostAddress.ToString()";
        }

        void OnConnectToServer(object sender, EventArgs e)
        {

        }

        void DataReceived(IAsyncResult ar)
        {
            try
            {
                // BeginReceive에서 추가적으로 넘어온 데이터를 AsyncObject 형식으로 변환한다.
                AsyncObject obj = (AsyncObject)ar.AsyncState;

                // 데이터 수신을 끝낸다.
                //서버에서 보낸 데이터가 이 receiced로 들어온다
                int received = obj.WorkingSocket.EndReceive(ar);

                // 받은 데이터가 없으면(연결끊어짐) 끝낸다.
                if (received <= 0)
                {
                    obj.WorkingSocket.Close();
                    return;
                }

                // 텍스트로 변환한다.
                string text = Encoding.UTF8.GetString(obj.Buffer);
                string[] recievetext = text.Split(';');
                textBox4.Text = recievetext[1];

                if (recievetext[0] == "client")//서버에서 보내준 본인 클라이언트 번호(ex)129.258.7.8:2540)
                {
                    textBox3.Text = recievetext[1];
                }                
                else if (recievetext[0] == "login")//로그인 버튼 눌렸을 때 서버쪽으로 입력한 아이디와 비밀번호를 보내서 거기서 true/false를 비교한 뒤 그 결과값을 받아 이용하는 곳
                {
                    if (textBox4.Text == "true")//true를 받았으면 로그인
                    {
                        textBox5.Text = recievetext[2];
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else if (textBox4.Text == "false")//false를 받았으면 로그인 안됨
                    {
                        textBox1.Text = "";
                        textBox2.Text = "";
                        MsgBoxHelper.Warn("아이디나 비밀번호가 틀렸습니다");
                    }
                }
                else if (recievetext[0] == "id_find")//아이디 찾기. 개인 정보를 입력하고 버튼을 누르면 서버로 정보가 넘어가서 넘어간 정보와 회원가입 시 집어넣은 정보가 비교되서 회원정보가 있는지 확인. 그 결과가 넘어와 이곳에서 사용.
                {
                    ID id_find = new ID(this);
                    id_find.ID_find(recievetext[1]);
                }
                else if (recievetext[0] == "passwd_find")
                {
                    Password passwd_find = new Password(this);
                    passwd_find.Passwd_find(recievetext[1]);
                }
                // 클라이언트에선 데이터를 전달해줄 필요가 없으므로 바로 수신 대기한다.
                // 데이터를 받은 후엔 다시 버퍼를 비워주고 같은 방법으로 수신을 대기한다.
                obj.ClearBuffer();

                // 수신 대기
                obj.WorkingSocket.BeginReceive(obj.Buffer, 0, 4096, 0, DataReceived, obj);
            }
            catch //(Exception ex)
            {
                //
                
                return;
            }
        }

        public void OnSendData(object sender, EventArgs e)
        {
            string[] info = textBox5.Text.Split(';');
            if (info[0] == "sign")//회원가입 버튼을 눌렀을 때 이곳에 회원가입 정보가 넘어온다.
            {
                //MessageBox.Show(textBox5.Text);
                byte[] bDts = Encoding.UTF8.GetBytes("sign" + '\x01' + textBox5.Text.Substring(5));
                mainSock.Send(bDts);//넘어온 정보를 서버로 보내기.
            }
            else if (info[0] == "id_find")//아이디 찾기 버튼을 눌렀을 때 찾기위해 넣은 정보(이름,전화번호)가 이곳에 넘어온다.
            {
                byte[] bDts = Encoding.UTF8.GetBytes("id_find" + '\x01' + textBox3.Text + ";" + textBox5.Text.Substring(8));
                mainSock.Send(bDts);//넘어온 정보를 서버로.
            }
            else if (info[0] == "passwd_find")
            {
                byte[] bDts = Encoding.UTF8.GetBytes("passwd_find" + '\x01' + textBox3.Text + ";" + textBox5.Text.Substring(12));
                mainSock.Send(bDts);
            }
        }

        // ip 유효성 검사
        public bool IsAvailableIP(string ip)
        {
            // ip 값이 null이면 실패 반환
            if (ip == null)
                return false;

            // ip 길이가 15자 넘거나 7보다 작으면 실패를 반환    
            if (ip.Length > 15 || ip.Length < 7)
                return false;

            // 숫자 갯수
            int nNumCount = 0;

            // '.' 갯수 
            int nDotCount = 0;

            for (int i = 0; i < ip.Length; i++)
            {
                if (ip[i] < '0' || ip[i] > '9')
                {
                    if ('.' == ip[i])
                    {
                        ++nDotCount;
                        nNumCount = 0;
                    }
                    else
                        return false;
                }
                else
                {
                    //'.'이 4개 이상이면 실패 반환
                    if (++nNumCount > 3)
                        return false;
                }
            }
            // '.' 3개 아니여도 실패 반환
            if (nDotCount != 3)
                return false;

            return true;
        }

        private void ChatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            mainSock.Close();
        }

        private void txtTTS_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(sender, e);
            }
        }

        public void button1_Click(object sender, EventArgs e)//로그인 버튼이다.
        {
            // 보낼 텍스트
            string id, passwd;
            id = textBox1.Text.Trim();
            passwd = textBox2.Text.Trim();

            string login_info = textBox3.Text + ";" + id + ";" + passwd;


            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MsgBoxHelper.Warn("아이디를 입력하지 않았습니다");
                textBox1.Focus();
                return;
            }
            else if (string.IsNullOrEmpty(textBox2.Text))
            {
                MsgBoxHelper.Warn("비밀번호를 입력하지 않았습니다");
                textBox2.Focus();
                return;
            }

            // 문자열을 utf8 형식의 바이트로 변환한다.
            byte[] bDts = Encoding.UTF8.GetBytes("login" + '\x01' + login_info);

            // 서버에 전송한다.

            mainSock.Send(bDts);
            //mainSock.Send(bDts);//넣은 아이디와 비밀번호를 서버로 보내는 곳.
            //mainSock.BeginSend(bDts, 0, 4096, 0, new AsyncCallback(DataReceived), mainSock);

            //this.Close();
        }

        private void button4_Click(object sender, EventArgs e)//회원가입 버튼
        {
            SignUp signup = new SignUp(this);
            signup.Show();
        }

        private void button2_Click(object sender, EventArgs e)//아이디 찾기 버튼
        {
            ID idfind = new ID(this);
            idfind.Show();
        }

        private void button3_Click(object sender, EventArgs e)//비밀번호 찾기 버튼
        {
            Password passwdfind = new Password(this);
            passwdfind.Show();
        }
    }
}
