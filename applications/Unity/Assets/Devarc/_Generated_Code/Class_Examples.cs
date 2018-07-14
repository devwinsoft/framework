using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using LitJson;
namespace Devarc
{
	[System.Serializable]
	public partial class _TEST_ENUM : IBaseObejct<TEST_ENUM>
	{
		public static TEST_ENUM Parse(string name)
		{
			return FrameworkUtil.Parse<TEST_ENUM>(name);
		}
		public string              NAME = "";
		public TEST_ENUM           ID;

		public _TEST_ENUM()
		{
		}
		public _TEST_ENUM(_TEST_ENUM obj)
		{
			Initialize(obj);
		}
		public _TEST_ENUM(PropTable obj)
		{
			Initialize(obj);
		}
		public TEST_ENUM GetKey() { return ID; }
		public string GetQuery_Select(TEST_ENUM _key)
		{
			return string.Format("select * from TEST_ENUM where ID='{0}';", _key);
		}
		public string GetQuery_InsertOrUpdate()
		{
			PropTable obj = ToTable();
			return string.Format("insert into _TEST_ENUM (`NAME`, `ID`) VALUES ('{0}', '{1}') on duplicate key update `NAME`='{0}', `ID`='{1}';", FrameworkUtil.InnerString(obj.GetStr("NAME")), obj.GetStr("ID"));
		}
		public bool IsDefault
		{
			get
			{
				if (string.IsNullOrEmpty(NAME) == false) return false;
				if ((int)ID != 0) return false;
				return true;
			}
		}
		public void Initialize(IBaseObejct from)
		{
			_TEST_ENUM obj = from as _TEST_ENUM;
			if (obj == null)
			{
				Log.Error("Cannot Initialize [name]:_TEST_ENUM");
				return;
			}
			NAME                = obj.NAME;
			ID                  = obj.ID;
		}
		public void Initialize(PropTable obj)
		{
			NAME                = obj.GetStr("NAME");
			ID                  = _TEST_ENUM.Parse(obj.GetStr("ID"));
		}
		public void Initialize(JsonData obj)
		{
			if (obj.Keys.Contains("NAME")) NAME = obj["NAME"].ToString(); else NAME = default(string);
			if (obj.Keys.Contains("ID")) ID = _TEST_ENUM.Parse(obj["ID"].ToString()); else ID = default(TEST_ENUM);
		}
		public void Initialize(IBaseReader obj)
		{
			NAME                = obj.GetString("NAME");
			ID                  = _TEST_ENUM.Parse(obj.GetString("ID"));
		}
		public override string ToString()
		{
		    StringBuilder sb = new StringBuilder();
		    sb.Append("{"); sb.Append(" \"NAME\":"); sb.Append("\""); sb.Append(NAME); sb.Append("\"");
		    sb.Append(","); sb.Append(" \"ID\":"); sb.Append("\""); sb.Append(ID.ToString()); sb.Append("\"");
		    sb.Append("}");
		    return sb.ToString();
		}
		public string ToJson()
		{
		    StringBuilder sb = new StringBuilder();
		    sb.Append("{"); sb.Append("\"NAME\":"); sb.Append("\""); sb.Append(FrameworkUtil.JsonString(NAME)); sb.Append("\"");
			if (default(TEST_ENUM) != ID) { sb.Append(","); sb.Append("\"ID\":"); sb.Append("\""); sb.Append(ID.ToString()); sb.Append("\""); }
		    sb.Append("}");
		    return sb.ToString();
		}
		public PropTable ToTable()
		{
			PropTable obj = new PropTable("_TEST_ENUM");
			obj.Attach("NAME", "string", CLASS_TYPE.VALUE, false, NAME);
			obj.Attach("ID", "TEST_ENUM", CLASS_TYPE.VALUE, true, ((int)ID).ToString());
			return obj;
		}
	}
	public static partial class Marshaler
	{
	    public static bool Read(NetBuffer msg, _TEST_ENUM obj)
	    {
	        bool success = true;
			success = success ? Marshaler.Read(msg, ref obj.NAME) : false;
			success = success ? Marshaler.Read(msg, ref obj.ID) : false;
	        return success;
	    }
	    public static bool Write(NetBuffer msg, _TEST_ENUM obj)
	    {
			Marshaler.Write(msg, obj.NAME);
			Marshaler.Write(msg, obj.ID);
	        return msg.IsError;
	    }
	    public static bool Read(NetBuffer msg, List<_TEST_ENUM> list)
	    {
	        bool success = true;
	        int cnt = msg.ReadInt16();
	        for (int i = 0; i < cnt; i++)
	        {
				_TEST_ENUM obj = new _TEST_ENUM();
				success = success ? Marshaler.Read(msg, ref obj.NAME) : false;
				success = success ? Marshaler.Read(msg, ref obj.ID) : false;
				list.Add(obj);
	        }
	        return success;
	    }
	    public static bool Write(NetBuffer msg, List<_TEST_ENUM> list)
	    {
	        msg.Write((Int16)list.Count);
	        foreach (_TEST_ENUM obj in list)
	        {
				Marshaler.Write(msg, obj.NAME);
				Marshaler.Write(msg, obj.ID);
	        }
	        return msg.IsError;
	    }
	}
	[System.Serializable]
	public class TEST_VECTOR : IBaseObejct
	{
		public float               x;
		public float               y;
		public float               z;

