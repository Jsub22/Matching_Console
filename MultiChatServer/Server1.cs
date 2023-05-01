using System;
using System.Net;
using System.Net.Sockets;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using MySql.Data.MySqlClient;

namespace MultiChatServer
{
    public partial class Server1 : Form
    {
        int first = 0;
        delegate void AppendTextDelegate(Control ctrl, string s);
        AppendTextDelegate _textAppender;
        Socket mainSock;
        IPAddress thisAddress;
        int roomnum = 100;
        string _server = "155.230.235.248"; //DB 서버 주소, 로컬일 경우 localhost
        int _port = 34056; //DB 서버 포트
        string _database = "swdb479"; //DB 이름
        string _id = "2019111479"; //계정 아이디
        string _pw = "2019111479"; //계정 비밀번호
        string _connectionAddress = "";

        public Server1()
        {
            InitializeComponent();
            mainSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            _textAppender = new AppendTextDelegate(AppendText);
            _connectionAddress = string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4}", _server, _port, _database, _id, _pw);
            listBox2.Items.Add("잡담방");
            listBox2.Items.Add("게임방");
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

        private void Server1_Load(object sender, EventArgs e)
        {
            IPHostEntry he = Dns.GetHostEntry(Dns.GetHostName());

            // 처음으로 발견되는 ipv4 주소를 사용한다.
            foreach (IPAddress addr in he.AddressList)
            {
                if (addr.AddressFamily == AddressFamily.InterNetwork)
                {
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


            try
            {
                using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                {
                    mysql.Open();
                    //accounts_table에 name, phone column 데이터를 삽입합니다. id는 자동으로 증가합니다.
                    MySqlCommand command = new MySqlCommand("SELECT * FROM t_loginlog", mysql);

                    MySqlDataReader R = command.ExecuteReader();

                    if (R.HasRows)
                    {
                        int i = 0;
                        while (R.Read())
                        {
                            try
                            {
                                using (MySqlConnection mydb = new MySqlConnection(_connectionAddress))
                                {
                                    mydb.Open();
                                    //accounts_table에 name, phone column 데이터를 삽입합니다. id는 자동으로 증가합니다.
                                    string insertQuery = string.Format("UPDATE t_loginlog SET m_fail='0' WHERE m_id='{0}'", R.GetString(0));

                                    MySqlCommand comm = new MySqlCommand(insertQuery, mydb);
                                    comm.ExecuteNonQuery();
                                    mydb.Close();
                                }
                            }
                            catch (Exception exc)
                            {
                                //MessageBox.Show(exc.Message);
                            }
                        }
                    }
                    R.Close();
                    mysql.Close();
                }
            }
            catch (Exception exc)
            {
                //MessageBox.Show(exc.Message);
            }


            lvList_OleDb_View();
        }

        void button2_Click(object sender, EventArgs e)
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
                MsgBoxHelper.Error("서버 시작 시 오류가 발생하였습니다.");
            }
        }

        List<Socket> connectedClients = new List<Socket>();
        void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                // 클라이언트의 연결 요청을 수락한다.
                Socket client = mainSock.EndAccept(ar);

                // 또 다른 클라이언트의 연결을 대기한다.
                mainSock.BeginAccept(AcceptCallback, null);

                AsyncObject obj = new AsyncObject(4096);
                obj.WorkingSocket = client;

                // 연결된 클라이언트 리스트에 추가해준다.
                connectedClients.Add(client);

                Socket socket = client;
                byte[] bDts = Encoding.UTF8.GetBytes("client;" + client.RemoteEndPoint.ToString());
                socket.Send(bDts);

                // 텍스트박스에 클라이언트가 연결되었다고 써준다.
                AppendText(connect, string.Format("{0}", client.RemoteEndPoint));

                // 클라이언트의 데이터를 받는다.
                client.BeginReceive(obj.Buffer, 0, 4096, 0, DataReceived, obj);
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
                textBox6.Text = text;
                textBox7.Text = msg;

