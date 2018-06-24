using UnityEngine;
using System.Collections;
using Devarc;

public class Stub_S2C : IStubBase, S2C.IStub
{
    public void RMI_S2C_Notify_Player(HostID remote, HostID _id, DataPlayer _data)
    {
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

    public bool OnReceiveData(object sender, NetBuffer msg)
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
