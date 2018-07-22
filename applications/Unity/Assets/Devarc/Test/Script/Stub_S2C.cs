using UnityEngine;
using System.Collections;
using Devarc;

public class Stub_S2C : StubBase, S2C.IStub
{
    public void RMI_S2C_Notify_Player(HostID remote, Notify_Player msg)
    {

    }

    public void RMI_S2C_Notify_Move(HostID remote, Notify_Move msg)
    {
    }

    public void RMI_S2C_Notify_Chat(HostID remote, Notify_Chat msg)
    {
        Log.Info(msg._msg);
    }

    public override bool OnReceive(object sender, NetBuffer msg)
    {
        switch (S2C.Stub.OnReceive(this, msg))
        {
            case RECEIVE_RESULT.SUCCESS:
                return true;
            default:
                break;
        }
        return false;
    }
}
