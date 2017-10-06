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

        protected override int GetBodyLengthFromHeader(IList<ArraySegment<byte>> packageData, int length)
        {
            if (length <= 0)
                return 0;
            byte[] header = packageData[0].Array;
            return (int)header[5] * 256 + (int)header[4];
        }

        public override NetClientPackageInfo ResolvePackage(IList<ArraySegment<byte>> packageData)
        {
            short rmi = (short)(packageData[0].ElementAt(1) * 256 + packageData[0].ElementAt(0));
            HostID hid = (HostID)(packageData[0].ElementAt(3) * 256 + packageData[0].ElementAt(2));
            NetClientPackageInfo packageInfo = new NetClientPackageInfo(rmi, hid, packageData[1]);
            return packageInfo;
        }
    }
}

