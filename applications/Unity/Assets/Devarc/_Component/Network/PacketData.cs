using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.ProtoBase;
using SuperSocket.ClientEngine;

namespace Devarc
{
    public struct PacketData
    {
        public short Rmi { get { return mRmi; } }
        short mRmi;

        public ArraySegment<byte> Data { get { return mData; } }
        private ArraySegment<byte> mData;
        private byte[] mBuffer;

        public ArraySegment<byte> Body { get { return mBody; } }
        private ArraySegment<byte> mBody;

        public void Init(short _rmi, string _body)
        {
            mBuffer = new byte[1024];

            byte[] tmpData = Encoding.Unicode.GetBytes(_body);
            Buffer.BlockCopy(tmpData, 0, mBuffer, 6, tmpData.Length);

            mData = new ArraySegment<byte>(mBuffer, 0, tmpData.Length + 6);
            mBody = new ArraySegment<byte>(mBuffer, 6, tmpData.Length);

            mRmi = _rmi;
            short bodySize = (short)tmpData.Length;
            Buffer.BlockCopy(BitConverter.GetBytes(_rmi), 0, mBuffer, 0, sizeof(short));
            Buffer.BlockCopy(BitConverter.GetBytes(bodySize), 0, mBuffer, 4, sizeof(short));
        }
    }
}




