using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Devarc;

public class Stub_C2S : C2S.IStub
{
    public void OnNotifyUserConnect(HostID host_hid)
    {
    }

    public void OnNotifyUserDisonnect(HostID host_hid)
    {
    }

    public void RMI_C2S_Move(HostID remote, VECTOR3 _look, DIRECTION _move)
    {
    }

    public void RMI_C2S_Chat(HostID remote, string _msg)
    {

    }

    public bool OnReceive(object sender, NetBuffer msg)
    {
        switch (C2S.Stub.OnReceive(this, msg))
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