                // 텍스트박스에 추가해준다.
                // 비동기식으로 작업하기 때문에 폼의 UI 스레드에서 작업을 해줘야 한다.
                // 따라서 대리자를 통해 처리한다.
                if (ip == "sign")
                {
                    //string[] info = msg.Split(';');
                    string[] sentence = msg.Split(':');

                    string[] info = sentence[0].Split(';'); // signup
                    string[] info2 = sentence[1].Split(';'); // prof
                    using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                    {
                        try
                        {
                            mysql.Open();
                            //accounts_table에 name, phone column 데이터를 삽입합니다. id는 자동으로 증가합니다.
                            string insertQuery = string.Format("INSERT INTO t_signup(m_id,m_pwd,m_name,m_birth,m_gender,m_phone) VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}'); ",
                            info[0], info[1], info[2], info[3], info[4], info[5]);

                            //insertQuery += string.Format("INSERT INTO t_loginlog VALUES('{0}','{1}','{2}');", info[0], info[1], "0");

                            insertQuery += string.Format("INSERT INTO t_profile(m_id, m_image, m_name, m_nickname, m_age, m_birth, m_gender, m_area, m_interest) VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');",
                                info2[0], info2[1], info2[2], info2[3], info2[4], info2[5], info2[6], info2[7], info2[8]);

                            insertQuery += string.Format("INSERT INTO t_loginlog(m_id, m_pwd, m_fail) VALUES('{0}', '{1}', '{2}');",
                                info[0], info[1], "0");

                            MySqlCommand command = new MySqlCommand(insertQuery, mysql);
                            command.ExecuteNonQuery();
                            mysql.Close();
                            //selectTable();
                        }
                        catch (Exception exc)
                        {
                            mysql.Close();
                            //MessageBox.Show(exc.Message);
                        }
                    }
                    lvList_OleDb_View();
                }
                else if (ip == "login")
                {
                    //MessageBox.Show("login 서버에 전달 완료");
                    int k = 0;
                    int mynick = 0;
                    int truefalse = 0;
                    string[] clientidpass = msg.Split(';');
                    textBox1.Text = clientidpass[1];
                    textBox2.Text = clientidpass[2];
                    listBox1.Items.Add(clientidpass[0]);
                    for (k = connectedClients.Count - 1; k >= 0; k--)
                    {
                        if (connectedClients[k].RemoteEndPoint.ToString() == clientidpass[0])
                        {
                            listBox1.Items.Add(connectedClients[k].RemoteEndPoint.ToString());
                            listBox1.Items.Add("동일 client 찾기 성공");
                            break;
                        }
                    }
                    foreach (ListViewItem item in listView1.Items)
                    {
                        if (item.SubItems[0].Text.Trim().Equals(textBox1.Text.Trim()))
                        {
                            if (item.SubItems[1].Text.Trim().Equals(textBox2.Text.Trim()))
                            {
                                truefalse = 1;
                                string[] client = connectedClients[k].RemoteEndPoint.ToString().Split(':');
                                string[] Client_f = textBox4.Text.Split(':');
                                //밑의 코드는 삭제하지 말기
                                //테스트 용이를 위해 주석처리를 한거다.
                                //이 코드는 한 컴퓨터에서 중복 로그인을 방지하고 현재 로그인한 사람을 알 수 있는 코드.
                                /*try
                                {
                                    using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                                    {
                                        mysql.Open();
                                        //accounts_table에 name, phone column 데이터를 삽입합니다. id는 자동으로 증가합니다.
                                        string insertQuery = string.Format("INSERT INTO t_login(m_id, m_pwd, m_client) VALUES ('{0}', '{1}', '{2}');", textBox1.Text, textBox2.Text, Client_f);

                                        MySqlCommand command = new MySqlCommand(insertQuery, mysql);
                                        command.ExecuteNonQuery();

                                        MessageBox.Show("로그인 정보 입력 완료 ");
                                        mysql.Close();
                                    }
                                }
                                catch (Exception exc)
                                {
                                    MessageBox.Show(exc.Message);
                                    truefalse = 0;
                                }*/
                                break;
                            }
                            else
                            {
                                item.SubItems[2].Text = (int.Parse(item.SubItems[2].Text) + 1).ToString();
                                if (int.Parse(item.SubItems[2].Text) > 5)
                                {
                                    MessageBox.Show("한 계정 로그인 실패 5번을 넘겼습니다.");
                                }
                                try
                                {
                                    using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                                    {
                                        mysql.Open();
                                        //accounts_table에 name, phone column 데이터를 삽입합니다. id는 자동으로 증가합니다.
                                        string insertQuery = string.Format("UPDATE t_loginlog SET m_fail='{0}' WHERE m_id='{1}'", item.SubItems[2].Text, item.SubItems[0].Text);

                                        MySqlCommand command = new MySqlCommand(insertQuery, mysql);
                                        command.ExecuteNonQuery();
                                        mysql.Close();
                                    }
                                }
                                catch (Exception exc)
                                {
                                    //MessageBox.Show(exc.Message);
                                    truefalse = 0;
                                }
                            }
                        }
                        /*
                        else if (item.SubItems[1].Text == textBox2.Text)
                        {
                            item.SubItems[2].Text = (int.Parse(item.SubItems[2].Text) + 1).ToString();

                            try
                            {
                                using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                                {
                                    mysql.Open();
                                    //accounts_table에 name, phone column 데이터를 삽입합니다. id는 자동으로 증가합니다.
                                    string insertQuery = string.Format("UPDATE t_loginlog SET m_fail='{0}' WHERE m_pwd='{1}'", item.SubItems[2].Text, item.SubItems[1].Text);

                                    MySqlCommand command = new MySqlCommand(insertQuery, mysql);
                                    command.ExecuteNonQuery();
                                    mysql.Close();
                                }
                            }
                            catch (Exception exc)
                            {
                                MessageBox.Show(exc.Message);
                                truefalse = 0;
                            }
                        }
                        */

                        mynick += 1;
                    }

                    byte[] bDts;
                    if (truefalse == 1)
                    {
                        bDts = Encoding.UTF8.GetBytes("login;true;" + listView4.Items[mynick].SubItems[3].Text);//맞으면 true. 로그인 해라.
                        listBox1.Items.Add("ok");
                        Socket socket = connectedClients[k];
                        socket.Send(bDts);//(true/false)를 보낸다.
                        listBox1.Items.Add(connectedClients[k].RemoteEndPoint.ToString());

                        listBox1.Items.Add("Client [login -> home]");
                        connectedClients.RemoveAt(k);

                        lvList_OleDb_View();
                    }
                    else
                    {
                        bDts = Encoding.UTF8.GetBytes("login;false");//틀리면 false. 로그인 하지 마라.
                        listBox1.Items.Add("no");

                        Socket socket = connectedClients[k];
                        socket.Send(bDts);//(true/false)를 보낸다.
                        listBox1.Items.Add(connectedClients[k].RemoteEndPoint.ToString());
                        lvList_OleDb_View();
                    }
                }

