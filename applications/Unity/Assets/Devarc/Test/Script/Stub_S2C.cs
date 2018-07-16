using UnityEngine;
using System.Collections;
using Devarc;

public class Stub_S2C : IStubBase, S2C.IStub
{
    public void RMI_S2C_Notify_Player(HostID remote, S2C.MSG.Notify_Player msg)
    {

    }

    public void RMI_S2C_Notify_Move(HostID remote, S2C.MSG.Notify_Move msg)
    {
    }

    public void RMI_S2C_Notify_Chat(HostID remote, S2C.MSG.Notify_Chat msg)
    {
        Log.Info(msg._msg);
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