		public TEST_VECTOR()
		{
		}
		public TEST_VECTOR(TEST_VECTOR obj)
		{
			Initialize(obj);
		}
		public TEST_VECTOR(PropTable obj)
		{
			Initialize(obj);
		}
		public bool IsDefault
		{
			get
			{
				if (x != 0) return false;
				if (y != 0) return false;
				if (z != 0) return false;
				return true;
			}
		}
		public void Initialize(IBaseObejct from)
		{
			TEST_VECTOR obj = from as TEST_VECTOR;
			if (obj == null)
			{
				Log.Error("Cannot Initialize [name]:TEST_VECTOR");
				return;
			}
			x                   = obj.x;
			y                   = obj.y;
			z                   = obj.z;
		}
		public void Initialize(PropTable obj)
		{
			x                   = obj.GetFloat("x");
			y                   = obj.GetFloat("y");
			z                   = obj.GetFloat("z");
		}
		public void Initialize(JsonData obj)
		{
			if (obj.Keys.Contains("x")) float.TryParse(obj["x"].ToString(), out x); else x = default(float);
			if (obj.Keys.Contains("y")) float.TryParse(obj["y"].ToString(), out y); else y = default(float);
			if (obj.Keys.Contains("z")) float.TryParse(obj["z"].ToString(), out z); else z = default(float);
		}
		public void Initialize(IBaseReader obj)
		{
			x                   = obj.GetFloat("x");
			y                   = obj.GetFloat("y");
			z                   = obj.GetFloat("z");
		}
		public override string ToString()
		{
		    StringBuilder sb = new StringBuilder();
		    sb.Append("{"); sb.Append(" \"x\":"); sb.Append("\""); sb.Append(x.ToString()); sb.Append("\"");
		    sb.Append(","); sb.Append(" \"y\":"); sb.Append("\""); sb.Append(y.ToString()); sb.Append("\"");
		    sb.Append(","); sb.Append(" \"z\":"); sb.Append("\""); sb.Append(z.ToString()); sb.Append("\"");
		    sb.Append("}");
		    return sb.ToString();
		}
		public string ToJson()
		{
		    if (IsDefault) { return "{}"; }
		    StringBuilder sb = new StringBuilder();
		    sb.Append("{"); sb.Append("\"x\":"); sb.Append("\""); sb.Append(x.ToString()); sb.Append("\"");
			if (default(float) != y) { sb.Append(","); sb.Append("\"y\":"); sb.Append("\""); sb.Append(y.ToString()); sb.Append("\""); }
			if (default(float) != z) { sb.Append(","); sb.Append("\"z\":"); sb.Append("\""); sb.Append(z.ToString()); sb.Append("\""); }
		    sb.Append("}");
		    return sb.ToString();
		}
		public PropTable ToTable()
		{
			PropTable obj = new PropTable("TEST_VECTOR");
			obj.Attach("x", "float", CLASS_TYPE.VALUE, false, x.ToString());
			obj.Attach("y", "float", CLASS_TYPE.VALUE, false, y.ToString());
			obj.Attach("z", "float", CLASS_TYPE.VALUE, false, z.ToString());
			return obj;
		}
	}
	public static partial class Marshaler
	{
	    public static bool Read(NetBuffer msg, TEST_VECTOR obj)
	    {
	        bool success = true;
			success = success ? Marshaler.Read(msg, ref obj.x) : false;
			success = success ? Marshaler.Read(msg, ref obj.y) : false;
			success = success ? Marshaler.Read(msg, ref obj.z) : false;
	        return success;
	    }
	    public static bool Write(NetBuffer msg, TEST_VECTOR obj)
	    {
			Marshaler.Write(msg, obj.x);
			Marshaler.Write(msg, obj.y);
			Marshaler.Write(msg, obj.z);
	        return msg.IsError;
	    }
	    public static bool Read(NetBuffer msg, List<TEST_VECTOR> list)
	    {
	        bool success = true;
	        int cnt = msg.ReadInt16();
	        for (int i = 0; i < cnt; i++)
	        {
				TEST_VECTOR obj = new TEST_VECTOR();
				success = success ? Marshaler.Read(msg, ref obj.x) : false;
				success = success ? Marshaler.Read(msg, ref obj.y) : false;
				success = success ? Marshaler.Read(msg, ref obj.z) : false;
				list.Add(obj);
	        }
	        return success;
	    }
	    public static bool Write(NetBuffer msg, List<TEST_VECTOR> list)
	    {
	        msg.Write((Int16)list.Count);
	        foreach (TEST_VECTOR obj in list)
	        {
				Marshaler.Write(msg, obj.x);
				Marshaler.Write(msg, obj.y);
				Marshaler.Write(msg, obj.z);
	        }
	        return msg.IsError;
	    }
	}
	[System.Serializable]
	public class TEST_PLAYER : IBaseObejct
	{
		public string              Name = "";
		public TEST_VECTOR         Pos = new TEST_VECTOR();

