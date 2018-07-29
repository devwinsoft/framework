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
using System.Reflection;
using Devarc;
using LitJson;

namespace TestClient
{
    public partial class Form1 : Form
    {
        public delegate void OnMessage(string _msg);
        StringBuilder mString = new StringBuilder();
        OnMessage mDelegate = null;

        class RMI_DATA
        {
            public Type type;
            public string name;
            public short rmi_id;
        }
        Dictionary<string, RMI_DATA> mRmiList = new Dictionary<string, RMI_DATA>();

        public Form1()
        {
            InitializeComponent();
            mDelegate = new OnMessage(onMessage);
            Log.OnMessage += OnLogMessage;
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
            textBox2.SelectionStart = textBox2.TextLength;
            textBox2.ScrollToCaret();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                string dllPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Protocol.Client.dll");
                Assembly assem = Assembly.LoadFile(dllPath);
                Type[] types = assem.GetTypes();
                for (int i = 0; i < types.Length; i++)
                {
                    if (types[i].GetInterfaces().Contains(typeof(IBaseObejct)))
                    {
                        NetProtocolAttribute attribute = types[i].GetCustomAttribute<NetProtocolAttribute>();
                        if (attribute == null)
                        {
                            Log.Error("{0} has not RMI_ID.", types[i].Name);
                            continue;
                        }
                        string displayName = types[i].FullName.Substring("Devarc.".Length);
                        RMI_DATA data = new RMI_DATA();
                        data.type = types[i];
                        data.name = displayName;
                        data.rmi_id = attribute.RMI_ID;
                        mRmiList.Add(data.name, data);
                        comboBox_rmi_name.Items.Add(displayName);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
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

        private void button_chat_Click(object sender, EventArgs e)
        {
            RMI_DATA data;
            if (mRmiList.TryGetValue(comboBox_rmi_name.Text, out data))
            {
                IBasePacket obj = (IBasePacket)Activator.CreateInstance(data.type);
                JsonData json = JsonMapper.ToObject(textBox_send.Text);
                obj.Initialize(json);
                NetBuffer msg = NetBufferPool.Instance.Pop();
                msg.Init((short)obj.RMI_ID, HostID.Server);
                obj.WriteTo(msg);
                SEND_RESULT result = TestClient.Instance.Proxy.Send(msg);
                if (result != SEND_RESULT.SUCCESS)
                {
                    Log.Info("Cannot send message. reason={0}", result);
                }
            }
        }

        private void button_clear_Click(object sender, EventArgs e)
        {
            mString.Clear();
            textBox2.Text = "";
        }

        private void comboBox_rmi_name_SelectedIndexChanged(object sender, EventArgs e)
        {
            RMI_DATA data;
            if (mRmiList.TryGetValue(comboBox_rmi_name.Text, out data))
            {
                textBox_rmi_id.Text = data.rmi_id.ToString();
                IBasePacket obj = (IBasePacket)Activator.CreateInstance(data.type);
                textBox_send.Text = obj.ToString();
            }
        }
    }
}
