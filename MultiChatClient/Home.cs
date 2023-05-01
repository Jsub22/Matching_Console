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
    public partial class Home : Form
    {
        string myclient;
        public string id, nick, matchingcall;
        //string roomnum;
        delegate void AppendTextDelegate(Control ctrl, string s);
        AppendTextDelegate _textAppender;
        Socket mainSock;
        Profile prof;
        RandomMatching randmatch;
        string[] str = new string[7];
        RandomMatching rm;

        //채팅방 기본 변수 ID값을 저장하는 리스트
        List<String> Chat_ID = new List<String>();
        //열린 채팅방 폼 값
        public List<ChatForm> List_ChatForm = new List<ChatForm>();

        public Home(string ID, string NICK)
        {
            InitializeComponent();
            id = ID;
            nick = NICK;

            //MessageBox.Show("(home) id:" + id + ", nick:" + nick);

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

            try { mainSock.Connect(defaultHostAddress, 9999); }//여기서 클라이언트가 서버와 연결
            //디폴트 호스트 IP주소 확인하기

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


            //기본 데이터 저장하기
            Chat_Data.Items.Add("잡담방");
            Chat_ID.Add("1");
            Chat_Data.Items.Add("게임방");
            Chat_ID.Add("2");

            //ListBox 레이아웃 변경을 위한 다시그리기 함수
            Chat_Data.DrawMode = DrawMode.OwnerDrawFixed;
            Chat_Data.ItemHeight = 30;

            //홈쪽에 본인 닉네임, ID값 적어주기
            IDTEXT.Text = nick + "(" + id + ")";
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

        public void Send(IAsyncResult result)
        {
            if (!mainSock.IsBound)
            {
                MsgBoxHelper.Warn("서버가 실행되고 있지 않습니다!");
                return;
            }

            mainSock.EndSend(result);
        }

        public void OnSendData(byte[] bDts, string str) // Profile 쪽에서 받아옴
        {
            if (!mainSock.IsBound)
            {
                MsgBoxHelper.Warn("서버가 실행되고 있지 않습니다!");
                return;
            }
            //MessageBox.Show(str + "에 의한 Send");

            //mainSock.Send(bDts, bDts.Length, SocketFlags.None);
            mainSock.BeginSend(bDts, 0, 4096, 0, new AsyncCallback(Send), mainSock);//여기
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
                //textBox4.Text = recievetext[1];
                //textBox3.Text = text;

                if (recievetext[0] == "client")//서버에서 보내준 본인 클라이언트 번호(ex)129.258.7.8:2540)
                {
                    textBox2.Text = recievetext[1];
                    myclient = recievetext[1];
                    obj.ClearBuffer();

                    string ic_info = id + ";" + nick + ";" + myclient + ";";
                    byte[] bDts = Encoding.UTF8.GetBytes("id_client" + '\x01' + ic_info);

                    OnSendData(bDts, "id_client");
                }
                else if (recievetext[0] == "random")//여기
                {
                    textBox3.Text = text;
                    str[0] = recievetext[7]; // 랜덤 클라이언트 주소
                    str[1] = recievetext[1]; // 랜덤 클라이언트 사진
                    str[2] = recievetext[2]; // 랜덤 클라이언트 닉네임
                    str[3] = recievetext[3]; // 랜덤 클라이언트 나이
                    str[4] = recievetext[4]; // 랜덤 클라이언트 성별
                    str[5] = recievetext[5]; // 랜덤 클라이언트 지역
                    str[6] = recievetext[6]; // 랜덤 클라이언트 관심사

                    text = "송신자;" + text;

                    randmatch.SetRandMatching(text);
                    //MessageBox.Show("랜덤 매칭으로 정보 넣기");
                }
                else if (recievetext[0] == "matchingput")//여기
                {
                    str[0] = recievetext[7]; // 랜덤 클라이언트 주소
                    str[1] = recievetext[1]; // 랜덤 클라이언트 프로필 사진
                    str[2] = recievetext[2]; // 랜덤 클라이언트 닉네임
                    str[3] = recievetext[3]; // 랜덤 클라이언트 나이
                    str[4] = recievetext[4]; // 랜덤 클라이언트 성별
                    str[5] = recievetext[5]; // 랜덤 클라이언트 지역
                    str[6] = recievetext[6]; // 랜덤 클라이언트 관심사

                    matchingcall = "수신자;" + text;
                    button2.Text = "매칭 요청";
                }
                else if (recievetext[0] == "randomcant")
                {
                    MsgBoxHelper.Warn("접속한 사람이 아무도 없습니다.");
                    textBox3.Text = "접속한 사람 없음";
                    randmatch.Close();
                }
                else if (recievetext[0] == "roomnum")
                {
                    randmatch.Close();
                    Chat_Data.SelectedItem = null;
                    //Listbox 값 선택을 초기화해 채팅방 이름을 랜덤채팅으로 만든다. 

                    ChatForm chat = new ChatForm(this, recievetext[1]);
                    chat.Show();

                    //채팅방 변수 내역을 각각 Listbox와 채팅방 변수 List에 저장한다.
                    Chat_Data.Items.Add(recievetext[2] + "님과의 채팅방");
                    Chat_ID.Add(recievetext[1]);
                    List_ChatForm.Add(chat);
                }
                else if (recievetext[0] == "failmatching")
                {
                    rm.Close(); // 거절 의사 밝힐 시 상대 매칭창 닫아버림
                    ReMatching();
                }
                else if (recievetext[0] == "repro") // 정보 받아옴
                {
                    prof.SetProfile(text);
                    //MessageBox.Show("홈으로 정보 들고 옴");
                }

                else if (recievetext[0] == "chnk") // 닉네임 중복 검사
                {
                    //MessageBox.Show("이미 존재하는 닉네임입니다.");
                }
                else if (recievetext[0] == "succ") // 성공
                {
                    //MessageBox.Show("프로필을 변경하였습니다.");

                    nick = recievetext[1];
                    IDTEXT.Text = nick + "(" + id + ")";
                    Invalidate();
                }

                else if (recievetext[0] == "roomname")
                {
                    Chat_Data.Items.Add(recievetext[1]);
                    Chat_ID.Add((Chat_ID.Count + 1).ToString());
                    Chat_Data.DrawMode = DrawMode.OwnerDrawFixed;
                    Chat_Data.ItemHeight = 30;
                }

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

        private void button1_Click(object sender, EventArgs e) // 프로필 설정
        {
            foreach (Form openForm in Application.OpenForms)
            {
                if (openForm.Name == "Profile") // 다중 폼 검사
                {
                    openForm.Activate();
                    return;
                }
            }
            string prof_info = id + ";" + myclient + ";";
            //.Show(prof_info + " (Profile : prof_info)");

            // 문자열을 utf8 형식의 바이트로 변환한다.
            byte[] bDts = Encoding.UTF8.GetBytes("repro" + '\x01' + prof_info);

            // 서버에 전송한다.
            OnSendData(bDts, "repro");

            Profile profile = new Profile(id, myclient, this);
            prof = profile;
            profile.Show();
        }

        public void button2_Click(object sender, EventArgs e)//랜덤 매칭
        {
            //MessageBox.Show(button2.Text);
            if (button2.Text == "매칭 요청")
            {
                foreach (Form openForm in Application.OpenForms)
                {
                    if (openForm.Name == "RandomMatching") // 다중 폼 검사
                    {
                        openForm.Activate();
                        return;
                    }
                }
                button2.Text = "랜덤 매칭";
                RandomMatching randommatching = new RandomMatching(this, matchingcall);
                randmatch = randommatching;
                randommatching.Show();
            }
            else
            {
                foreach (Form openForm in Application.OpenForms)
                {
                    if (openForm.Name == "RandomMatching") // 다중 폼 검사
                    {
                        openForm.Activate();
                        return;
                    }
                }
                byte[] bDts = Encoding.UTF8.GetBytes("random" + '\x01' + id + ";" + myclient);
                OnSendData(bDts, "random");
                textBox5.Text = "랜덤 누름";
                string text = "";
                RandomMatching randommatching = new RandomMatching(this, text);
                randmatch = randommatching;
                randommatching.Show();
            }
        }
        public void ReMatching()
        {
            byte[] bDts = Encoding.UTF8.GetBytes("random" + '\x01' + id + ";" + myclient);
            OnSendData(bDts, "random");
            textBox5.Text = "랜덤 누름";
            string text = "";
            RandomMatching randommatching = new RandomMatching(this, text);
            randmatch = randommatching;
            randommatching.Show();
        }

        public void MatchingCall(string text)
        {
            RandomMatching randommatching = new RandomMatching(this, text);
            randmatch = randommatching;
            randommatching.Show();
        }

        //Chat_Data 리스트박스 새로 그리기
        private void Chat_Data_DrawItem(object sender, DrawItemEventArgs e)
        {
            StringFormat strfmt = new StringFormat();
            strfmt.LineAlignment = StringAlignment.Center;
            strfmt.Alignment = StringAlignment.Center;

            e.DrawBackground();
            //e.Graphics.DrawString(Chat_Data.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), e.Bounds);
            //기본 값 설정

            ListBox lst = (ListBox)sender;
            e.Graphics.DrawString(lst.Items[e.Index].ToString(),
                 new Font("맑은 고딕", 10, FontStyle.Bold),
                 new SolidBrush(e.ForeColor), e.Bounds, strfmt);
            e.DrawFocusRectangle();
        }

        //채팅방 목록을 클릭했을때 채팅방이 열리도록 하기
        private void Chat_Data_DoubleClick(object sender, EventArgs e)
        {

            foreach (Form openForm in Application.OpenForms)
            {
                if (openForm.Name == Chat_ID[Chat_Data.SelectedIndex]) // 열린 폼의 이름 검사
                {
                    if (openForm.WindowState == FormWindowState.Minimized)
                    {  // 폼을 최소화시켜 하단에 내려놓았는지 검사
                        openForm.WindowState = FormWindowState.Normal;
                        openForm.Location = new Point(this.Location.X + this.Width, this.Location.Y);
                    }
                    openForm.Activate();
                    return;
                    // 만약 폼이 활성화되어있을 경우 해당 폼을 띄워준다.
                }
            }

            ChatForm CF = new ChatForm(this, Chat_ID[Chat_Data.SelectedIndex]);
            List_ChatForm.Add(CF);
            // 만약 폼이 실행되지 않았을 경우 New Form 객체 생성

            CF.StartPosition = FormStartPosition.Manual;  // 원하는 위치를 직접 지정해서 띄우기 위해
            CF.Location = new Point(this.Location.X + this.Width, this.Location.Y); // 메인폼의 오른쪽에 위치하도록 만듦
            CF.Show();

            Chat_Data.SelectedItem = null;
            // Listbox 선택 값 초기화
        }

        public void Matching(string result)
        {//내 클라이언트, 결과, 상대 클라이언트
            byte[] bDts = Encoding.UTF8.GetBytes("matching" + '\x01' + myclient + ";" + result  + ";" + str[0]+ ";");
            OnSendData(bDts, "matching");
        }

        public void MatchingPut() // 매칭 신청 (송신자의 요청)
        {   // 상대의 클라이언트 번호, 나의 클라이언트 번호, 나의 아이디
            byte[] bDts = Encoding.UTF8.GetBytes("matchingput" + '\x01' + str[0] + ";" + id  + ";" + myclient+ ";");
            OnSendData(bDts, "matchingput");
        }

        private void button3_Click(object sender, EventArgs e) // 방 만들기
        {
            RoomMaker roommaker = new RoomMaker(this);
            roommaker.Show();
        }

        private void button4_Click(object sender, EventArgs e) // 로그아웃
        {
            /*
            foreach (Home openForm in Application.OpenForms)
            {
                openForm.Home_FormClosing(sender, e);
            }
            */
            /*
            foreach (ChatForm openForm in List_ChatForm)
            {
                openForm.HomeChatForm_FormClosing(sender, e);
            }

            byte[] bDts = Encoding.UTF8.GetBytes("logout" + '\x01' + myclient + ";");
            OnSendData(bDts, "logout");
            */
        }

        // 종속 관계에 있는 모든 폼 종료시 해당 방에서 퇴장한다는 알림을 남기게 만들어준다.
        private void Home_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!mainSock.IsBound)
            {
                mainSock.Close();
                return;
            }

            if (MessageBox.Show("정말 로그아웃 하시겠습니까?", "Logout",
                MessageBoxButtons.YesNo) == DialogResult.Yes)

            {
                foreach (ChatForm openForm in List_ChatForm)
                {
                    openForm.HomeChatForm_FormClosing(sender, e);
                }

                byte[] bDts = Encoding.UTF8.GetBytes("logout" + '\x01' + myclient + ";");
                OnSendData(bDts, "logout");
                mainSock.Close();
            }
            else
            {
                e.Cancel = true;
            }
        }
    }
}
