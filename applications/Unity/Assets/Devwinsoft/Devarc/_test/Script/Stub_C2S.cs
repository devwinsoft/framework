using UnityEngine;
using System.Collections;
using Devarc;

public class Stub_C2S : IServerStub, C2S.IStub
{
    public void OnNotifyUserConnect(HostID host_hid)
    {

    }

    public void OnNotifyUserDisonnect(HostID host_hid)
    {

    }

    public void RMI_C2S_Chat(HostID remote, string _msg)
    {

    }

    public void RMI_C2S_Move(HostID remote, VECTOR3 _look, DIRECTION _move)
    {

    }

    public bool OnReceive(int rid, HostID hid, NetBuffer msg)
    {
        return C2S.Stub.OnReceive(this, rid, hid, msg);
    }
}
