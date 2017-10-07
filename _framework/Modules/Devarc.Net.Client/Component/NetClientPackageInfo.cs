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
        public NetClientPackageInfo(short rmi, HostID hid, short seq, ArraySegment<byte> body)
        {
            Msg = new NetBuffer();
            Msg.Init(rmi, hid, seq, body);
        }

        public NetBuffer Msg = null;

        public short Key { get { return Msg.Rmi; } }

        public short Rmi { get { return Msg.Rmi; } }

        public HostID Hid { get { return Msg.Hid; } }

        public short Seq { get { return Msg.Seq; } }

        public ArraySegment<byte> Body { get { return Msg.Body; } }
    }

}

