using System;
using System.Collections;
using System.Collections.Generic;
using Devarc;

public class PlayerData : TContents<PlayerData, HostID>
{
    public static LockableContainer<PlayerData, HostID> LIST = new LockableContainer<PlayerData, HostID>(100);

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
