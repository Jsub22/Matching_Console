using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;

namespace MultiChatServer {
    //https://slaner.tistory.com/170?category=546117
    public partial class ChatForm : Form {
        delegate void AppendTextDelegate(Control ctrl, string s);
        AppendTextDelegate _textAppender;
        Socket mainSock;
        IPAddress thisAddress;
        int roomnum = 0;

        public ChatForm() {
            InitializeComponent();
            mainSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            _textAppender = new AppendTextDelegate(AppendText);
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
            foreach (IPAddress addr in he.AddressList) {
                if (addr.AddressFamily == AddressFamily.InterNetwork) {
                    thisAddress = addr;
                    break;
                }    
            }

            // 주소가 없다면..
            if (thisAddress == null)
                // 로컬호스트 주소를 사용한다.
                thisAddress = IPAddress.Loopback;

            txtAddress.Text = thisAddress.ToString();
            txtPort.Focus();
        }
        void BeginStartServer(object sender, EventArgs e) 
        {
            try
            {
                int port;
                if (!int.TryParse(txtPort.Text, out port))
                {
                    MsgBoxHelper.Error("포트 번호가 잘못 입력되었거나 입력되지 않았습니다.");
                    txtPort.Focus();
                    txtPort.SelectAll();
                    return;
                }

                //바인딩 되어있는지 확인
                if (mainSock.IsBound)
                {
                    MsgBoxHelper.Info("이미 서버에 연결되어 있습니다!");
                    return;
                }

                // 서버에서 클라이언트의 연결 요청을 대기하기 위해
                // 소켓을 열어둔다.

                IPEndPoint serverEP = new IPEndPoint(thisAddress, port);
                mainSock.Bind(serverEP);
                mainSock.Listen(10);

                // 비동기적으로 클라이언트의 연결 요청을 받는다.
                mainSock.BeginAccept(AcceptCallback, null);
                MsgBoxHelper.Info("서버가 구동 되었습니다!");
            }
            catch
            {
                MsgBoxHelper.Error("서버 시작시 오류가 발생하였습니다.");
            }

        }

        List<Socket> connectedClients = new List<Socket>();

        //따로 추가한 내용
        //전체 소캣이 아닌 채팅방 소켓만 따로 가져오기
        List<Socket> chat_connectedClients = new List<Socket>();
        //서버에 저장되는 닉네임
        List<String> My_NICK = new List<string>();
        // 서버에 저장되는 채팅방 아이디
        List<String> My_Chat = new List<string>();


