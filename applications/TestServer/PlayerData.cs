using System;
using System.Collections;
using System.Collections.Generic;
using Devarc;

public class PlayerData : IContents<HostID>
{
    public static Container_S1<PlayerData, HostID> LIST = new Container_S1<PlayerData, HostID>(100);

    public DataPlayer Data;

    public void OnAlloc(HostID k1)
    {
        Data.id = k1;
    }

    public void OnFree()
    {

    }

    public HostID GetKey1()
    {
        return Data.id;
    }
}
