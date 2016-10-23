using UnityEngine;
using System.Collections;
using Devarc;

public class Stub_S2C : IClientStub, S2C.IStub
{
    public void OnNotifyConnecting()
    {

    }

    public void OnNotifyConnected(HostID host_hid)
    {
    }

    public void OnNotifyDisConnected(DISCONNECTION_REASON reason)
    {
    }

    public void RMI_S2C_Notify_Player(HostID remote, HostID _id, DataPlayer _data)
    {

    }

    public void RMI_S2C_Notify_Move(HostID remote, VECTOR3 _look, DIRECTION _move)
    {
    }

    public void RMI_S2C_Notify_Chat(HostID remote, string _msg)
    {
        Log.Info(_msg);
    }

    public bool OnReceive(int rid, HostID hid, NetBuffer msg)
    {
        return S2C.Stub.OnReceive(this, rid, hid, msg);
    }
}
