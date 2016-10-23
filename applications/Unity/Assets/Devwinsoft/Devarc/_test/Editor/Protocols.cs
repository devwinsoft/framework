using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devarc;

namespace Protocols
{
    public class C2S
    {
        public const int RMI_VERSION = 1;
        public const int RMI_START = 6000;

        public static void Move(VECTOR3 _look, DIRECTION _move) { }
        public static void Chat(string _msg) { }
    }

    public class S2C
    {
        public const int RMI_VERSION = 1;
        public const int RMI_START = 5000;

        public static void Notify_Player(HostID _id, DataPlayer _data) { }
        public static void Notify_Move(VECTOR3 _look, DIRECTION _move) { }
        public static void Notify_Chat(string _msg) { }
    }

}