                else if (ip == "id_find")
                {
                    string[] namephone = msg.Split(';');
                    int k = 0;
                    textBox1.Text = namephone[1];
                    textBox2.Text = namephone[2];
                    for (k = connectedClients.Count - 1; k >= 0; k--)
                    {
                        if (connectedClients[k].RemoteEndPoint.ToString() == namephone[0])
                        {
                            listBox1.Items.Add(connectedClients[k].RemoteEndPoint.ToString());
                            listBox1.Items.Add("동일 client 찾기 성공");
                            break;
                        }
                    }
                    foreach (ListViewItem item in listView2.Items)
                    {
                        if (item.SubItems[2].Text == textBox1.Text && item.SubItems[5].Text == textBox2.Text)
                        {
                            textBox3.Text = "id_find;" + item.SubItems[0].Text + ";";
                            break;
                        }
                        else
                            textBox3.Text = "id_find;none";
                    }
                    byte[] bDts;
                    bDts = Encoding.UTF8.GetBytes(textBox3.Text);
                    Socket socket = connectedClients[k];
                    socket.Send(bDts);
                }
                else if (ip == "passwd_find")
                {
                    string[] namephone = msg.Split(';');
                    int i = 0;
                    int k = 0;
                    textBox1.Text = namephone[1];
                    textBox2.Text = namephone[2];
                    textBox4.Text = namephone[3];
                    for (k = connectedClients.Count - 1; k >= 0; k--)
                    {
                        if (connectedClients[k].RemoteEndPoint.ToString() == namephone[0])
                        {
                            listBox1.Items.Add(connectedClients[k].RemoteEndPoint.ToString());
                            listBox1.Items.Add("동일 client 찾기 성공");
                            break;
                        }
                    }
                    foreach (ListViewItem item in listView2.Items)
                    {
                        if (item.SubItems[0].Text == textBox1.Text && item.SubItems[2].Text == textBox2.Text && item.SubItems[5].Text == textBox4.Text)
                        {
                            textBox3.Text = "passwd_find;" + item.SubItems[1].Text;
                            break;
                        }
                        else
                            textBox3.Text = "passwd_find;none";
                    }
                    byte[] bDts;
                    bDts = Encoding.UTF8.GetBytes(textBox3.Text);
                    Socket socket = connectedClients[k];
                    socket.Send(bDts);
                    i++;
                }
                // profile 요청 정보
                else if (ip == "repro") // ("repro" + '\x01' + id + ";" + myclient);
                {
                    string[] spmsg = msg.Split(';');
                    int k = 0;

                    for (k = connectedClients.Count - 1; k >= 0; k--)
                    {
                        string conclient = connectedClients[k].RemoteEndPoint.ToString();

                        if (conclient.Equals(spmsg[1].Substring(0, conclient.Length))) // 동일(본인) 클라이언트 찾음
                        {
                            listBox1.Items.Add(connectedClients[k].RemoteEndPoint.ToString());
                            listBox1.Items.Add("동일 client 찾기 성공");
                            break;
                        }
                    }
                    string items = "";
                    using (MySqlConnection connection = new MySqlConnection(_connectionAddress))
                    {
                        string Query = string.Format("SELECT * FROM t_profile WHERE m_id='{0}'", spmsg[0]);
                        try//예외 처리
                        {
                            connection.Open();
                            MySqlCommand cmd = new MySqlCommand(Query, connection);

                            // 만약에 내가처리한 Mysql에 정상적으로 들어갔다면 메세지를 보여주라는 뜻
                            //if (cmd.ExecuteNonQuery() == 1)
                            //{command.ExecuteNonQuery();
                            cmd.ExecuteNonQuery();
                            MySqlDataReader reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                items = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8}",
                            reader["m_id"], reader["m_image"], reader["m_name"], reader["m_nickname"], reader["m_age"],
                            reader["m_birth"], reader["m_gender"], reader["m_area"], reader["m_interest"]);

                            }
                            reader.Close();
                            connection.Close();
                            byte[] bDts = Encoding.UTF8.GetBytes("repro;" + items); // 동일 아이디 정보 클라이언트로 보냄
                            Socket socket = connectedClients[k];
                            socket.Send(bDts);
                            listBox1.Items.Add(connectedClients[k].RemoteEndPoint.ToString());
                            listBox1.Items.Add("reprofile 정보 요청 성공");
                            //}
                        }
                        catch (Exception ex)
                        {
                            connection.Close();
                            listBox1.Items.Add("reprofile 정보 요청 실패\n" + ex);
                        }
                    }
                }
                // profile 정보 수정
                else if (ip == "prof") // ("proh" + '\x01' + prof_info);
                {
                    string[] spmsg = msg.Split(';');
                    int k = 0;
                    byte[] bDts;
                    Socket socket;

                    for (k = connectedClients.Count - 1; k >= 0; k--)
                    {
                        string conclient = connectedClients[k].RemoteEndPoint.ToString();

                        if (conclient.Equals(spmsg[9].Substring(0, conclient.Length))) // 동일(본인) 클라이언트 찾음
                                                                                       // if (conclient[0] == spmsg[0])
                        {
                            listBox1.Items.Add(connectedClients[k].RemoteEndPoint.ToString());
                            listBox1.Items.Add("동일 client 찾기 성공");
                            break;
                        }
                    }

                    using (MySqlConnection connection = new MySqlConnection(_connectionAddress))
                    {
                        string Query = string.Format("SELECT * FROM t_profile WHERE m_id<>'{0}' and m_nickname='{1}'", spmsg[0], spmsg[3]);
                        try//예외 처리
                        {
                            connection.Open();
                            MySqlCommand cmd = new MySqlCommand(Query, connection);

                            // 만약에 내가처리한 Mysql에 정상적으로 들어갔다면 메세지를 보여주라는 뜻
                            if (cmd.ExecuteNonQuery() == 1)
                            {
                                bDts = Encoding.UTF8.GetBytes("chnk;");
                                socket = connectedClients[k];
                                socket.Send(bDts);  // 클라이언트로 보냄

                                listBox1.Items.Add(connectedClients[k].RemoteEndPoint.ToString());
                                listBox1.Items.Add("동일 닉네임 존재");
                            }
                            else
                            {
                                Query = string.Format("set sql_safe_updates=0; UPDATE t_profile SET m_image={0}, m_nickname='{1}', m_area='{2}', m_interest='{3}' WHERE m_id='{4}'",
                                    spmsg[1], spmsg[3], spmsg[7], spmsg[8], spmsg[0]);
                                cmd = new MySqlCommand(Query, connection);
                                cmd.ExecuteNonQuery();
                                connection.Close();
                                //if (cmd.ExecuteNonQuery() == 1)
                                //{
                                bDts = Encoding.UTF8.GetBytes("succ;" + spmsg[3]);
                                socket = connectedClients[k];
                                socket.Send(bDts);  // 클라이언트로 보냄

                                listBox1.Items.Add("home 경로 프로필 수정 성공");
                                //}
                            }
                        }
                        catch (Exception ex)
                        {
                            connection.Close();
                            listBox1.Items.Add("reprofile 정보 요청 실패\n" + ex);
                        }
                    }

                    lvList_OleDb_View();
                }
                else if (ip == "logout") // 나의 클라이언트 번호
                {
                    listBox1.Items.Add(msg);

                    string[] info = msg.Split(';');

                    int k = 0;
                    for (k = listView3.Items.Count - 1; k >= 0; k--)
                    {
                        string item = listView3.Items[k].SubItems[2].Text.Trim();
                        if (item.Equals(info[0].Substring(0, item.Length)))
                            break;
                    }
                    listView3.Items.RemoveAt(k);

                    for (k = connectedClients.Count - 1; k >= 0; k--)
                    {
                        string clients = connectedClients[k].RemoteEndPoint.ToString().Trim();
                        if (clients.Equals(info[0].Substring(0, clients.Length)))
                            break;
                    }

                    connectedClients.RemoveAt(k);
                    listBox1.Items.Add("logout:" + connectedClients[k].RemoteEndPoint.ToString());
                }
                else if (ip == "id_client")
                {
                    //connect.Items.Add(msg);
                    string[] info = msg.Split(';');

                    ListViewItem lvt = new ListViewItem();
                    lvt.Text = info[0];
                    lvt.SubItems.Add(info[1]);
                    lvt.SubItems.Add(info[2]);
                    listView3.Items.Add(lvt);
                }
                else if (ip == "random") // 내 아이디, 내 클라이언트 번호
                {
                    //MessageBox.Show("일단 진입완료!!!!!!!!");
                    string[] randinfo = msg.Split(';');

                    textBox6.Text = listView3.Items.Count.ToString(); // 로그인 클라이언트 숫자
                    int k = 0;
                    int a = -1;

                    byte[] bDts;
                    Socket socket;

                    foreach (ListViewItem item in listView3.Items)
                    {
                        if (item.SubItems[0].Text.Trim().Equals(randinfo[0]))
                        {
                            break;
                        }
                        k += 1;
                    }

                    if (listView3.Items.Count > 1)
                    {
                        listBox1.Items.Add("--2명 이상 접속 중--");
                        
                        do //나 자신을 찾지 않기 위해
                        {
                            Random rand = new Random();
                            a = rand.Next(0, listView3.Items.Count);
                        }
                        while (a == k);

                        listBox1.Items.Add("k:" + k.ToString());
                        listBox1.Items.Add("a:" + a.ToString());
                        textBox7.Text = a.ToString(); // 랜덤 숫자

                        string cid = listView3.Items[a].SubItems[0].Text.Trim();
                        string caddr = listView3.Items[a].SubItems[2].Text.Trim();
                        
                        for (k = connectedClients.Count - 1; k >= 0; k--)
                        {
                            string conclient = connectedClients[k].RemoteEndPoint.ToString();
                            if (conclient.Equals(randinfo[1].Substring(0, conclient.Length)))
                            {
                                string items = "";

                                using (MySqlConnection connection = new MySqlConnection(_connectionAddress))
                                {
                                    string Query = string.Format("SELECT * FROM t_profile WHERE m_id='{0}'", cid);
                                    try//예외 처리
                                    {
                                        connection.Open();
                                        MySqlCommand cmd = new MySqlCommand(Query, connection);

                                        cmd.ExecuteNonQuery();
                                        MySqlDataReader reader = cmd.ExecuteReader();
                                        while (reader.Read())
                                        {
                                            items = string.Format("{0};{1};{2};{3};{4};{5};{6};",
                                        reader["m_image"], reader["m_nickname"], reader["m_age"],
                                        reader["m_gender"], reader["m_area"], reader["m_interest"], caddr);
                                            //listBox1.Items.Add(reader["m_gender"]);
                                            //items = caddr + ";" + reader["m_image"] + ";" + reader["m_nickname"] + ";" + reader["m_age"] + ";" + reader["m_gender"] + ";" + reader["m_area"] + ";" + reader["m_interest"];
                                        }
                                        reader.Close();
                                        connection.Close();
                                        listBox1.Items.Add("random;" + items);
                                        bDts = Encoding.UTF8.GetBytes("random;" + items); // 동일 아이디 정보 클라이언트로 보냄
                                        socket = connectedClients[k];
                                        socket.Send(bDts);
                                        listBox1.Items.Add(connectedClients[k].RemoteEndPoint.ToString());
                                        listBox1.Items.Add("클라이언트 정보 전송 성공");
                                        //}
                                    }
                                    catch (Exception ex)
                                    {
                                        connection.Close();
                                        listBox1.Items.Add("클라이언트 정보 전송 실패\n" + ex);
                                    }
                                }

                                obj.ClearBuffer();
                                obj.WorkingSocket.BeginReceive(obj.Buffer, 0, 4096, 0, DataReceived, obj);
                                return;
                            }
                        }
                        listBox1.Items.Add("신원 조회 불가로 매칭 실패");
                        bDts = Encoding.UTF8.GetBytes("randomcant;");
                        socket = connectedClients[k];
                        socket.Send(bDts);
                    }
                    else//접속한 사람이 한 사람 뿐일 때.
                    {
                        listBox1.Items.Add("--1 명 접속 중--");
                        listBox1.Items.Add("인원 수 미달로 매칭 실패");
                        bDts = Encoding.UTF8.GetBytes("randomcant;");
                        socket = connectedClients[k];
                        socket.Send(bDts);
                    }
                    //-------------------------------------------------------------------------------------------------------------------
                }
                else if (ip == "matchingput") // 상대 클라이언트 주소, 나의 클라이언트 주소, 나의 ID
                {
                    listBox1.Items.Add("지금 매칭 신청 누름");
                    string[] clients = msg.Split(';');

                    textBox6.Text = connectedClients.Count.ToString();
                    int k = 0;
                    for (k = connectedClients.Count - 1; k >= 0; k--)
                    {
                        string conclient = connectedClients[k].RemoteEndPoint.ToString();
                        if (conclient.Equals(clients[0].Substring(0, conclient.Length))) // 상대 찾음
                        {
                            listBox1.Items.Add(connectedClients[k].RemoteEndPoint.ToString());
                            listBox1.Items.Add("동일 client 찾기 성공");
                            break;
                        }
                    }

                    foreach (ListViewItem item in listView4.Items)
                    {
                        string itemid = item.SubItems[0].Text;
                        if (itemid.Equals(clients[1]))
                        {
                            textBox1.Text = clients[2]; // 클라이언트 주소
                            textBox2.Text = item.SubItems[1].Text; // 프로필 사진
                            textBox3.Text = item.SubItems[3].Text; // 닉네임
                            textBox4.Text = item.SubItems[4].Text; // 나이
                            textBox5.Text = item.SubItems[6].Text; // 성별
                            textBox6.Text = item.SubItems[7].Text; // 지역
                            textBox7.Text = item.SubItems[8].Text; // 취향
                            break;
                        }
                    }

                    byte[] bDts;
                    string str1 = string.Format("matchingput;{0};{1};{2};{3};{4};{5};{6};"
                        , textBox2.Text, textBox3.Text, textBox4.Text, textBox5.Text, textBox6.Text, textBox7.Text, textBox1.Text);
                    bDts = Encoding.UTF8.GetBytes(str1);
                    Socket socket = connectedClients[k];
                    socket.Send(bDts);
                    listBox1.Items.Add(str1);
                }
                else if (ip == "matching") // 나의 클라이언트 주소, 상대의 클라이언트 주소, 상대 응답
                {
                    listBox1.Items.Add("매칭 수락함");
                    string[] client12 = msg.Split(';');
                    int k;
                    byte[] bDts;
                    Socket socket;

                    if (client12[1] == "Yes")
                    { // 상대의 반응에 따른 전송 내용
                        bDts = Encoding.UTF8.GetBytes("roomnum;" + roomnum.ToString());
                        for (k = connectedClients.Count - 1; k >= 0; k--)
                        {
                            string conclient = connectedClients[k].RemoteEndPoint.ToString();
                            if (conclient.Equals(client12[0].Substring(0, conclient.Length))) // 나
                            {
                                listBox1.Items.Add(connectedClients[k].RemoteEndPoint.ToString());
                                socket = connectedClients[k];
                                socket.Send(bDts);
                            }
                            if (conclient.Equals(client12[2].Substring(0, conclient.Length))) // 상대
                            {
                                listBox1.Items.Add(connectedClients[k].RemoteEndPoint.ToString());
                                socket = connectedClients[k];
                                socket.Send(bDts);
                            }
                        }
                    }
                    else // client12[2] == "No"
                    {
                        bDts = Encoding.UTF8.GetBytes("failmatching;");
                        for (k = connectedClients.Count - 1; k >= 0; k--)
                        {
                            string conclient = connectedClients[k].RemoteEndPoint.ToString();
                            if (conclient.Equals(client12[0].Substring(0, conclient.Length))) // 나
                            {
                                listBox1.Items.Add(connectedClients[k].RemoteEndPoint.ToString());
                                socket = connectedClients[k];
                                socket.Send(bDts);

                                break;
                            }
                        }
                    }
                }
                else if (ip == "makeroom")
                {
                    listBox2.Items.Add(msg);
                    byte[] bDts;
                    bDts = Encoding.UTF8.GetBytes("roomname;" + msg);
                    for (int i = connectedClients.Count - 1; i >= 0; i--)
                    {
                        Socket socket = connectedClients[i];
                        try { socket.Send(bDts); }
                        catch
                        {
                            // 오류 발생하면 전송 취소하고 리스트에서 삭제한다.
                            try { socket.Dispose(); } catch { }
                            connectedClients.RemoveAt(i);
                        }
                    }
                }
                // for을 통해 "역순"으로 클라이언트에게 데이터를 보낸다.
                else
                {
                    connect.Items.Clear();
                    for (int i = connectedClients.Count - 1; i >= 0; i--)
                    {
                        Socket socket = connectedClients[i];
                        if (socket != obj.WorkingSocket)
                        {
                            try { socket.Send(obj.Buffer); }
                            catch
                            {
                                // 오류 발생하면 전송 취소하고 리스트에서 삭제한다.
                                try { socket.Dispose(); } catch { }
                                foreach (ListViewItem item in listView3.Items)
                                {
                                    if (connectedClients[i].RemoteEndPoint.ToString() == item.SubItems[2].Text)
                                    {
                                        //이 밑에 코드도 삭제하지 말것
                                        //로그인한 유저가 창을 닫을 때 현재 로그인 한 사람을 볼 수 있는 view에서 정보를 삭제

                                        try
                                        {
                                            string[] del = connectedClients[i].RemoteEndPoint.ToString().Split(':');
                                            using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                                            {
                                                mysql.Open();
                                                //accounts_table에 name, phone column 데이터를 삽입합니다. id는 자동으로 증가합니다.
                                                string insertQuery = string.Format("DELETE FROM t_login WHERE m_client = '{0}');", del[0]);

                                                MySqlCommand command = new MySqlCommand(insertQuery, mysql);
                                                command.ExecuteNonQuery();

                                                //selectTable();
                                            }
                                        }
                                        catch (Exception exc)
                                        {
                                            //MessageBox.Show(exc.Message);
                                        }
                                        lvList_OleDb_View();
                                        break;
                                    }
                                }
                                connectedClients.RemoveAt(i);
                            }
                        }
                        else
                            connect.Items.Add(connectedClients[i].RemoteEndPoint.ToString());
                    }
                }
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
        private void ChatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            mainSock.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ChatForm chatform = new ChatForm();
            chatform.Show();
        }

        private void lvList_OleDb_View()
        {
            listView1.Items.Clear();
            listView2.Items.Clear();
            //listView3.Items.Clear();
            listView4.Items.Clear();

            try
            {
                using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                {
                    mysql.Open();
                    //accounts_table에 name, phone column 데이터를 삽입합니다. id는 자동으로 증가합니다.
                    MySqlCommand command = new MySqlCommand("SELECT * FROM t_loginlog", mysql);

                    MySqlDataReader R = command.ExecuteReader();

                    if (R.HasRows)
                    {
                        int i = 0;
                        while (R.Read())
                        {
                            i = i + 1;
                            ListViewItem lvt = new ListViewItem();
                            lvt.Text = R.GetString(0);
                            lvt.SubItems.Add(R.GetString(1));
                            lvt.SubItems.Add(R.GetString(2));
                            listView1.Items.Add(lvt);
                        }
                    }
                    R.Close();
                    mysql.Close();
                }
            }
            catch (Exception exc)
            {
                //MessageBox.Show(exc.Message);
            }

            try
            {
                using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                {
                    mysql.Open();
                    //accounts_table에 name, phone column 데이터를 삽입합니다. id는 자동으로 증가합니다.
                    MySqlCommand command = new MySqlCommand("SELECT * FROM t_signup", mysql);

                    MySqlDataReader R = command.ExecuteReader();

                    if (R.HasRows)
                    {
                        int i = 0;
                        while (R.Read())
                        {
                            i = i + 1;
                            ListViewItem lvt = new ListViewItem();
                            lvt.Text = R.GetString(0);
                            lvt.SubItems.Add(R.GetString(1));
                            lvt.SubItems.Add(R.GetString(2));
                            lvt.SubItems.Add(R.GetString(3));
                            lvt.SubItems.Add(R.GetString(4));
                            lvt.SubItems.Add(R.GetString(5));
                            listView2.Items.Add(lvt);
                        }
                    }
                    R.Close();
                    mysql.Close();
                }
            }
            catch (Exception exc)
            {
                //MessageBox.Show(exc.Message);
            }

            try
            {
                using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                {
                    mysql.Open();
                    //accounts_table에 name, phone column 데이터를 삽입합니다. id는 자동으로 증가합니다.
                    MySqlCommand command = new MySqlCommand("SELECT * FROM t_profile", mysql);

                    MySqlDataReader R = command.ExecuteReader();

                    if (R.HasRows)
                    {
                        int i = 0;
                        while (R.Read())
                        {
                            i = i + 1;
                            ListViewItem lvt = new ListViewItem();
                            lvt.Text = R.GetString(0);
                            lvt.SubItems.Add(R.GetString(1));
                            lvt.SubItems.Add(R.GetString(2));
                            lvt.SubItems.Add(R.GetString(3));
                            lvt.SubItems.Add(R.GetString(4));
                            lvt.SubItems.Add(R.GetString(5));
                            lvt.SubItems.Add(R.GetString(6));
                            lvt.SubItems.Add(R.GetString(7));
                            lvt.SubItems.Add(R.GetString(8));
                            listView4.Items.Add(lvt);
                        }
                    }
                    R.Close();
                    mysql.Close();
                }
            }
            catch (Exception exc)
            {
                //MessageBox.Show(exc.Message);
            }
            try
            {
                using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                {
                    mysql.Open();
                    //accounts_table에 name, phone column 데이터를 삽입합니다. id는 자동으로 증가합니다.
                    MySqlCommand command = new MySqlCommand("SELECT * FROM t_login", mysql);

                    MySqlDataReader R = command.ExecuteReader();

                    if (R.HasRows)
                    {
                        int i = 0;
                        while (R.Read())
                        {
                            i = i + 1;
                            ListViewItem lvt = new ListViewItem();
                            lvt.Text = R.GetString(0);
                            lvt.SubItems.Add(R.GetString(1));
                            lvt.SubItems.Add(R.GetString(2));
                            listView3.Items.Add(lvt);
                        }
                    }
                    R.Close();
                    mysql.Close();
                }
            }
            catch (Exception exc)
            {
                //MessageBox.Show(exc.Message);
            }
        }
    }
}
