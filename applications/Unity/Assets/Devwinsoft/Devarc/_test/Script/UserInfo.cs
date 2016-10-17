using UnityEngine;
using System.Collections;
using Devarc;

public class UserInfo : IContents<HostID>
{
    public static Container_S1<UserInfo, HostID> LIST = new Container_S1<UserInfo, HostID>(100);

    HostID hid;

    public void OnAlloc(HostID k1)
    {
        this.hid = k1;
    }

    public void OnFree()
    {

    }

    public HostID GetKey1()
    {
        return this.hid;
    }
}
