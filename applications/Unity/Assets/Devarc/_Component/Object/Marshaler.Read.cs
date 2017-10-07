﻿//
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
        public static bool Read(NetBuffer msg, ref bool obj)
        {
            obj = msg.ReadByte() != 0 ? true : false;
            return !msg.IsError;
        }
        public static bool Read(NetBuffer msg, ref byte obj)
        {
            obj = msg.ReadByte();
            return !msg.IsError;
        }
        public static bool Read(NetBuffer msg, ref Int16 obj)
        {
            obj = msg.ReadInt16();
            return !msg.IsError;
        }
        public static bool Read(NetBuffer msg, ref Int32 obj)
        {
            obj = msg.ReadInt32();
            return !msg.IsError;
        }
        public static bool Read(NetBuffer msg, ref Int64 obj)
        {
            obj = msg.ReadInt64();
            return !msg.IsError;
        }
        public static bool Read(NetBuffer msg, ref UInt32 obj)
        {
            obj = msg.ReadUInt32();
            return !msg.IsError;
        }
        public static bool Read(NetBuffer msg, ref HostID obj)
        {
            obj = msg.ReadInt16();
            return !msg.IsError;
        }
        public static bool Read(NetBuffer msg, ref float obj)
        {
            obj = msg.ReadFloat();
            return !msg.IsError;
        }
        public static bool Read(NetBuffer msg, ref string obj)
        {
            obj = msg.ReadString();
            return !msg.IsError;
        }

        public static bool Read(NetBuffer msg, out bool[] obj)
        {
            int cnt = msg.ReadInt16();
            obj = new bool[cnt];
            for (int i = 0; i < cnt; i++)
            {
                obj[i] = msg.ReadByte() != 0 ? true : false;
            }
            return !msg.IsError;
        }
        public static bool Read(NetBuffer msg, out byte[] obj)
        {
            msg.ReadBytes(out obj);
            return !msg.IsError;
        }
        public static bool Read(NetBuffer msg, out Int16[] obj)
        {
            int cnt = msg.ReadInt16();
            obj = new Int16[cnt];
            for (int i = 0; i < cnt; i++)
            {
                obj[i] = msg.ReadInt16();
            }
            return !msg.IsError;
        }
        public static bool Read(NetBuffer msg, out Int32[] obj)
        {
            int cnt = msg.ReadInt16();
            obj = new Int32[cnt];
            for (int i = 0; i < cnt; i++)
            {
                obj[i] = msg.ReadInt32();
            }
            return !msg.IsError;
        }
        public static bool Read(NetBuffer msg, out Int64[] obj)
        {
            int cnt = msg.ReadInt16();
            obj = new Int64[cnt];
            for (int i = 0; i < cnt; i++)
            {
                obj[i] = msg.ReadInt64();
            }
            return !msg.IsError;
        }
        public static bool Read(NetBuffer msg, out HostID[] obj)
        {
            int cnt = msg.ReadInt16();
            obj = new HostID[cnt];
            for (int i = 0; i < cnt; i++)
            {
                Read(msg, ref obj[i]);
            }
            return !msg.IsError;
        }
        public static bool Read(NetBuffer msg, out float[] obj)
        {
            int cnt = msg.ReadInt16();
            obj = new float[cnt];
            for (int i = 0; i < cnt; i++)
            {
                obj[i] = msg.ReadFloat();
            }
            return !msg.IsError;
        }
        public static bool Read(NetBuffer msg, out string[] obj)
        {
            int cnt = msg.ReadInt16();
            obj = new string[cnt];
            for (int i = 0; i < cnt; i++)
            {
                obj[i] = msg.ReadString();
            }
            return !msg.IsError;
        }

        public static bool Read(NetBuffer msg, List<int> list)
        {
            int cnt = msg.ReadInt16();
            for (int i = 0; i < cnt; i++)
            {
                list.Add(msg.ReadInt32());
            }
            return !msg.IsError;
        }

        public static bool Read(NetBuffer msg, List<float> list)
        {
            int cnt = msg.ReadInt16();
            for (int i = 0; i < cnt; i++)
            {
                list.Add(msg.ReadFloat());
            }
            return !msg.IsError;
        }

        public static bool Read(NetBuffer msg, List<string> list)
        {
            int cnt = msg.ReadInt16();
            for (int i = 0; i < cnt; i++)
            {
                list.Add(msg.ReadString());
            }
            return !msg.IsError;
        }
    }
}
