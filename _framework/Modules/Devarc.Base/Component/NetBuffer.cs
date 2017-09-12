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
using System.Text;
using System.IO;

namespace Devarc
{
    public class NetBuffer
    {
        MemoryStream stream;
        UnicodeEncoding encoding = new UnicodeEncoding();
        byte[] buf = new byte[1024];
        public int Length { get { return Convert.ToInt32(stream.Length); } }
        public int Pos { get { return (int)stream.Position; } }

        public NetBuffer()
        {
            stream = new MemoryStream();
        }

        public NetBuffer(byte[] _data)
        {
            stream = new MemoryStream(_data);
        }

        public void Clear()
        {
            stream.Close();
        }

        public void Rewind(int pos)
        {
            stream.Seek(pos, SeekOrigin.Begin);
        }

        public byte[] ToArray()
        {
            return stream.ToArray();
        }

        public byte ReadByte()
        {
            return Convert.ToByte(stream.ReadByte());
        }

        public byte[] ReadBytes(int cnt)
        {
            byte[] data = new byte[cnt];
            stream.Read(data, 0, cnt);
            return data;
        }

        public float ReadFloat()
        {
            int len = stream.Read(buf, 0, sizeof(float));
            return BitConverter.ToSingle(buf, len - sizeof(float));
        }

        public Int16 ReadInt16()
        {
            int len = stream.Read(buf, 0, sizeof(Int16));
            return BitConverter.ToInt16(buf, len - sizeof(Int16));
        }

        public Int32 ReadInt32()
        {
            int len = stream.Read(buf, 0, sizeof(Int32));
            return BitConverter.ToInt32(buf, len - sizeof(Int32));
        }

        public Int64 ReadInt64()
        {
            int len = stream.Read(buf, 0, sizeof(Int64));
            return BitConverter.ToInt64(buf, len - sizeof(Int64));
        }

        public string ReadString()
        {
            int data_len = ReadInt16();
            int read_len = stream.Read(buf, 0, data_len);
            return encoding.GetString(buf, 0, read_len);
        }

        public void Write(byte val)
        {
            stream.WriteByte(val);
        }

        public void Write(float val)
        {
            stream.Write(BitConverter.GetBytes(val), 0, sizeof(float));
        }

        public void Write(bool val)
        {
            stream.Write(BitConverter.GetBytes(val), 0, sizeof(bool));
        }
        public void Write(Int16 val)
        {
            stream.Write(BitConverter.GetBytes(val), 0, sizeof(Int16));
        }

        public void Write(Int32 val)
        {
            stream.Write(BitConverter.GetBytes(val), 0, sizeof(Int32));
        }

        public void Write(Int64 val)
        {
            stream.Write(BitConverter.GetBytes(val), 0, sizeof(Int64));
        }

        public void Write(HostID val)
        {
            stream.Write(BitConverter.GetBytes(val), 0, sizeof(Int16));
        }

        public void Write(string val)
        {
            byte[] temp = encoding.GetBytes(val);
            Write((Int16)temp.Length);
            stream.Write(temp, 0, temp.Length);
        }

        public void Write(byte[] data, int offset, int count)
        {
            stream.Write(data, offset, count);
        }
    }
}
