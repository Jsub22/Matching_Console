using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

namespace MultiChatClient {
    public partial class ChatForm : Form {
        delegate void AppendTextDelegate(Control ctrl, string s);
        AppendTextDelegate _textAppender;
        Socket mainSock;

        // 서버에서 채팅방 정보를 받아오기
        List<string> INFO_ID = new List<string>();
        String My_ID; // 내 아이디
        String My_NICK; // 내 닉네임
        String Chat_ID; // 채팅방 변수
        Home hm;

        ViewImage viewimg = new ViewImage();
        //Popup frm = new Popup();
        string popup_path;

        // 채팅 폼을 만들면서 채팅방 변수 및 아이디에 변수 추가, 채팅방 이름 추가
        public ChatForm(Home home, string roomnum) {
            InitializeComponent();
            mainSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            _textAppender = new AppendTextDelegate(AppendText);
            popup_path = Environment.CurrentDirectory;
            popup_path = Path.GetFullPath(Path.Combine(popup_path, @"..\..\")) + @"dingdong.wav";

            hm = home;

            this.Name = roomnum;
            this.Chat_ID = roomnum;
            this.My_ID = hm.id;
            this.My_NICK = hm.nick;


            // 채팅방 이름의 경우, 만약 Listbox에서 더블클릭해서 넘어오는 경우 Listbox의 값을 그대로 이용한다. 아닐경우 랜덤 채팅이라고 제목을 지어준다.
            if(hm.Chat_Data.SelectedItem == null)
            {
                this.Text = "랜덤 채팅";
            }
            else
            {
                this.Text = hm.Chat_Data.Items[hm.Chat_Data.SelectedIndex].ToString();
            }

            //기본 포트를 9000으로 설정한다.
            txtPort.Text = "9000";

            //자동 실행
            //btnConnect.PerformClick();
        }



        void AppendText(Control ctrl, string s) {
            if (ctrl.InvokeRequired) ctrl.Invoke(_textAppender, ctrl, s);
            else {
                string source = ctrl.Text;
                ctrl.Text = source + Environment.NewLine + s;
            }
        }

        void OnFormLoaded(object sender, EventArgs e) {
            IPHostEntry he = Dns.GetHostEntry(Dns.GetHostName());

            // 처음으로 발견되는 ipv4 주소를 사용한다.
            IPAddress defaultHostAddress = null;
            foreach (IPAddress addr in he.AddressList) {
                if (addr.AddressFamily == AddressFamily.InterNetwork) {
                    defaultHostAddress = addr;
                    break;
                }
            }

            // 주소가 없다면..
            if (defaultHostAddress == null)
                // 로컬호스트 주소를 사용한다.
                defaultHostAddress = IPAddress.Loopback;

            txtAddress.Text = defaultHostAddress.ToString();

        }

        void OnConnectToServer(object sender, EventArgs e) {
            if (mainSock.Connected) {
                MsgBoxHelper.Error("이미 연결되어 있습니다!");
                return;
            }

            
            if (!IsAvailableIP(txtAddress.Text))
            {
                MsgBoxHelper.Error("IP 번호가 입력되지 않았거나 형식이 유효하지 않습니다.");
                txtAddress.Focus();
                txtAddress.SelectAll();
                return;
            }


            int port;
            if (!int.TryParse(txtPort.Text, out port)) {
                MsgBoxHelper.Error("포트 번호가 잘못 입력되었거나 입력되지 않았습니다.");
                txtPort.Focus();
                txtPort.SelectAll();
                return;
            }

            try { mainSock.Connect(txtAddress.Text, port); }
            catch (Exception ex) {
                MsgBoxHelper.Error("연결에 실패했습니다!\n오류 내용: {0}", MessageBoxButtons.OK, ex.Message);
                return;
            }

            // 연결 완료되었다는 메세지를 띄워준다.
            txtHistory.AppendText("채팅방과 연결되었습니다. 환영합니다!\r\n");

            // 채팅방에 들어가면서 해당 채팅방에 참가했다는 메세지를 서버에 보낸다.
            byte[] bDts = Encoding.UTF8.GetBytes("Input" + '\x01' + My_NICK + '\x01' + Chat_ID);
            mainSock.Send(bDts);

            // 연결 완료, 서버에서 데이터가 올 수 있으므로 수신 대기한다.
            AsyncObject obj = new AsyncObject(50000);
            obj.WorkingSocket = mainSock;
            mainSock.BeginReceive(obj.Buffer, 0, obj.BufferSize, 0, DataReceived, obj);
        }
        
        void DataReceived(IAsyncResult ar) 
        {
            try
            {
                // BeginReceive에서 추가적으로 넘어온 데이터를 AsyncObject 형식으로 변환한다.
                AsyncObject obj = (AsyncObject)ar.AsyncState;

                // 데이터 수신을 끝낸다.
                int received = obj.WorkingSocket.EndReceive(ar);

                // 받은 데이터가 없으면(연결끊어짐) 끝낸다.
                if (received <= 0)
                {
                    obj.WorkingSocket.Close();
                    return;
                }

                // 텍스트로 변환한다.
                string text = Encoding.UTF8.GetString(obj.Buffer);

                // 0x01 기준으로 짜른다.
                // tokens[0] - 보낸 사람 ID
                // tokens[1] - 보낸 메세지
                string[] tokens = text.Split('\x01');
                string id = tokens[0];

                //MessageBox.Show(ip.TrimEnd() + " 뒷쪽 문자열 받는거 체크");

                //처음 값을 받을 때 client id값을 확인한다.
                string[] tokens_t = text.Split(';');
                string t_ip = tokens_t[0];

                if (t_ip == "client") { }


                // 다른 사람이 서버에 참가했다는 값을 받았을 경우
                else if (id == "Input")
                {
                    string msg = tokens[1];
                    String Temp = "------ " + msg.Replace("\0", "") + "님이 입장하셨습니다 ------";
                    txtHistory.AppendText(Temp + "\r\n");
                }


                // 다른 사람이 서버에 퇴장했다는 값을 받았을 경우
                else if (id == "Output")
                {
                    string msg = tokens[1];
                    String Temp = "------ " + msg.Replace("\0", "") + "님이 퇴장하셨습니다 ------";
                    txtHistory.AppendText(Temp + "\r\n");
                }

                // 서버 정보를 알고싶은 경우 현재 참가 인원과 해당 인원들의 닉네임을 출력한다.
                else if (id == "ChatInfo")
                {
                    string msg = tokens[1];
                    //MessageBox.Show(msg.Replace("\0", ""));
                }

                else if (id == "ChatText")
                {
                    id = tokens[1];
                    string date = tokens[2];
                    string msg = tokens[3];
                    txtHistory.AppendText(string.Format("[받음] {0} {1} : {2}\r\n", id, date, msg));

                    //Alert();
                    AlertSound();
                }

                else if (id == "SecChat")
                {
                    txtHistory.AppendText(string.Format("[받음] {0}의 귓속말 {1} : {2}\r\n", tokens[1], tokens[2], tokens[3]));
                    AlertSound();
                }

                else
                {
                    byte[] bDts = obj.Buffer;
                    MemoryStream ms = new MemoryStream(bDts);

                    //viewimg = new ViewImage();
                    viewimg.pictureBox1.Image = Image.FromStream(ms);
                    //viewimg.Show();
                    txtHistory.AppendText("[받음] 새 이미지가 도착했습니다.\r\n");
                    AlertSound();
                }
                // 클라이언트에선 데이터를 전달해줄 필요가 없으므로 바로 수신 대기한다.
                // 데이터를 받은 후엔 다시 버퍼를 비워주고 같은 방법으로 수신을 대기한다.

                obj.ClearBuffer();

                // 수신 대기
                obj.WorkingSocket.BeginReceive(obj.Buffer, 0, 50000, 0, DataReceived, obj);
            }
            catch //(Exception ex)
            {
                return;
            }
        }

        void OnSendData(object sender, EventArgs e) {
            // 서버가 대기중인지 확인한다.
            if (!mainSock.IsBound) {
                MsgBoxHelper.Warn("서버가 실행되고 있지 않습니다!");
                return;
            }

            // 보낼 텍스트
            string tts = txtTTS.Text.Trim();
            if (string.IsNullOrEmpty(tts)) {
                MsgBoxHelper.Warn("텍스트가 입력되지 않았습니다!");
                txtTTS.Focus();
                return;
            }

            // 서버 ip 주소와 메세지를 담도록 만든다.
            IPEndPoint ip = (IPEndPoint) mainSock.LocalEndPoint;
            string addr = ip.Address.ToString();
            string date = DateTime.Now.ToString("M월d일 HH:mm");
            byte[] bDts;

            if (checkBox1.Checked)
            {
                if (textBox1.TextLength <= 0)
                {
                    MsgBoxHelper.Warn("귓속말을 보낼 닉네임이 입력되지 않았습니다.");
                    textBox1.Focus();
                    return;
                }
                bDts = Encoding.UTF8.GetBytes("SecChat" + '\x01' + textBox1.Text + '\x01' + My_NICK + '\x01' + date + '\x01' + tts + '\x01' + Chat_ID);
            }
            else // 문자열을 utf8 형식의 바이트로 변환한다.
            {
                bDts = Encoding.UTF8.GetBytes("ChatText" + '\x01' + My_NICK + '\x01' + date + '\x01' + tts + '\x01' + Chat_ID);
            }

            // 서버에 전송한다.
            mainSock.Send(bDts);

            // 전송 완료 후 텍스트박스에 추가하고, 원래의 내용은 지운다.
            txtHistory.AppendText(string.Format("[보냄] {0} {1} : {2}\r\n", My_NICK, date, tts));
            txtTTS.Clear();
            txtTTS.Focus();
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


        // 채팅방을 닫는 경우 서버에 채팅방을 닫는다는 알람을 전송한다.
        public void ChatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!mainSock.IsBound)
            {
                mainSock.Close();
                return;
            }

            byte[] bDts = Encoding.UTF8.GetBytes("Output" + '\x01' + My_NICK + '\x01' + Chat_ID);
            mainSock.Send(bDts);

            mainSock.Close();

            for (int i = hm.List_ChatForm.Count-1; i >= 0; i--)
            {
                if (this == hm.List_ChatForm[i]) hm.List_ChatForm.RemoveAt(i);
            }
        }

        //부모인 Home에 인해 닫히는 경우
        public void HomeChatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!mainSock.IsBound)
            {
                mainSock.Close();
                return;
            }

