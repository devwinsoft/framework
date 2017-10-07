using UnityEngine;
using System.Collections;
using Devarc;

public class Stub_S2C : S2C.IStub
{
    HostID mHostID = HostID.None;

    public void OnNotifyConnecting()
    {

    }

    public void OnNotifyConnected(HostID host_hid)
    {
        mHostID = host_hid;
    }

    public void OnNotifyDisConnected(DISCONNECTION_REASON reason)
    {
    }

    public void RMI_S2C_Notify_Player(HostID remote, HostID _id, DataPlayer _data)
    {
        if (mHostID == _id)
        {
            SceneTest.Instance.CreateMainPlayer(_id, _data);
        }
        else
        {
            SceneTest.Instance.CreateUserPlayer(_id, _data);
        }
    }

    public void RMI_S2C_Notify_Move(HostID remote, VECTOR3 _look, DIRECTION _move)
    {
    }

    public void RMI_S2C_Notify_Chat(HostID remote, int channel, string _msg)
    {
        Log.Info(_msg);
    }
    public void RMI_S2C_Notify_Chat(HostID remote, string _msg)
    {
        Log.Info(_msg);
    }

    public bool OnReceive(object sender, NetBuffer msg)
    {
        switch (S2C.Stub.OnReceive(this, msg))
        {
            case RECEIVE_RESULT.INVALID_PACKET:
            case RECEIVE_RESULT.NOT_IMPLEMENTED:
                break;
            default:
                break;
        }
        return true;
    }
}
