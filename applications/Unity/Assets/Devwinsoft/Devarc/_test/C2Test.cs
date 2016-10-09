using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devarc;

public class C2Test
{
    public const int RMI_VERSION = 1;
    public const int RMI_START = 6000;

    public static void Chat(string _name, string _msg) { }
    public static void Request_UnitData(UNIT _type) { }
    public static void SendFile(string _file_name, byte[] _data) { }
}

