using System;
using System.Collections;
using System.Collections.Generic;
using Devarc;

public class PlayerData : TLockObject<PlayerData, int>
{
    public static TLockContainer<PlayerData, int> LIST = new TLockContainer<PlayerData, int>(100);

    public DataPlayer Data;

    public PlayerData()
    {
        Data = new DataPlayer();
    }

    public override void OnAlloc(int hid)
    {
        Data.id = hid;
    }

    public override void OnFree()
    {

    }

    public override int GetKey1()
    {
        return Data.id;
    }
}
