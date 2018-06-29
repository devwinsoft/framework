using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devarc
{
    public class NetBufferPool
    {
        public static NetBufferPool Instance
        {
            get
            {
                if (msInstance != null)
                    return msInstance;
                msInstance = new NetBufferPool();
                return msInstance;
            }
        }
        private static NetBufferPool msInstance = new NetBufferPool();

        Queue<NetBuffer> mPool = new Queue<NetBuffer>();

        public NetBuffer Pop()
        {
            lock (mPool)
            {
                if (mPool.Count == 0)
                {
                    return new NetBuffer();
                }
                return mPool.Dequeue();
            }
        }

        public void Push(NetBuffer obj)
        {
            lock (mPool)
            {
                mPool.Enqueue(obj);
            }
        }
    }
}
