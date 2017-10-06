using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.ProtoBase;
using SuperSocket.ClientEngine;

namespace Devarc
{
    public class NetClientReceiveFilter
        : SuperSocket.ProtoBase.FixedHeaderReceiveFilter<NetClientPackageInfo>
    {
        const int headerSize = 6;

        public NetClientReceiveFilter()
            : base(headerSize)
        {
        }

#if UNITY_5
        protected override int GetBodyLengthFromHeader(IBufferStream bufferStream, int length)
        {
            if (length <= 0)
                return 0;
            ArraySegment<byte> header = bufferStream.Buffers[0];
            int dataLength = BitConverter.ToInt16(header.Array, header.Offset + 4);
            return dataLength;
        }

        public override NetClientPackageInfo ResolvePackage(IBufferStream bufferStream)
        {
            ArraySegment<byte> header = bufferStream.Buffers[0];
            short rmi = BitConverter.ToInt16(header.Array, header.Offset);
            HostID hid = (HostID)BitConverter.ToInt16(header.Array, header.Offset + 2);
            NetClientPackageInfo packageInfo = new NetClientPackageInfo(rmi, hid, bufferStream.Buffers[1]);
            return packageInfo;
        }
#else
        protected override int GetBodyLengthFromHeader(IList<ArraySegment<byte>> packageData, int length)
        {
            if (length <= 0)
                return 0;
            ArraySegment<byte> header = packageData[0];
            int dataLength = BitConverter.ToInt16(header.Array, header.Offset + 4);
            return dataLength;
        }

        public override NetClientPackageInfo ResolvePackage(IList<ArraySegment<byte>> packageData)
        {
            ArraySegment<byte> header = packageData[0];
            ArraySegment<byte> body = packageData[1];
            short rmi = BitConverter.ToInt16(header.Array, header.Offset + 0);
            HostID hid = (HostID)BitConverter.ToInt16(header.Array, header.Offset + 2); ;
            NetClientPackageInfo packageInfo = new NetClientPackageInfo(rmi, hid, body);
            return packageInfo;
        }
#endif
    }
}

