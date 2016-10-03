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
            Log.SetCallback(OnLogMessage);
        }

        private void button_start_Click(object sender, EventArgs e)
        {
            int port = 5000;
            if (int.TryParse(textBox_port.Text, out port) && TestServer.Instance.IsRunning == false)
            {
                TestServer.Instance.Run(port);
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            TestServer.Instance.server.Tick();
        }

    }
}
