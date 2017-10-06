using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.Common;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.Facility.Protocol;

namespace Devarc
{
    public class NetServerReceiveFilter
        : SuperSocket.Facility.Protocol.FixedHeaderReceiveFilter<NetRequestInfo>
    {
        const int headerSize = 6;

        public NetServerReceiveFilter() : base(headerSize)
        {
        }

        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            return (int)header[offset + 4] * 256 + (int)header[offset + 5];
        }

        protected override NetRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            NetBuffer msg = new NetBuffer();
            msg.Init(header, bodyBuffer, offset, length);
            return new NetRequestInfo("DATA", msg);
        }

        public override NetRequestInfo Filter(byte[] readBuffer, int offset, int length, bool toBeCopied, out int rest)
        {
            rest = 0;
            if (readBuffer == null)
            {
                return NullRequestInfo;
            }

            if (length < headerSize)
            {
                Log.Error("Invalid packet.");
                return NullRequestInfo;
            }

            NetBuffer msg = new NetBuffer();
            msg.Init(readBuffer, offset, length);
            NetRequestInfo retObject = new NetRequestInfo("DATA", msg);
            return retObject;
        }
    }


    public class NetRequestInfo : RequestInfo<byte[]>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryRequestInfo"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="rmi">The remote message id.</param>
        /// <param name="body">The body.</param>
        public NetRequestInfo(string key, NetBuffer msg)
            : base(key, msg.Body.Array)
        {
            Msg = msg;
        }

        public NetBuffer Msg = null;
    }
}


