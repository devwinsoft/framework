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
using System.Globalization;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

namespace Devarc
{
    public struct HostID : IComparable, IFormattable, IConvertible, IComparable<short>, IEquatable<short>
    {
        private Int16 mValue { get; set; }
        public const short MaxValue = 32767;
        public const short MinValue = -32768;
        public const short None = 0;
        public const short Server = 1;
        public const short TcpHostStart = 10001;
        public const short TcpHostEnd = 30000;

        public static implicit operator short(HostID value)
        {
            return value.mValue;
        }
        public static implicit operator HostID(short value)
        {
            HostID temp = new HostID();
            temp.mValue = value;
            return temp;
        }
        public static bool operator ==(HostID value1, HostID value2)
        {
            return value1.mValue == value2.mValue;
        }
        public static bool operator !=(HostID value1, HostID value2)
        {
            return value1.mValue != value2.mValue;
        }
        public static HostID operator ++(HostID value)
        {
            value.mValue++;
            return value;
        }

        public int CompareTo(short value) { return mValue.CompareTo(value); }
        public int CompareTo(object value) { return mValue.CompareTo(value); }
        public bool Equals(short obj) { return mValue.Equals(obj); }
        public bool Equals(HostID obj) { return mValue == obj.mValue; }
        public override bool Equals(object obj)
        {
            if (typeof(HostID) != obj.GetType())
                return false;
            HostID val = (HostID)obj;
            return mValue.Equals(val);
        }
        public override int GetHashCode()
        {
            return mValue.GetHashCode();
        }
        public TypeCode GetTypeCode() { return mValue.GetTypeCode(); }
        public static short Parse(string s) { return Int16.Parse(s); }
        public static short Parse(string s, IFormatProvider provider) { return Int16.Parse(s, provider); }
        public static short Parse(string s, NumberStyles style) { return Int16.Parse(s, style); }
        public static short Parse(string s, NumberStyles style, IFormatProvider provider) { return Int16.Parse(s, style, provider); }
        public override string ToString() { return mValue.ToString(); }
        public string ToString(IFormatProvider provider) { return mValue.ToString(); }
        public string ToString(string format) { return mValue.ToString(); }
        public string ToString(string format, IFormatProvider provider) { return mValue.ToString(); }
        public static bool TryParse(string s, out short result) { return Int16.TryParse(s, out result); }
        public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out short result) { return Int16.TryParse(s, style, provider, out result); }