            byte[] bDts = Encoding.UTF8.GetBytes("Output" + '\x01' + My_NICK + '\x01' + Chat_ID);
            mainSock.Send(bDts);

            mainSock.Close();

        }

        private void txtTTS_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OnSendData(sender,  e);
            }
        }


        // 채팅방 정보 확인 버튼
        // 서버와 연결되어있지 않은 경우, 실행되지 않으며, 실행될 경우 채팅방 변수값과 아이디를 서버로 넘긴다.
        private void INFO_Check_Click(object sender, EventArgs e)
        {
            if (!mainSock.IsBound)
            {
                MsgBoxHelper.Warn("서버가 실행되고 있지 않습니다!");
                return;
            }

            byte[] bDts = Encoding.UTF8.GetBytes("ChatInfo" + '\x01' + My_NICK + '\x01' + Chat_ID);
            mainSock.Send(bDts);
        }

        private void attchImg_Click(object sender, EventArgs e)
        {
            string filePath = string.Empty;
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Multiselect = false;
            openDlg.Filter = "Image Files|*.jpg;*.jpeg;*.png";
            if (openDlg.ShowDialog() == DialogResult.OK)
                filePath = openDlg.FileName;
            if (string.IsNullOrEmpty(filePath))
                return;

            txtHistory.AppendText("[보냄] 이미지를 전송했습니다.\r\n");

            Image myImage = Image.FromFile(filePath);
            Clipboard.SetDataObject(myImage);
            DataFormats.Format myFormat = DataFormats.GetFormat(DataFormats.Bitmap);

            MemoryStream ms = new MemoryStream();
            myImage.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            byte[] bDts = ms.ToArray();

            if (!mainSock.IsBound)
            {
                MsgBoxHelper.Warn("서버가 실행되고 있지 않습니다!");
                return;
            }
            mainSock.Send(bDts);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            viewimg.Show();
        }

        /*void Alert()
        {
            timer1.Start();

            System.Drawing.Rectangle ScreenRectangle = Screen.PrimaryScreen.WorkingArea;
            int xPos = ScreenRectangle.Width - frm.Bounds.Width - 5;
            int yPos = ScreenRectangle.Height - frm.Bounds.Height - 5;

            frm.Show();
            frm.SetBounds(xPos, yPos, frm.Size.Width, frm.Size.Height, BoundsSpecified.Location);
            frm.BringToFront();
        }*/

        private void AlertSound()
        {
            System.Media.SoundPlayer sp = new System.Media.SoundPlayer(popup_path);
            sp.Play();
        }
    }
}