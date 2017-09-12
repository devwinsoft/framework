using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Devarc;

public class Stub_C2S : IServerStub, C2S.IStub
{
    public void OnNotifyUserConnect(HostID host_hid)
    {
        using (PlayerData.MAP.WRITE_LOCK())
        {
            PlayerData.MAP.Alloc(host_hid);
        } // unlock
    }

    public void OnNotifyUserDisonnect(HostID host_hid)
    {
        using (PlayerData.MAP.WRITE_LOCK())
        {
            PlayerData.MAP.Free1(host_hid);
        } // unlock
    }

    public void RMI_C2S_Move(HostID remote, VECTOR3 _look, DIRECTION _move)
    {
        using (PlayerData.MAP.READ_LOCK())
        {
            List<PlayerData>.Enumerator enumerator = PlayerData.MAP.GetEnumerator();
            while(enumerator.MoveNext())
            {
                PlayerData obj = enumerator.Current;
                SceneTest.Instance.proxyS2C.Notify_Move(obj.GetKey1(), _look, _move);
            }
        } // unlock

    }

    public void RMI_C2S_Chat(HostID remote, string _msg)
    {

    }

    public bool OnReceive(int rid, HostID hid, NetBuffer msg)
    {
        return C2S.Stub.OnReceive(this, rid, hid, msg);
    }
}
