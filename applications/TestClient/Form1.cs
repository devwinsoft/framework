﻿using System;
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            TestClient.Instance.client.Tick(0.001f * timer1.Interval);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TestClient.Instance.client.Disconnect();
        }

        private void button_select_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TableData.UnLoad_TestSchema();
                TableData.Load_TestSchema_XmlFile(openFileDialog1.FileName);
                //TableData.UnLoad_ClientObject();
                //TableData.Load_ClientObject_JSON(openFileDialog1.FileName + ".json");

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < T_DataCharacter.LIST.Count;i++ )
                {
                    sb.Append(T_DataCharacter.LIST.ElementAt(i).ToJson().Replace("\n","\\n"));
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
                TableData.Save_TestSchema_XmlFile(saveFileDialog1.FileName);
                TableData.Save_TestSchema_JsonFile(saveFileDialog1.FileName + ".json");
            }
        }

        private void button_chat_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox_chat.Text) == false)
            {
                TestClient.C2Test.Chat(HostID.Server, textBox_name.Text, textBox_chat.Text);
                textBox_chat.Text = "";
            }
        }
    }
}
