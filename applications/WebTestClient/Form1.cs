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
using System.Net;
using System.Reflection;
using Devarc;

namespace WebTestClient
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
            textBox_output.Text = mString.ToString();
            textBox_output.SelectionStart = textBox_user_seq.TextLength;
            textBox_output.ScrollToCaret();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                string dllPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Modules.Protocol.Client.dll");
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

        private void button_request_Click(object sender, EventArgs e)
        {
            DateTime sendTime = DateTime.Now;
            WebRequest request = WebRequest.Create(textBox_address.Text);
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";

            // Encoding
            Encoding encoding = Encoding.UTF8;
            string postData = string.Format("data={{\"user_seq\":\"{0}\",\"user_key\":\"{1}\",\"rmi_id\":\"{2}\",\"rmi_data\":\"{3}\"}}"
                , textBox_user_seq.Text, textBox_user_key.Text, textBox_rmi_id.Text, textBox_data.Text);
            byte[] result = encoding.GetBytes(postData);

            Stream postDataStream = request.GetRequestStream();
            postDataStream.Write(result, 0, result.Length);
            postDataStream.Close();

            // Response
            try
            {
                mString.Clear();
                textBox_output.Clear();
                Log.Info(sendTime.ToString("[yyyy-MM-dd HH:mm:ss]"));
                Log.Info(postData);
                Log.Info("");

                WebResponse response = request.GetResponse();
                Stream respPostStream = response.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, Encoding.Default);
                string resultPost = readerPost.ReadToEnd();

                readerPost.Close();
                respPostStream.Close();

                DateTime recvTime = DateTime.Now;
                Log.Info("{0} ElapsedTime={1}sec", recvTime.ToString("[yyyy-MM-dd HH:mm:ss]"), (recvTime - sendTime).Milliseconds / 1000f);
                Log.Info(resultPost);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        private void button_clear_Click(object sender, EventArgs e)
        {
            mString.Clear();
            textBox_output.Clear();
        }

        private void comboBox_rmi_name_SelectedIndexChanged(object sender, EventArgs e)
        {
            RMI_DATA data;
            if (mRmiList.TryGetValue(comboBox_rmi_name.Text, out data))
            {
                textBox_rmi_id.Text = data.rmi_id.ToString();
            }
        }
    }
}
