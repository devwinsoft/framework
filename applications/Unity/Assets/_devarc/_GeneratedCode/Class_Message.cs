using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using LitJson;
namespace Devarc
{
	public class _MESSAGE : IBaseObejct
	{
		public static MESSAGE Parse(string name)
		{
			int result;
			if (Int32.TryParse(name, out result))
				return (MESSAGE)result;
			if (name == "SUCCESS")
				return MESSAGE.SUCCESS;
			return (MESSAGE)0;
		}
		public MESSAGE             ID;

		public _MESSAGE()
		{
		}
		public _MESSAGE(_MESSAGE obj)
		{
			Initialize(obj);
		}
		public _MESSAGE(PropTable obj)
		{
			Initialize(obj);
		}
		public void Initialize(IBaseObejct from)
		{
			_MESSAGE obj = from as _MESSAGE;
			if (obj == null)
			{
				Log.Message(LOG_TYPE.ERROR, "Cannot Initialize [name]:_MESSAGE");
				return;
			}
			ID                  = obj.ID;
		}
		public void Initialize(PropTable obj)
		{
			ID                  = _MESSAGE.Parse(obj.ToStr("ID"));
		}
		public void Initialize(JsonData obj)
		{
			if (obj.Keys.Contains("ID")) ID = _MESSAGE.Parse(obj["ID"].ToString()); else ID = default(MESSAGE);
		}
		public override string ToString()
		{
		    StringBuilder sb = new StringBuilder();
		    sb.Append("{"); sb.Append(" \"ID\":"); sb.Append("\""); sb.Append(ID.ToString()); sb.Append("\"");
		    sb.Append("}");
		    return sb.ToString();
		}
		public string ToJson()
		{
		    StringBuilder sb = new StringBuilder();
		    sb.Append("{"); sb.Append("\"ID\":"); sb.Append("\""); sb.Append(ID.ToString()); sb.Append("\"");
		    sb.Append("}");
		    return sb.ToString();
		}
		public PropTable ToTable()
		{
			PropTable obj = new PropTable("_MESSAGE");
			obj.Attach("Name", "", CLASS_TYPE.VALUE, false, ID.ToString());
			obj.Attach("ID", "MESSAGE", CLASS_TYPE.VALUE, true, ((int)ID).ToString());
			return obj;
		}
	}
	public static partial class Marshaler
	{
	    public static bool Read(NetBuffer msg, _MESSAGE obj)
	    {
	        bool success = true;
			success = success ? Marshaler.Read(msg, ref obj.ID) : false;
	        return success;
	    }
	    public static void Write(NetBuffer msg, _MESSAGE obj)
	    {
			Marshaler.Write(msg, obj.ID);
	    }
	    public static bool Read(NetBuffer msg, List<_MESSAGE> list)
	    {
	        bool success = true;
	        int cnt = msg.ReadInt16();
	        for (int i = 0; i < cnt; i++)
	        {
				_MESSAGE obj = new _MESSAGE();
				success = success ? Marshaler.Read(msg, ref obj.ID) : false;
				list.Add(obj);
	        }
	        return success;
	    }
	    public static void Write(NetBuffer msg, List<_MESSAGE> list)
	    {
	        msg.Write((Int16)list.Count);
	        foreach (_MESSAGE obj in list)
	        {
				Marshaler.Write(msg, obj.ID);
	        }
	    }
	}
	public class T_MESSAGE : _MESSAGE, IContents<MESSAGE>, IDisposable
	{
	    public static Container_C1<T_MESSAGE, MESSAGE> LIST = new Container_C1<T_MESSAGE, MESSAGE>();
	    public MESSAGE GetKey1()
	    {
	        return base.ID;
	    }
	    public void OnAlloc(MESSAGE key)
	    {
	        base.ID = key;
	    }
	    public void OnFree()
	    {
	    }
	    public void Dispose()
	    {
	    }
	}
	public enum MESSAGE
	{
		SUCCESS             = 0,
	}
	public static partial class Marshaler
	{
	    public static bool Read(NetBuffer msg, ref MESSAGE obj)
	    {
	        try
	        {
	            obj = (MESSAGE)msg.ReadInt32();
	            return true;
	        }
	        catch (System.Exception)
	        {
	            return false;
	        }
	    }
	    public static void Write(NetBuffer msg, MESSAGE obj)
	    {
	        msg.Write((Int32)obj);
	    }
	    public static bool Read(NetBuffer msg, out MESSAGE[] obj)
	    {
	        try
	        {
	            int cnt = msg.ReadInt16();
	            obj = new MESSAGE[cnt];
	            for (int i = 0; i < cnt; i++)
	            {
	                obj[i] = (MESSAGE)msg.ReadInt32();
	            }
	            return true;
	        }
	        catch (System.Exception)
	        {
	            obj = null;;
	            return false;
	        }
	    }
	    public static bool Read(NetBuffer msg, List<MESSAGE> obj)
	    {
	        try
	        {
	            int cnt = msg.ReadInt16();
	            obj = new List<MESSAGE>();
	            for (int i = 0; i < cnt; i++)
	            {
	                obj[i] = (MESSAGE)msg.ReadInt32();
	            }
	            return true;
	        }
	        catch (System.Exception)
	        {
	            obj = null;;
	            return false;
	        }
	    }
	    public static void Write(NetBuffer msg, MESSAGE[] list)
	    {
	        msg.Write((Int16)list.Length);
	        foreach (MESSAGE obj in list)
	        {
	            msg.Write((Int32)obj);
	        }
	    }
	    public static void Write(NetBuffer msg, List<MESSAGE> list)
	    {
	        msg.Write((Int16)list.Count);
	        foreach (MESSAGE obj in list)
	        {
	            msg.Write((Int32)obj);
	        }
	    }
	}
} // end of namespace
namespace Devarc
{
} // end of namespace
namespace Devarc
{
	public partial class BinWriter
	{
	}
} // end of namespace
