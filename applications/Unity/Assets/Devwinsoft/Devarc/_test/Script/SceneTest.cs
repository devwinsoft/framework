using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Devarc;

public class SceneTest : MonoBehaviour
{
    public C2S.Proxy net_C2S = new C2S.Proxy();
    public NetClient client = new NetClient();
    Stub_S2C stub = new Stub_S2C();

    string mAddress = "127.0.0.1";
    string mPort = "5000";
    string mMessage = "";
    List<string> mMessageList = new List<string>();
    StringBuilder mBuilder = new StringBuilder();

    void Start()
    {
        Log.SetCallback(callback_Message);
        client.InitStub(this.stub);
    }

    void OnGUI()
    {
        mAddress = GUI.TextArea(new Rect(20, 20, 130, 20), mAddress);
        mPort = GUI.TextArea(new Rect(160, 20, 60, 20), mPort);
        if (GUI.Button(new Rect(230, 20, 70, 20), "Connect"))
        {
            int port = 0;
            if (int.TryParse(mPort, out port))
            {
                client.Connect(mAddress, port, 10f);
            }
        }
        GUI.TextField(new Rect(20, 45, 280, 180), mMessage);
    }

    void Update()
    {
        client.Tick(Time.deltaTime);
    }

    void callback_Message(LOG_TYPE tp, string msg)
    {
        Debug.Log(msg);
        if (mMessageList.Count > 10)
        {
            string temp = mMessageList[0];
            mBuilder.Remove(0, temp.Length + 2);
            mMessageList.RemoveAt(0);
        }
        mMessageList.Add(msg);
        mBuilder.AppendLine(msg);
        mMessage = mBuilder.ToString();
    }
}
