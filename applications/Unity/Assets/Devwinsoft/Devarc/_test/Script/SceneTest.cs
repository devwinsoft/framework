using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Devarc;

public enum PLAY_STATE
{
    NONE,
    SERVER_READY,
    SERVER_RUN,
    CLIENT_READY,
    CLIENT_RUN,
}

public class SceneTest : MonoBehaviour
{
    public Transform prefabPlayer;
    public Transform prefabUser;

    [HideInInspector]
    public PLAY_STATE State = PLAY_STATE.NONE;

    public S2C.Proxy proxyS2C = new S2C.Proxy();
    public NetServer server = new NetServer();
    Stub_C2S stubC2S = new Stub_C2S();

    public C2S.Proxy proxyC2S = new C2S.Proxy();
    public NetClient client = new NetClient();
    Stub_S2C stubS2C = new Stub_S2C();

    string[] mModes = new string[] { "Run As Server", "Run As Client" };
    int mSelectedMode = -1;

    string mAddress = "127.0.0.1";
    string mPort = "5000";
    string mMessage = "";
    List<string> mMessageList = new List<string>();
    StringBuilder mBuilder = new StringBuilder();

    void Start()
    {
        Log.SetCallback(callback_Message);
        server.InitStub(this.stubC2S);
        client.InitStub(this.stubS2C);
    }

    void OnGUI()
    {
        switch(this.State)
        {
            case PLAY_STATE.NONE:
                {
                    mSelectedMode = GUI.SelectionGrid(new Rect(20, 20, 280, 20), mSelectedMode, mModes, mModes.Length);
                    switch(mSelectedMode)
                    {
                        case 0:
                            this.State = PLAY_STATE.SERVER_READY;
                            break;
                        case 1:
                            this.State = PLAY_STATE.CLIENT_READY;
                            break;
                        default:
                            break;
                    }
                }
                break;
            case PLAY_STATE.SERVER_READY:
                {
                    GUI.Label(new Rect(20, 20, 70, 20), "Server Port");
                    mPort = GUI.TextArea(new Rect(100, 20, 80, 20), mPort);
                    if (this.server.IsRunning == false)
                    {
                        if (GUI.Button(new Rect(230, 20, 70, 20), "Start"))
                        {
                            int portValue;
                            if (int.TryParse(mPort, out portValue))
                            {
                                this.server.Start(portValue, 2);
                            }
                        }
                    }
                    else
                    {
                        if (GUI.Button(new Rect(230, 20, 70, 20), "Terminate"))
                        {
                            this.server.Stop();
                        }
                    }
                }
                break;
            case PLAY_STATE.CLIENT_READY:
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
                }
                break;
            case PLAY_STATE.CLIENT_RUN:
            case PLAY_STATE.SERVER_RUN:
            default:
                break;
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
