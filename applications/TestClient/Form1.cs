using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Devarc;

namespace TestClient
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
            Log.SetCallback(OnLogMessage);
        }

        void OnLogMessage(LOG_TYPE tp, string msg)
        {
            this.Invoke(mDelegate, new object[] { msg });
        }
        void onMessage(string msg)
        {
            mString.Append(msg);
            mString.Append("\r\n");
            textBox2.Text = mString.ToString();
        }

        private void button_connect_Click(object sender, EventArgs e)
        {
            int _port = 0;
            if (int.TryParse(textBox_port.Text, out _port))
            {
                TestClient.Instance.Connect(textBox_addresss.Text, _port);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TestClient.Instance.Disconnect();
        }

        private void button_select_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Table.UnLoad_TestSchema();
                Table.Load_TestSchema_SheetFile(openFileDialog1.FileName);
                //TableData.UnLoad_ClientObject();
                //TableData.Load_ClientObject_JSON(openFileDialog1.FileName + ".json");

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < Table.T_DataCharacter.Count;i++ )
                {
                    sb.Append(Table.T_DataCharacter.ElementAt(i).ToJson().Replace("\n","\\n"));
                    sb.Append("\r\n");
                }
                textBox2.Text = sb.ToString();

                //FileStream fs = File.OpenRead(openFileDialog1.FileName);
                //byte[] buf = new byte[fs.Length];
                //fs.Read(buf, 0, buf.Length);
                //fs.Close();
                //C2Test.Proxy.SendFile(HostID.Server, openFileDialog1.SafeFileName, buf);

            }
        }
        private void button_send_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Table.Save_TestSchema_SheetFile(saveFileDialog1.FileName);
                Table.Save_TestSchema_JsonFile(saveFileDialog1.FileName + ".json");
            }
        }

        private void button_chat_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox_chat.Text) == false)
            {
                TestClient.Instance.Proxy.Chat(HostID.Server, textBox_chat.Text);
                //textBox_chat.Text = "";
            }
        }
    }
}
