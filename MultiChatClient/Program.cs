using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MultiChatClient {
    static class Program {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Login());
            //Home home = new Home();
            //Application.Run(home);
            Login login = new Login();
            if (login.ShowDialog() == DialogResult.OK)
            {
                string id = login.textBox1.Text;
                string nick = login.textBox5.Text;

                //부모폼 자식폼 종속하기
                Application.Run(new Home(id, nick));

                //부모폼 자식폼 독립하기
                //(new Home(id)).Show();
                //Application.Run();

            }

        }
    }
}
