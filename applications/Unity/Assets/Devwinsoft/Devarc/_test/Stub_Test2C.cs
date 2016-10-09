using UnityEngine;
using System.Collections;
using Devarc;

public class Stub_Test2C : IClientStub, Test2C.IStub
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

    public void RMI_Test2C_Notify_Chat(HostID remote, string _name, string _msg)
    {
        Log.Message(LOG_TYPE.INFO, "[{0}]: {1}", _name, _msg);
    }

    public void RMI_Test2C_Notify_UnitData(HostID remote, DataCharacter _data)
    {
    }

    public void RMI_Test2C_Notify_SendFile_Result(HostID remote, bool _success)
    {
        if (_success)
            Log.Message(LOG_TYPE.INFO, "Success!");
        else
            Log.Message(LOG_TYPE.INFO, "Failed!");
    }


    public bool OnReceive(int rid, HostID hid, NetBuffer msg)
    {
        return Test2C.Stub.OnReceive(this, rid, hid, msg);
    }
}