        void AcceptCallback(IAsyncResult ar) {

            try
            {
                // 클라이언트의 연결 요청을 수락한다.
                Socket client = mainSock.EndAccept(ar);

                // 또 다른 클라이언트의 연결을 대기한다.
                mainSock.BeginAccept(AcceptCallback, null);

                AsyncObject obj = new AsyncObject(50000);
                obj.WorkingSocket = client;

                // 연결된 클라이언트 리스트에 추가해준다.
                connectedClients.Add(client);
                Socket socket = client;
                byte[] bDts = Encoding.UTF8.GetBytes("client;"+client.RemoteEndPoint.ToString());
                socket.Send(bDts);

                    // 텍스트박스에 클라이언트가 연결되었다고 써준다.
                    AppendText(txtHistory, string.Format("클라이언트 (@ {0})가 연결되었습니다.", client.RemoteEndPoint));

                // 클라이언트의 데이터를 받는다.
                client.BeginReceive(obj.Buffer, 0, 50000, 0, DataReceived, obj);
            }
            catch 
            {
                //
                return;
            }
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
                // tokens[0] - 보낸 사람 IP
                // tokens[1] - 보낸 메세지
                string[] tokens = text.Split('\x01');
                string ip = tokens[0];
                string msg = tokens[1];
                textBox1.Text = text;
                textBox2.Text = msg;

                //방에서 입장할 경우 입장했다고 값 보내기
                if (ip == "Input")
                {
                    //채팅방 변수값과 아이디, 해당 소캣값을 각각 리스트에 저장해준다.
                    My_NICK.Add(tokens[1].Replace("\0", ""));
                    My_Chat.Add(tokens[2].Replace("\0", ""));

                    chat_connectedClients.Add(connectedClients[connectedClients.Count - 1]);


                    //for문을 돌려서 해당 채팅방 변수값이면서 아이디가 다른 모든 채팅방에 해당 유저가 들어왔다고 알려준다.
                    for (int i = chat_connectedClients.Count - 1; i >= 0; i--)
                    {
                        if (My_Chat[My_Chat.Count - 1] == My_Chat[i])
                        {
                            Socket socket = chat_connectedClients[i];
                            String Temp = tokens[1].Replace("\0", "");

                            byte[] bDts = Encoding.UTF8.GetBytes("Input" + '\x01' + Temp);
                            if (socket != obj.WorkingSocket)
                            {
                                try { socket.Send(bDts); }
                                catch
                                {
                                }
                            }
                        }
                    }
                }

                // 방에서 퇴장할 경우 퇴장했다고 값 보내기
                else if (ip == "Output")
                {

                    //만들었던 모든 리스트에서 채팅방에서 나가는 유저의 정보를 삭제한다.
                    for (int i = My_Chat.Count - 1; i >= 0; i--)
                    {
                        if (My_NICK[i] == tokens[1].Replace("\0", ""))
                        {
                            if (My_Chat[i] == tokens[2].Replace("\0", ""))
                            {
                                My_Chat.RemoveAt(i);
                                My_NICK.RemoveAt(i);
                                chat_connectedClients.RemoveAt(i);
                            }
                        }
                    }


                    //for문을 돌려서 해당 채팅방 변수값이면서 아이디가 다른 모든 채팅방에 해당 유저가 퇴장한다고 알려준다.
                    for (int i = chat_connectedClients.Count - 1; i >= 0; i--)
                    {
                        if (tokens[2].Replace("\0", "") == My_Chat[i])
                        {
                            if (tokens[1].Replace("\0", "") != My_NICK[i])
                            {
                                Socket socket = chat_connectedClients[i];
                                byte[] bDts = Encoding.UTF8.GetBytes("Output" + '\x01' + tokens[1]);
                                if (socket != obj.WorkingSocket)
                                {
                                    try { socket.Send(bDts); }
                                    catch
                                    {
                                    }
                                }
                            }
                        }
                    }
                }

                //채팅 정보 받아오기
                else if (ip == "ChatInfo")
                {

                    int Count = 0;
                    // 채팅방 인원 및 닉네임 모음 string을 만들어준다.
                    String InfoMember = "채팅방 인원 닉네임 \r";

                    for (int i = chat_connectedClients.Count - 1; i >= 0; i--)
                    {
                        if (tokens[2].Replace("\0", "") == My_Chat[i])
                        {
                            InfoMember = InfoMember + "- " + My_NICK[i] + "\r";
                            Count++;
                        }
                    }
                    
                    InfoMember = "현재 채팅방 인원수 : " + Count + "\r" + InfoMember;


                    //INFO 값을 요청한 해당 유저에게만 그대로 값을 넘겨준다..
                    for (int i = chat_connectedClients.Count - 1; i >= 0; i--)
                    {
                        if (tokens[2].Replace("\0", "") == My_Chat[i])
                        {
                            if (tokens[1].Replace("\0", "") == My_NICK[i])
                            {
                                Socket socket = chat_connectedClients[i];
                                byte[] bDts = Encoding.UTF8.GetBytes("ChatInfo" + '\x01' + InfoMember);
                                try { socket.Send(bDts); }
                                catch
                                {
                                }
                            }
                        }
                    }


                }

                else if (ip == "ChatText")
                {
                    string id = tokens[1];
                    string date = tokens[2];
                    msg = tokens[3];
                    string Chat_ID = tokens[4].TrimEnd();
                    AppendText(txtHistory, string.Format("[받음] {0} {1} : {2} ... {3}", id, date, msg, Chat_ID));

                    for (int i = chat_connectedClients.Count - 1; i >= 0; i--)
                    {
                        if (Chat_ID.Replace("\0", "") != My_Chat[i]) continue;

                        Socket socket = chat_connectedClients[i];
                        if (socket != obj.WorkingSocket)
                        {
                            try { socket.Send(obj.Buffer); }
                            catch
                            {
                                try { socket.Dispose(); } catch { }
                                for (int j = connectedClients.Count - 1; j >= 0; j--)
                                {
                                    if (chat_connectedClients[i] == connectedClients[j])
                                    {
                                        connectedClients.RemoveAt(j);
                                    }
                                }
                                chat_connectedClients.RemoveAt(i);
                                My_NICK.RemoveAt(i);
                                My_Chat.RemoveAt(i);
                            }
                        }
                    }
                }

                else if (ip == "SecChat")
                {
                    for (int i = chat_connectedClients.Count - 1; i >= 0; i--)
                    {
                        if (tokens[1].Replace("\0", "") == My_NICK[i])
                        {
                            Socket socket = chat_connectedClients[i];
                            byte[] bDts = Encoding.UTF8.GetBytes("SecChat" + '\x01' + tokens[2] + '\x01' + tokens[3] + '\x01' + tokens[4] + '\x01' + tokens[5]);
                            try { socket.Send(bDts); }
                            catch
                            {
                            }
                        }
                    }
                }

                else
                { 
                    string Chat_ID = tokens[3].TrimEnd();
                    byte[] bDts = obj.Buffer;
                    MemoryStream ms = new MemoryStream(bDts);

                    for (int i = connectedClients.Count - 1; i >= 0; i--)
                    {

                        Socket socket = connectedClients[i];
                        if (socket != obj.WorkingSocket)
                        {
                            try { socket.Send(obj.Buffer); }
                            catch
                            {
                                try { socket.Dispose(); } catch { }
                                connectedClients.RemoveAt(i);
                            }
                        }
                    }

                }

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
            
            // 문자열을 utf8 형식의 바이트로 변환한다.
            byte[] bDts = Encoding.UTF8.GetBytes("ChatText" + '\x01' + "Server Host" + '\x01' + tts);

            // 연결된 모든 클라이언트에게 전송한다.
            for (int i = connectedClients.Count - 1; i >= 0; i--) {
                Socket socket = connectedClients[i];
                try { socket.Send(bDts); } catch {
                    // 오류 발생하면 전송 취소하고 리스트에서 삭제한다.
                    try { socket.Dispose(); } catch { }
                    connectedClients.RemoveAt(i);
                }
            }

            // 전송 완료 후 텍스트박스에 추가하고, 원래의 내용은 지운다.
            AppendText(txtHistory, string.Format("[보냄] {0} : {1}", "Server Host", tts));
            txtTTS.Clear();
        }

        private void ChatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            mainSock.Close();
        }

        private void txtTTS_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OnSendData(sender, e);
            }
        }
        
        private void button1_Click(object sender, EventArgs e)//listBox 정보를 txt파일로 내보내는 버튼.
        {
        }
    }
}