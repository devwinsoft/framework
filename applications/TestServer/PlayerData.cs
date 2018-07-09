using System;
using System.Collections;
using System.Collections.Generic;
using Devarc;

public class PlayerData : TLockObject<PlayerData, HostID>
{
    public static TLockContainer<PlayerData, HostID> LIST = new TLockContainer<PlayerData, HostID>(100);

    public DataPlayer Data;

    public PlayerData()
    {
        Data = new DataPlayer();
    }

    public override void OnAlloc(HostID hid)
    {
        Data.id = hid;
    }

    public override void OnFree()
    {

    }

    public override HostID GetKey1()
    {
        return Data.id;
    }
}
