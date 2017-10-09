using System;
using System.Collections;
using System.Collections.Generic;
using Devarc;

public class PlayerData : TContents<PlayerData, HostID>
{
    public static LockableContainer<PlayerData, HostID> MAP = new LockableContainer<PlayerData, HostID>(100);

    public DataPlayer data;

    public void OnAlloc(HostID k1)
    {
        data.id = k1;
    }

    public void OnFree()
    {

    }

    public HostID GetKey1()
    {
        return data.id;
    }
}
