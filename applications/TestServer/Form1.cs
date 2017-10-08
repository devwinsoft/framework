using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Devarc;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;

namespace TestServer
{
    public partial class Form1 : Form
    {
        public delegate void OnMessage(string _msg);

        StringBuilder mString = new StringBuilder();
        OnMessage mDelegate = null;

        public Form1()
        {
            InitializeComponent();
            Log.SetCallback(OnLogMessage);
            mDelegate = new OnMessage(onMessage);
        }

        void OnLogMessage(LOG_TYPE tp, string msg)
        {
            this.Invoke(mDelegate, new object[] { msg });
        }
        void onMessage(string msg)
        {
            mString.Append(msg);
            mString.Append("\r\n");
            textBox1.Text = mString.ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox_port.Text = TestServer.Instance.Config.Port.ToString();
        }

        private void button_start_Click(object sender, EventArgs e)
        {
            TestServer server = TestServer.Instance;
            switch (server.State)
            {
                case ServerState.NotInitialized:
                case ServerState.NotStarted:
                    server.Start();
                    break;
                default:
                    break;
            }
        }

        private void button_stop_Click(object sender, EventArgs e)
        {
            TestServer.Instance.Stop();
        }

        private void button_clear_Click(object sender, EventArgs e)
        {
            mString.Clear();
            textBox1.Text = mString.ToString();
        }

        private void button_test_Click(object sender, EventArgs e)
        {
            if (TestServer.Instance.State != ServerState.Running)
                return;
            foreach (NetSession session in TestServer.Instance.GetAllSessions())
            {
                TestServer.Instance.Proxy.Notify_Chat(session.Hid, "TEST OK");
            }
        }
    }
}
