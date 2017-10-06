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
using System.Text;
using System.IO;

namespace Devarc
{
    public class NetBuffer
    {
        const int headeSize = 6;

        public ArraySegment<byte> Body
        {
            get
            {
                if (m_body == null || m_dirty)
                {
                    UpdateData();
                }
                return m_body;
            }
        }
        public ArraySegment<byte> Data
        {
            get
            {
                if (m_data == null || m_dirty)
                {
                    UpdateData();
                }
                return m_data;
            }
        }
        public short Rmi { get { return m_rmi; } }
        public HostID Hid { get { return m_hid; } }
        public int Length { get { return m_len; } }
        public int Pos { get { return m_pos; } }

        UnicodeEncoding m_encoding = new UnicodeEncoding();
        ArraySegment<byte> m_body;
        ArraySegment<byte> m_data;
        byte[] m_buf = new byte[1024];
        int m_len = headeSize;
        int m_pos = headeSize;
        short m_rmi = 0;
        HostID m_hid = HostID.None;
        bool m_dirty = false;

        public NetBuffer()
        {
            this.m_rmi = 0;
            this.m_len = headeSize;
            this.m_pos = headeSize;
        }

        public void Init(short rmi, HostID hid, ArraySegment<byte> body)
        {
            this.m_rmi = rmi;
            this.m_hid = hid;
            this.m_len = headeSize + body.Count;
            this.m_pos = headeSize;
            Buffer.BlockCopy(body.Array, body.Offset, m_buf, headeSize, body.Count);
            UpdateData();
        }

        public void Init(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            this.m_rmi = BitConverter.ToInt16(header.Array, header.Offset);
            this.m_hid = (HostID)BitConverter.ToInt16(header.Array, header.Offset + 2);
            this.m_len = headeSize + length;
            this.m_pos = headeSize;
            Buffer.BlockCopy(bodyBuffer, offset, m_buf, headeSize, length);
            UpdateData();
        }

        public void Init(short _rmi, HostID _hid)
        {
            this.m_rmi = _rmi;
            this.m_hid = _hid;
            this.m_len = headeSize;
            this.m_pos = headeSize;

            Buffer.BlockCopy(BitConverter.GetBytes(_rmi), 0, m_buf, 0, sizeof(short));
            UpdateData();
        }

        public void Init(byte[] readBuffer, int offset, int length)
        {
            this.m_rmi = BitConverter.ToInt16(readBuffer, offset);
            this.m_hid = (HostID)BitConverter.ToInt16(readBuffer, offset + 2);
            this.m_len = length;
            this.m_pos = headeSize;

            Buffer.BlockCopy(readBuffer, offset, m_buf, 0, length);
            UpdateData();
        }

        public void UpdateData()
        {
            // set body length
            Buffer.BlockCopy(BitConverter.GetBytes(m_rmi), 0, m_buf, 0, sizeof(short));
            Buffer.BlockCopy(BitConverter.GetBytes(m_hid), 0, m_buf, 2, sizeof(short));
            Buffer.BlockCopy(BitConverter.GetBytes(m_len - headeSize), 0, m_buf, 4, sizeof(short));

            m_body = new ArraySegment<byte>(m_buf, headeSize, m_len - headeSize);
            m_data = new ArraySegment<byte>(m_buf, 0, m_len);
            m_dirty = false;
        }

        public void Clear()
        {
            Array.Clear(m_buf, 0, this.Length);
            this.m_rmi = 0;
            this.m_len = headeSize;
            this.m_pos = headeSize;
            m_dirty = true;
        }

        public void Rewind(int _pos)
        {
            this.m_pos = _pos;
        }

        public byte ReadByte()
        {
            return m_buf[m_pos++];
        }

        public int ReadBytes(out byte[] _data)
        {
            Int16 readCnt = ReadInt16();
            readCnt = Math.Min((short)(m_buf.Length - m_pos), readCnt);
            readCnt = Math.Max(readCnt, (short)0);

            _data = new byte[readCnt];
            Buffer.BlockCopy(m_buf, m_pos, _data, 0, readCnt);
            m_pos += readCnt;
            return readCnt;
        }

        public float ReadFloat()
        {
            float value = BitConverter.ToSingle(m_buf, m_pos);
            m_pos += sizeof(float);
            return value;
        }

        public Int16 ReadInt16()
        {
            Int16 value = BitConverter.ToInt16(m_buf, m_pos);
            m_pos += sizeof(Int16);
            return value;
        }

        public Int32 ReadInt32()
        {
            Int32 value = BitConverter.ToInt32(m_buf, m_pos);
            m_pos += sizeof(Int32);
            return value;
        }

        public UInt32 ReadUInt32()
        {
            UInt32 value = BitConverter.ToUInt32(m_buf, m_pos);
            m_pos += sizeof(UInt32);
            return value;
        }

        public Int64 ReadInt64()
        {
            Int64 value = BitConverter.ToInt64(m_buf, m_pos);
            m_pos += sizeof(Int64);
            return value;
        }

        public string ReadString()
        {
            Int16 tmpLength = ReadInt16();
            tmpLength = Math.Min((short)(m_buf.Length - Pos), tmpLength);
            tmpLength = Math.Max(tmpLength, (short)0);

            string value = m_encoding.GetString(m_buf, m_pos, tmpLength);
            m_pos += tmpLength;
            return value;
        }

        public void Write(byte val)
        {
            m_buf[m_len] = val;
            m_len++;
            m_dirty = true;
        }

        public void Write(float val)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(val), 0, m_buf, m_len, sizeof(float));
            m_len += sizeof(float);
            m_dirty = true;
        }

        public void Write(bool val)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(val), 0, m_buf, m_len, sizeof(bool));
            m_len += sizeof(bool);
            m_dirty = true;
        }
        public void Write(Int16 val)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(val), 0, m_buf, m_len, sizeof(Int16));
            m_len += sizeof(Int16);
            m_dirty = true;
        }

        public void Write(Int32 val)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(val), 0, m_buf, m_len, sizeof(Int32));
            m_len += sizeof(Int32);
            m_dirty = true;
        }

        public void Write(UInt32 val)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(val), 0, m_buf, m_len, sizeof(UInt32));
            m_len += sizeof(UInt32);
            m_dirty = true;
        }

        public void Write(Int64 val)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(val), 0, m_buf, m_len, sizeof(Int64));
            m_len += sizeof(Int64);
            m_dirty = true;
        }

        public void Write(HostID val)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(val), 0, m_buf, m_len, sizeof(Int16));
            m_len += sizeof(Int16);
            m_dirty = true;
        }

        public void Write(string val)
        {
            byte[] temp = m_encoding.GetBytes(val);
            Write((Int16)temp.Length);

            Buffer.BlockCopy(temp, 0, m_buf, m_len, temp.Length);
            m_len += temp.Length;
            m_dirty = true;
        }

        public void Write(byte[] _data)
        {
            Write((Int16)_data.Length);

            Buffer.BlockCopy(_data, 0, m_buf, m_len, _data.Length);
            m_len += _data.Length;
            m_dirty = true;
        }
    }
}
