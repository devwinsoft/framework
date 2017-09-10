using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using LitJson;
namespace Devarc
{
    [System.Serializable]
    public class SpineAnimationName : IBaseObejct
    {
        public static implicit operator string(SpineAnimationName obj)
        {
            return obj.value;
        }
        public static implicit operator SpineAnimationName(string obj)
        {
            return new SpineAnimationName(obj);
        }
        public SpineAnimationName(string obj)
        {
            this.value = obj;
            this.speed = 1f;
        }

        public string value = "";
        public float speed = 1f;

        public SpineAnimationName()
        {
        }
        public SpineAnimationName(SpineAnimationName obj)
        {
            Initialize(obj);
        }
        public SpineAnimationName(PropTable obj)
        {
            Initialize(obj);
        }
        public bool IsDefault
        {
            get
            {
                if (string.IsNullOrEmpty(value) == false) return false;
                return true;
            }
        }
        public void Initialize(IBaseObejct from)
        {
            SpineAnimationName obj = from as SpineAnimationName;
            if (obj == null)
            {
                Log.Error("Cannot Initialize [name]:SpineAnimationName");
                return;
            }
            value = obj.value;
            speed = obj.speed;
        }
        public void Initialize(PropTable obj)
        {
            value = obj.GetStr("value");
            speed = obj.GetFloat("speed");
        }
        public void Initialize(JsonData obj)
        {
            if (obj.Keys.Contains("value")) value = obj["value"].ToString(); else value = default(string);
            if (obj.Keys.Contains("speed")) value = obj["speed"].ToString(); else speed = 1f;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{"); sb.Append(" \"value\":"); sb.Append("\""); sb.Append(value); sb.Append("\"");
            sb.Append(","); sb.Append(" \"speed\":"); sb.Append("\""); sb.Append(value); sb.Append("\"");
            sb.Append("}");
            return sb.ToString();
        }
        public string ToJson()
        {
            if (IsDefault) { return "{}"; }
            StringBuilder sb = new StringBuilder();
            sb.Append("{"); sb.Append("\"value\":"); sb.Append("\""); sb.Append(value); sb.Append("\"");
            sb.Append(","); sb.Append("\"speed\":"); sb.Append("\""); sb.Append(value); sb.Append("\"");
            sb.Append("}");
            return sb.ToString();
        }
        public PropTable ToTable()
        {
            PropTable obj = new PropTable("SpineAnimationName");
            obj.Attach("value", "string", CLASS_TYPE.VALUE, false, value);
            obj.Attach("speed", "float", CLASS_TYPE.VALUE, false, speed.ToString());
            return obj;
        }
    }
    public static partial class Marshaler
    {
        public static bool Read(NetBuffer msg, SpineAnimationName obj)
        {
            bool success = true;
            success = success ? Marshaler.Read(msg, ref obj.value) : false;
            success = success ? Marshaler.Read(msg, ref obj.speed) : false;
            return success;
        }
        public static void Write(NetBuffer msg, SpineAnimationName obj)
        {
            Marshaler.Write(msg, obj.value);
            Marshaler.Write(msg, obj.speed);
        }
        public static bool Read(NetBuffer msg, List<SpineAnimationName> list)
        {
            bool success = true;
            int cnt = msg.ReadInt16();
            for (int i = 0; i < cnt; i++)
            {
                SpineAnimationName obj = new SpineAnimationName();
                success = success ? Marshaler.Read(msg, ref obj.value) : false;
                success = success ? Marshaler.Read(msg, ref obj.speed) : false;
                list.Add(obj);
            }
            return success;
        }
        public static void Write(NetBuffer msg, List<SpineAnimationName> list)
        {
            msg.Write((Int16)list.Count);
            foreach (SpineAnimationName obj in list)
            {
                Marshaler.Write(msg, obj.value);
                Marshaler.Write(msg, obj.speed);
            }
        }
    }


    [System.Serializable]
    public class SpineBoneName : IBaseObejct
    {
        public static implicit operator string(SpineBoneName obj)
        {
            return obj.value;
        }
        public static implicit operator SpineBoneName(string obj)
        {
            return new SpineBoneName(obj);
        }
        public SpineBoneName(string obj)
        {
            this.value = obj;
        }
        public string value = "";

        public SpineBoneName()
        {
        }
        public SpineBoneName(SpineBoneName obj)
        {
            Initialize(obj);
        }
        public SpineBoneName(PropTable obj)
        {
            Initialize(obj);
        }
        public bool IsDefault
        {
            get
            {
                if (string.IsNullOrEmpty(value) == false) return false;
                return true;
            }
        }
        public void Initialize(IBaseObejct from)
        {
            SpineBoneName obj = from as SpineBoneName;
            if (obj == null)
            {
                Log.Error("Cannot Initialize [name]:SpineBoneName");
                return;
            }
            value = obj.value;
        }
        public void Initialize(PropTable obj)
        {
            value = obj.GetStr("value");
        }
        public void Initialize(JsonData obj)
        {
            if (obj.Keys.Contains("value")) value = obj["value"].ToString(); else value = default(string);
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{"); sb.Append(" \"value\":"); sb.Append("\""); sb.Append(value); sb.Append("\"");
            sb.Append("}");
            return sb.ToString();
        }
        public string ToJson()
        {
            if (IsDefault) { return "{}"; }
            StringBuilder sb = new StringBuilder();
            sb.Append("{"); sb.Append("\"value\":"); sb.Append("\""); sb.Append(value); sb.Append("\"");
            sb.Append("}");
            return sb.ToString();
        }
        public PropTable ToTable()
        {
            PropTable obj = new PropTable("SpineBoneName");
            obj.Attach("value", "string", CLASS_TYPE.VALUE, false, value);
            return obj;
        }
    }
    public static partial class Marshaler
    {
        public static bool Read(NetBuffer msg, SpineBoneName obj)
        {
            bool success = true;
            success = success ? Marshaler.Read(msg, ref obj.value) : false;
            return success;
        }
        public static void Write(NetBuffer msg, SpineBoneName obj)
        {
            Marshaler.Write(msg, obj.value);
        }
        public static bool Read(NetBuffer msg, List<SpineBoneName> list)
        {
            bool success = true;
            int cnt = msg.ReadInt16();
            for (int i = 0; i < cnt; i++)
            {
                SpineBoneName obj = new SpineBoneName();
                success = success ? Marshaler.Read(msg, ref obj.value) : false;
                list.Add(obj);
            }
            return success;
        }
        public static void Write(NetBuffer msg, List<SpineBoneName> list)
        {
            msg.Write((Int16)list.Count);
            foreach (SpineBoneName obj in list)
            {
                Marshaler.Write(msg, obj.value);
            }
        }
    }
}