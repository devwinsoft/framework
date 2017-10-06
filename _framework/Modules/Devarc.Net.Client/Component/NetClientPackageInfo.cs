using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.ProtoBase;
using SuperSocket.ClientEngine;

namespace Devarc
{
    public class NetClientPackageInfo : IPackageInfo<short>
    {
        public NetClientPackageInfo(short rmi, HostID hid, ArraySegment<byte> body)
        {
            Msg = new NetBuffer();
            Msg.Init(rmi, hid, body);
        }

        public NetBuffer Msg = null;

        public short Rmi { get { return Msg.Rmi; } }

        public ArraySegment<byte> Body { get { return Msg.Body; } }
    }

}

