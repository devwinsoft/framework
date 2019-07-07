//
// Copyright (c) 2012 Hyoung Joon, Kim
// http://www.devwinsoft.com/
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

//
// @author Hyoung Joon, Kim (maoshy@nate.com)
// @version $Rev: 1, $Date: 2012-02-20
//

using System;
using System.Collections.Generic;

namespace Devarc
{
    public static partial class Marshaler
    {
        /*
         * Tcp
         * 
         */
        public static bool Write(NetBuffer msg, bool obj)
        {
            msg.Write((byte)(obj ? 1 : 0));
            return !msg.IsError;
        }
        public static bool Write(NetBuffer msg, byte obj)
        {
            msg.Write(obj);
            return !msg.IsError;
        }
        public static bool Write(NetBuffer msg, Int16 obj)
        {
            msg.Write(obj);
            return !msg.IsError;
        }
        public static bool Write(NetBuffer msg, Int32 obj)
        {
            msg.Write(obj);
            return !msg.IsError;
        }
        public static bool Write(NetBuffer msg, Int64 obj)
        {
            msg.Write(obj);
            return !msg.IsError;
        }
        public static bool Write(NetBuffer msg, HostID obj)
        {
            msg.Write(obj);
            return !msg.IsError;
        }
        public static bool Write(NetBuffer msg, float obj)
        {
            msg.Write(obj);
            return !msg.IsError;
        }
        public static bool Write(NetBuffer msg, string obj)
        {
            msg.Write(obj);
            return !msg.IsError;
        }

        public static bool Write(NetBuffer msg, bool[] list)
        {
            msg.Write((Int16)list.Length);
            foreach (bool obj in list)
            {
                msg.Write((byte)(obj ? 1 : 0));
            }
            return !msg.IsError;
        }
        public static bool Write(NetBuffer msg, byte[] list)
        {
            msg.Write((Int16)list.Length);
            foreach (byte obj in list)
            {
                msg.Write(obj);
            }
            return !msg.IsError;
        }
        public static bool Write(NetBuffer msg, Int16[] list)
        {
            msg.Write((Int16)list.Length);
            foreach (Int16 obj in list)
            {
                msg.Write(obj);
            }
            return !msg.IsError;
        }
        public static bool Write(NetBuffer msg, Int32[] list)
        {
            msg.Write((Int16)list.Length);
            foreach (Int32 obj in list)
            {
                msg.Write(obj);
            }
            return !msg.IsError;
        }
        public static bool Write(NetBuffer msg, Int64[] list)
        {
            msg.Write((Int16)list.Length);
            foreach (Int64 obj in list)
            {
                msg.Write(obj);
            }
            return !msg.IsError;
        }
        public static bool Write(NetBuffer msg, HostID[] list)
        {
            msg.Write((Int16)list.Length);
            foreach (HostID obj in list)
            {
                msg.Write(obj);
            }
            return !msg.IsError;
        }
        public static bool Write(NetBuffer msg, float[] list)
        {
            msg.Write((Int16)list.Length);
            foreach (float obj in list)
            {
                msg.Write(obj);
            }
            return !msg.IsError;
        }
        public static bool Write(NetBuffer msg, string[] list)
        {
            msg.Write((Int16)list.Length);
            foreach (string obj in list)
            {
                msg.Write(obj);
            }
            return !msg.IsError;
        }

        public static bool Write(NetBuffer msg, List<int> list)
        {
            msg.Write((Int16)list.Count);
            foreach (int obj in list)
            {
                msg.Write(obj);
            }
            return !msg.IsError;
        }

        public static bool Write(NetBuffer msg, List<float> list)
        {
            msg.Write((float)list.Count);
            foreach (float obj in list)
            {
                msg.Write(obj);
            }
            return !msg.IsError;
        }

        public static bool Write(NetBuffer msg, List<string> list)
        {
            msg.Write((Int16)list.Count);
            foreach (string obj in list)
            {
                msg.Write(obj);
            }
            return !msg.IsError;
        }
    }
}
