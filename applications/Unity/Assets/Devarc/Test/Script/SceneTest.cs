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
    public static SceneTest Instance { get { return msInstance; } }
    static SceneTest msInstance = null;

    public TextAsset assetLString;

    public Transform prefabMainPlayer;
    public Transform prefabUserPlayer;

    //[HideInInspector]
    //public PLAY_STATE State = PLAY_STATE.NONE;

    public C2S.Proxy proxy = new C2S.Proxy();
    public NetClient client = new NetClient();
    Stub_S2C stub = new Stub_S2C();


    string mAddress = "127.0.0.1";
    string mPort = "5000";
    string mMessage = "";
    List<string> mMessageList = new List<string>();
    StringBuilder mBuilder = new StringBuilder();

    void Start()
    {
        Log.OnMessage += callback_Message;

        //TextAsset txtAsset;
        //txtAsset = Resources.Load<TextAsset>("TableData/Localize/LString_"); TableManager.Load_LString_SheetData(txtAsset.text);

        Table.Open(Application.streamingAssetsPath + "/database.sqlite");
        DataCharacter data;

        data = Table.T_DataCharacter.GetAt(Table.Session, UNIT.HUMAN_FEMALE);
        data = Table.T_DataCharacter.GetAt(UNIT.DRAGON_BLACK);

        Table.Session.Close();

        proxy.Init(client);
        client.InitStub(stub);
    }

    void OnGUI()
    {
        if (client.Client.IsConnected == false)
        {
            mAddress = GUI.TextArea(new Rect(20, 20, 130, 20), mAddress);
            mPort = GUI.TextArea(new Rect(160, 20, 60, 20), mPort);
            if (GUI.Button(new Rect(230, 20, 70, 20), "Connect"))
            {
                int port = 0;
                if (int.TryParse(mPort, out port))
                {
                    client.Connect(mAddress, port);
                }
            }
        }
        else
        {
            GUI.Label(new Rect(20, 20, 130, 20), mAddress);
            GUI.Label(new Rect(160, 20, 60, 20), mPort);
            if (GUI.Button(new Rect(Screen.width - 120, 20, 100, 20), "Test"))
            {
                proxy.Request_Chat(HostID.Server, "ABC TEST 1234");
            }
        }
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