		public TEST_PLAYER()
		{
		}
		public TEST_PLAYER(TEST_PLAYER obj)
		{
			Initialize(obj);
		}
		public TEST_PLAYER(PropTable obj)
		{
			Initialize(obj);
		}
		public bool IsDefault
		{
			get
			{
				if (string.IsNullOrEmpty(Name) == false) return false;
				if (Pos.IsDefault == false) return false;
				return true;
			}
		}
		public void Initialize(IBaseObejct from)
		{
			TEST_PLAYER obj = from as TEST_PLAYER;
			if (obj == null)
			{
				Log.Error("Cannot Initialize [name]:TEST_PLAYER");
				return;
			}
			Name                = obj.Name;
			Pos.Initialize(obj.Pos);
		}
		public void Initialize(PropTable obj)
		{
			Name                = obj.GetStr("Name");
			Pos.Initialize(obj.GetTable("Pos"));
		}
		public void Initialize(JsonData obj)
		{
			if (obj.Keys.Contains("Name")) Name = obj["Name"].ToString(); else Name = default(string);
			if (obj.Keys.Contains("Pos")) Pos.Initialize(obj["Pos"]);
		}
		public void Initialize(IBaseReader obj)
		{
			Name                = obj.GetString("Name");
			Pos.Initialize(JsonMapper.ToObject(obj.GetString("Pos")));
		}
		public override string ToString()
		{
		    StringBuilder sb = new StringBuilder();
		    sb.Append("{"); sb.Append(" \"Name\":"); sb.Append("\""); sb.Append(Name); sb.Append("\"");
		    sb.Append(","); sb.Append(" \"Pos\":"); sb.Append(Pos != null ? Pos.ToString() : "{}");
		    sb.Append("}");
		    return sb.ToString();
		}
		public string ToJson()
		{
		    if (IsDefault) { return "{}"; }
		    StringBuilder sb = new StringBuilder();
		    sb.Append("{"); sb.Append("\"Name\":"); sb.Append("\""); sb.Append(FrameworkUtil.JsonString(Name)); sb.Append("\"");
		    sb.Append(","); sb.Append("\"Pos\":"); sb.Append(Pos != null ? Pos.ToJson() : "{}");
		    sb.Append("}");
		    return sb.ToString();
		}
		public PropTable ToTable()
		{
			PropTable obj = new PropTable("TEST_PLAYER");
			obj.Attach("Name", "string", CLASS_TYPE.VALUE, false, Name);
			obj.Attach_Class("Pos", "TEST_VECTOR", Pos.ToTable());
			return obj;
		}
	}
	public static partial class Marshaler
	{
	    public static bool Read(NetBuffer msg, TEST_PLAYER obj)
	    {
	        bool success = true;
			success = success ? Marshaler.Read(msg, ref obj.Name) : false;
			success = success ? Marshaler.Read(msg, obj.Pos) : false;
	        return success;
	    }
	    public static bool Write(NetBuffer msg, TEST_PLAYER obj)
	    {
			Marshaler.Write(msg, obj.Name);
			Marshaler.Write(msg, obj.Pos);
	        return msg.IsError;
	    }
	    public static bool Read(NetBuffer msg, List<TEST_PLAYER> list)
	    {
	        bool success = true;
	        int cnt = msg.ReadInt16();
	        for (int i = 0; i < cnt; i++)
	        {
				TEST_PLAYER obj = new TEST_PLAYER();
				success = success ? Marshaler.Read(msg, ref obj.Name) : false;
				success = success ? Marshaler.Read(msg, obj.Pos) : false;
				list.Add(obj);
	        }
	        return success;
	    }
	    public static bool Write(NetBuffer msg, List<TEST_PLAYER> list)
	    {
	        msg.Write((Int16)list.Count);
	        foreach (TEST_PLAYER obj in list)
	        {
				Marshaler.Write(msg, obj.Name);
				Marshaler.Write(msg, obj.Pos);
	        }
	        return msg.IsError;
	    }
	}
	public enum TEST_ENUM
	{
		NONE                = 0,
		FIGHTER             = 1,
		MAGE                = 2,
	}
	public static partial class Marshaler
	{
	    public static bool Read(NetBuffer msg, ref TEST_ENUM obj)
	    {
	        obj = (TEST_ENUM)msg.ReadInt32();
	        return !msg.IsError;
	    }
	    public static bool Write(NetBuffer msg, TEST_ENUM obj)
	    {
	        msg.Write((Int32)obj);
	        return !msg.IsError;
	    }
	    public static bool Read(NetBuffer msg, out TEST_ENUM[] obj)
	    {
	        int cnt = msg.ReadInt16();
	        obj = new TEST_ENUM[cnt];
	        for (int i = 0; i < cnt; i++)
	        {
	            obj[i] = (TEST_ENUM)msg.ReadInt32();
	        }
	        return !msg.IsError;
	    }
	    public static bool Read(NetBuffer msg, List<TEST_ENUM> obj)
	    {
	        int cnt = msg.ReadInt16();
	        obj = new List<TEST_ENUM>();
	        for (int i = 0; i < cnt; i++)
	        {
	            obj[i] = (TEST_ENUM)msg.ReadInt32();
	        }
	        return !msg.IsError;
	    }
	    public static bool Write(NetBuffer msg, TEST_ENUM[] list)
	    {
	        msg.Write((Int16)list.Length);
	        foreach (TEST_ENUM obj in list)
	        {
	            msg.Write((Int32)obj);
	        }
	        return !msg.IsError;
	    }
	    public static bool Write(NetBuffer msg, List<TEST_ENUM> list)
	    {
	        msg.Write((Int16)list.Count);
	        foreach (TEST_ENUM obj in list)
	        {
	            msg.Write((Int32)obj);
	        }
	        return !msg.IsError;
	    }
	}
} // end of namespace
