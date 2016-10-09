using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devarc;

public class Test2C
{
    public const int RMI_VERSION = 1;
    public const int RMI_START = 5000;

    public static void Notify_Chat(string _name, string _msg) { }
    public static void Notify_UnitData(DataCharacter _data) { }
    public static void Notify_SendFile_Result(bool _success) { }
}