        public bool ToBoolean(IFormatProvider provider) { return Convert.ToBoolean(mValue); }
        public byte ToByte(IFormatProvider provider) { return Convert.ToByte(mValue); }
        public char ToChar(IFormatProvider provider) { return Convert.ToChar(mValue); }
        public DateTime ToDateTime(IFormatProvider provider) { return Convert.ToDateTime(mValue); }
        public decimal ToDecimal(IFormatProvider provider) { return Convert.ToDecimal(mValue); }
        public double ToDouble(IFormatProvider provider) { return Convert.ToDouble(mValue); }
        public short ToInt16(IFormatProvider provider) { return Convert.ToInt16(mValue); }
        public int ToInt32(IFormatProvider provider) { return Convert.ToInt32(mValue); }
        public long ToInt64(IFormatProvider provider) { return Convert.ToInt64(mValue); }
        public sbyte ToSByte(IFormatProvider provider) { return Convert.ToSByte(mValue); }
        public float ToSingle(IFormatProvider provider) { return Convert.ToSingle(mValue); }
        public object ToType(Type conversionType, IFormatProvider provider) { return typeof(HostID); }
        public ushort ToUInt16(IFormatProvider provider) { return Convert.ToUInt16(mValue); }
        public uint ToUInt32(IFormatProvider provider) { return Convert.ToUInt32(mValue); }
        public ulong ToUInt64(IFormatProvider provider) { return Convert.ToUInt64(mValue); }
    }

    //public struct HostID : IComparable, IFormattable, IConvertible, IComparable<long>, IEquatable<long>
    //{
    //    private Int64 mValue { get; set; }
    //    public const long MaxValue = 9223372036854775807;
    //    public const long MinValue = -9223372036854775808;
    //    public const long None = 0;
    //    public const long Server = 1;
    //    public const long UdpHostStart = 10001;
    //    public const long UdpHostEnd = 50000;
    //    public const long TcpHostStart = 50001;
    //    public const long TcpHostEnd = 90000;

    //    public static implicit operator long(HostID value)
    //    {
    //        return value.mValue;
    //    }
    //    public static implicit operator HostID(long value)
    //    {
    //        HostID temp = new HostID();
    //        temp.mValue = value;
    //        return temp;
    //    }
    //    public static HostID operator ++(HostID value)
    //    {
    //        value.mValue++;
    //        return value;
    //    }

    //    public int CompareTo(long value) { return mValue.CompareTo(value); }
    //    public int CompareTo(object value) { return mValue.CompareTo(value); }
    //    public bool Equals(long obj) { return mValue.Equals(obj); }
    //    public bool Equals(HostID obj) { return mValue == obj; }
    //    public override bool Equals(object obj)
    //    {
    //        if (typeof(HostID) != obj.GetType())
    //            return false;
    //        HostID val = (HostID)obj;
    //        return mValue.Equals(val);
    //    }
    //    public override int GetHashCode()
    //    {
    //        return mValue.GetHashCode();
    //    }
    //    public TypeCode GetTypeCode() { return mValue.GetTypeCode(); }
    //    public static long Parse(string s) { return Int64.Parse(s); }
    //    public static long Parse(string s, IFormatProvider provider) { return Int64.Parse(s, provider); }
    //    public static long Parse(string s, NumberStyles style) { return Int64.Parse(s, style); }
    //    public static long Parse(string s, NumberStyles style, IFormatProvider provider) { return Int64.Parse(s, style, provider); }
    //    public override string ToString() { return mValue.ToString(); }
    //    public string ToString(IFormatProvider provider) { return mValue.ToString(); }
    //    public string ToString(string format) { return mValue.ToString(); }
    //    public string ToString(string format, IFormatProvider provider) { return mValue.ToString(); }
    //    public static bool TryParse(string s, out long result) { return Int64.TryParse(s, out result); }
    //    public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out long result) { return Int64.TryParse(s, style, provider, out result); }

    //    public bool ToBoolean(IFormatProvider provider) { return Convert.ToBoolean(mValue); }
    //    public byte ToByte(IFormatProvider provider) { return Convert.ToByte(mValue); }
    //    public char ToChar(IFormatProvider provider) { return Convert.ToChar(mValue); }
    //    public DateTime ToDateTime(IFormatProvider provider) { return Convert.ToDateTime(mValue); }
    //    public decimal ToDecimal(IFormatProvider provider) { return Convert.ToDecimal(mValue); }
    //    public double ToDouble(IFormatProvider provider) { return Convert.ToDouble(mValue); }
    //    public short ToInt16(IFormatProvider provider) { return Convert.ToInt16(mValue); }
    //    public int ToInt32(IFormatProvider provider) { return Convert.ToInt32(mValue); }
    //    public long ToInt64(IFormatProvider provider) { return Convert.ToInt64(mValue); }
    //    public sbyte ToSByte(IFormatProvider provider) { return Convert.ToSByte(mValue); }
    //    public float ToSingle(IFormatProvider provider) { return Convert.ToSingle(mValue); }
    //    public object ToType(Type conversionType, IFormatProvider provider) { return typeof(HostID); }
    //    public ushort ToUInt16(IFormatProvider provider) { return Convert.ToUInt16(mValue); }
    //    public uint ToUInt32(IFormatProvider provider) { return Convert.ToUInt32(mValue); }
    //    public ulong ToUInt64(IFormatProvider provider) { return Convert.ToUInt64(mValue); }
    //}
}
