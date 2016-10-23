using UnityEngine;
using System.Collections;
using Devarc;

public class PlayerData : IContents<HostID>
{
    public static Container_S1<PlayerData, HostID> LIST = new Container_S1<PlayerData, HostID>(100);

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
