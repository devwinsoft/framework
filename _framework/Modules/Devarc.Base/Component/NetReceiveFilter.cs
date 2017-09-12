using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.Common;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.Facility.Protocol;

namespace Devarc
{
    public class NetReceiveFilter : FixedHeaderReceiveFilter<BinaryRequestInfo>
    {
        public NetReceiveFilter() : base(6)
		{
        }

        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            return (int)header[offset + 4] * 256 + (int)header[offset + 5];
        }

        protected override BinaryRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            return new BinaryRequestInfo(Encoding.UTF8.GetString(header.Array, header.Offset, 4), bodyBuffer.CloneRange(offset, length));
        }

        //public override BinaryRequestInfo Filter(byte[] readBuffer, int offset, int length, bool toBeCopied, out int rest)
        //{
        //    rest = 0;

        //    //데이터가 있는지?
        //    if (null == readBuffer)
        //    {
        //        return NullRequestInfo;
        //    }

        //    //데이터가 있다.

        //    //리턴할 객체
        //    BinaryRequestInfo briReturn;
        //    //자른 데이터

        //    byte[] buf = new byte[length];

        //    //데이터를 오프셋을 기준으로 자른다.
        //    Buffer.BlockCopy(readBuffer, offset, buf, 0, length);

        //    //데이터를 넣는다.
        //    briReturn = new BinaryRequestInfo("DATA", buf);
        //    return briReturn;
        //}
    }
}
