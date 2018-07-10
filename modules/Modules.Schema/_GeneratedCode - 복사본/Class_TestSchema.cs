using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using LitJson;
namespace Devarc
{
	[System.Serializable]
	public partial class DataCharacter : IBaseObejct<UNIT>
	{
		public UNIT                unit_type;
		public string              name { get { return Table.T_LString.GetAt(_name.Key); } }
		LString             _name = new LString();
		public List<UNIT> items = new List<UNIT>();
		public List<DataAbility> stats = new List<DataAbility>();
		public DataAbility         ability = new DataAbility();
		public List<string> nodes = new List<string>();
		public UInt32              unit_uid;
		public string              specialCode { get { return Table.T_LString.GetAt(_specialCode.Key); } }
		LString             _specialCode = new LString();

		public DataCharacter()
		{
		}
		public DataCharacter(DataCharacter obj)
		{
			Initialize(obj);
		}
		public DataCharacter(PropTable obj)
		{
			Initialize(obj);
		}
		public UNIT GetKey() { return unit_type; }
		public string GetSelectQuery(UNIT _key) { return string.Format("select unit_type, name, items, stats, ability, nodes, unit_uid, specialCode from DataCharacter where unit_type='{0}';", _key); }
		public bool IsDefault
		{
			get
			{
				if ((int)unit_type != 0) return false;
				if (items.Count > 0) return false;
				if (stats.Count > 0) return false;
				if (ability.IsDefault == false) return false;
				if (nodes.Count > 0) return false;
				if (unit_uid != 0) return false;
				return true;
			}
		}
		public void Initialize(IBaseObejct from)
		{
			DataCharacter obj = from as DataCharacter;
			if (obj == null)
			{
				Log.Error("Cannot Initialize [name]:DataCharacter");
				return;
			}
			unit_type           = obj.unit_type;
			_name.Initialize(obj._name);
			items.Clear();
			items.AddRange(obj.items);
			stats.Clear();
			foreach(DataAbility _obj in obj.stats) { DataAbility _new = new DataAbility(_obj); stats.Add(_new); }
			ability.Initialize(obj.ability);
			nodes.Clear();
			nodes.AddRange(obj.nodes);
			unit_uid            = obj.unit_uid;
			_specialCode.Initialize(obj._specialCode);
		}
		public void Initialize(PropTable obj)
		{
			unit_type           = _UNIT.Parse(obj.GetStr("unit_type"));
			_name.Key = FrameworkUtil.MakeLStringKey("DataCharacter", "name", unit_type.ToString());
			_name.Value = obj.GetStr("name");
			items.Clear();
			JsonData __items = JsonMapper.ToObject(obj.GetStr("items"));
			if (__items != null && __items.IsArray) { foreach (var node in __items as IList) { items.Add(_UNIT.Parse(node.ToString())); } }
			stats.Clear();
			JsonData __stats = JsonMapper.ToObject(obj.GetStr("stats"));
			if (__stats != null && __stats.IsArray) { foreach (var node in __stats as IList) { DataAbility _v = new DataAbility(); _v.Initialize(node as JsonData); stats.Add(_v); } }
			ability.Initialize(obj.GetTable("ability"));
			obj.GetList<string>("nodes", nodes);
			unit_uid            = obj.GetUInt32("unit_uid");
			_specialCode.Key = obj.GetStr("specialCode");
			_specialCode.Value = obj.GetStr("specialCode");
		}
		public void Initialize(JsonData obj)
		{
			if (obj.Keys.Contains("unit_type")) unit_type = _UNIT.Parse(obj["unit_type"].ToString()); else unit_type = default(UNIT);
			_name.Key = FrameworkUtil.MakeLStringKey("DataCharacter", "name", unit_type.ToString());
			if (obj.Keys.Contains("items")) foreach (JsonData node in obj["items"]) { items.Add(_UNIT.Parse(node.ToString())); }
			if (obj.Keys.Contains("stats")) foreach (JsonData node in obj["stats"]) { DataAbility _v = new DataAbility(); _v.Initialize(node); stats.Add(_v); }
			if (obj.Keys.Contains("ability")) ability.Initialize(obj["ability"]);
			if (obj.Keys.Contains("nodes")) foreach (JsonData node in obj["nodes"]) { nodes.Add(DES.Decrypt(node.ToString())); }
			if (obj.Keys.Contains("unit_uid")) uint.TryParse(obj["unit_uid"].ToString(), out unit_uid); else unit_uid = default(uint);
			if (obj.Keys.Contains("specialCode")) _specialCode.Key = obj["specialCode"].ToString(); else _specialCode.Key = default(string);
		}
		public void Initialize(SQLite_Reader obj)
		{
			unit_type           = _UNIT.Parse(obj.GetString(0));
			_name.Key = FrameworkUtil.MakeLStringKey("DataCharacter", "name", unit_type.ToString());
			string __items = obj.GetString(2); items.Clear(); if (!string.IsNullOrEmpty(__items)) foreach (JsonData node in JsonMapper.ToObject(__items)) { items.Add(_UNIT.Parse(node.ToString())); };
			string __stats = obj.GetString(3); stats.Clear(); if (!string.IsNullOrEmpty(__stats)) FrameworkUtil.FillList<DataAbility>(__stats, stats);
			ability.Initialize(JsonMapper.ToObject(obj.GetString(4)));
			string __nodes = obj.GetString(5); nodes.Clear(); if (!string.IsNullOrEmpty(__nodes)) foreach (JsonData node in JsonMapper.ToObject(__nodes)) { nodes.Add(node.ToString()); };
			unit_uid            = (uint)obj.GetUInt32(6);
			_specialCode.Key = obj.GetString(7);
		}
		public override string ToString()
		{
		    StringBuilder sb = new StringBuilder();
		    sb.Append("{"); sb.Append(" \"unit_type\":"); sb.Append("\""); sb.Append(unit_type.ToString()); sb.Append("\"");
		    sb.Append(","); sb.Append(" \"name\":"); sb.Append("\""); sb.Append(name); sb.Append("\"");
		    sb.Append(","); sb.Append(" \"items\":"); sb.Append("["); for (int i = 0; i < items.Count; i++) { UNIT _obj = items[i]; if (i > 0) sb.Append(","); sb.Append(string.Format("\"{0}\"", _obj)); } sb.Append("]");
		    sb.Append(","); sb.Append(" \"stats\":"); sb.Append("["); for (int i = 0; i < stats.Count; i++) { sb.Append(stats[i].ToString()); } sb.Append("]");
		    sb.Append(","); sb.Append(" \"ability\":"); sb.Append(ability != null ? ability.ToString() : "{}");
		    sb.Append(","); sb.Append(" \"nodes\":"); sb.Append("["); for (int i = 0; i < nodes.Count; i++) { string _obj = nodes[i]; if (i > 0) sb.Append(","); sb.Append("\""); sb.Append(_obj); sb.Append("\""); } sb.Append("]");
		    sb.Append(","); sb.Append(" \"unit_uid\":"); sb.Append("\""); sb.Append(unit_uid.ToString()); sb.Append("\"");
		    sb.Append(","); sb.Append(" \"specialCode\":"); sb.Append("\""); sb.Append(specialCode); sb.Append("\"");
		    sb.Append("}");
		    return sb.ToString();
		}
		public string ToJson()
		{
		    if (IsDefault) { return "{}"; }
		    StringBuilder sb = new StringBuilder();
		    sb.Append("{"); sb.Append("\"unit_type\":"); sb.Append("\""); sb.Append(unit_type.ToString()); sb.Append("\"");
			if (items.Count > 0) { sb.Append(","); sb.Append("\"items\":"); sb.Append("["); for (int i = 0; i < items.Count; i++) { UNIT _obj = items[i]; if (i > 0) sb.Append(","); sb.Append(string.Format("\"{0}\"", _obj)); } sb.Append("]"); }
			if (stats.Count > 0) { sb.Append(","); sb.Append("\"stats\":"); sb.Append("["); for (int i = 0; i < stats.Count; i++) { if (i > 0) sb.Append(","); sb.Append(stats[i].ToJson()); } sb.Append("]"); }
		    sb.Append(","); sb.Append("\"ability\":"); sb.Append(ability != null ? ability.ToJson() : "{}");
			if (nodes.Count > 0) { sb.Append(","); sb.Append("\"nodes\":"); sb.Append("["); for (int i = 0; i < nodes.Count; i++) { string _obj = nodes[i]; if (i > 0) sb.Append(","); sb.Append("\""); sb.Append(DES.Encrypt(_obj)); sb.Append("\""); } sb.Append("]"); }
			if (default(uint) != unit_uid) { sb.Append(","); sb.Append("\"unit_uid\":"); sb.Append("\""); sb.Append(unit_uid.ToString()); sb.Append("\""); }
			if (string.IsNullOrEmpty(_specialCode.Key) == false) { sb.Append(","); sb.Append("\"specialCode\":"); sb.Append("\""); sb.Append(_specialCode.Key); sb.Append("\""); }
		    sb.Append("}");
		    return sb.ToString();
		}
		public PropTable ToTable()
		{
			PropTable obj = new PropTable("DataCharacter");
			obj.Attach("unit_type", "UNIT", CLASS_TYPE.VALUE, KEY_TYPE.MAP, unit_type.ToString());
			obj.Attach("name", "LString", CLASS_TYPE.VALUE, KEY_TYPE.NONE, _name.Value);
			obj.Attach_List<UNIT>("items", "UNIT", VAR_TYPE.ENUM, items);
			obj.Attach_List<DataAbility>("stats", "DataAbility", VAR_TYPE.CLASS, stats);
			obj.Attach_Class("ability", "DataAbility", ability.ToTable());
			obj.Attach_List<string>("nodes", "string", VAR_TYPE.CSTRING, nodes);
			obj.Attach("unit_uid", "uint", CLASS_TYPE.VALUE, KEY_TYPE.NONE, unit_uid.ToString());
			obj.Attach("specialCode", "LString", CLASS_TYPE.VALUE, KEY_TYPE.NONE, _specialCode.Key);
			return obj;
		}
	}
	public static partial class Marshaler
	{
	    public static bool Read(NetBuffer msg, DataCharacter obj)
	    {
	        bool success = true;
			success = success ? Marshaler.Read(msg, ref obj.unit_type) : false;
			success = success ? Marshaler.Read(msg, obj.items) : false;
			success = success ? Marshaler.Read(msg, obj.stats) : false;
			success = success ? Marshaler.Read(msg, obj.ability) : false;
			success = success ? Marshaler.Read(msg, ref obj.unit_uid) : false;
	        return success;
	    }
	    public static bool Write(NetBuffer msg, DataCharacter obj)
	    {
			Marshaler.Write(msg, obj.unit_type);
			Marshaler.Write(msg, obj.name);
			Marshaler.Write(msg, obj.items);
			Marshaler.Write(msg, obj.stats);
			Marshaler.Write(msg, obj.ability);
			Marshaler.Write(msg, obj.nodes);
			Marshaler.Write(msg, obj.unit_uid);
			Marshaler.Write(msg, obj.specialCode);
	        return msg.IsError;
	    }
	    public static bool Read(NetBuffer msg, List<DataCharacter> list)
	    {
	        bool success = true;
	        int cnt = msg.ReadInt16();
	        for (int i = 0; i < cnt; i++)
	        {
				DataCharacter obj = new DataCharacter();
				success = success ? Marshaler.Read(msg, ref obj.unit_type) : false;
				success = success ? Marshaler.Read(msg, obj.items) : false;
				success = success ? Marshaler.Read(msg, obj.stats) : false;
				success = success ? Marshaler.Read(msg, obj.ability) : false;
				success = success ? Marshaler.Read(msg, ref obj.unit_uid) : false;
				list.Add(obj);
	        }
	        return success;
	    }
	    public static bool Write(NetBuffer msg, List<DataCharacter> list)
	    {
	        msg.Write((Int16)list.Count);
	        foreach (DataCharacter obj in list)
	        {
				Marshaler.Write(msg, obj.unit_type);
				Marshaler.Write(msg, obj.name);
				Marshaler.Write(msg, obj.items);
				Marshaler.Write(msg, obj.stats);
				Marshaler.Write(msg, obj.ability);
				Marshaler.Write(msg, obj.nodes);
				Marshaler.Write(msg, obj.unit_uid);
				Marshaler.Write(msg, obj.specialCode);
	        }
	        return msg.IsError;
	    }
	}
	[System.Serializable]
	public class DataAbility : IBaseObejct
	{
		public Int32               str;
		public Int32               dex;
		public Int32               vit;

		public DataAbility()
		{
		}
		public DataAbility(DataAbility obj)
		{
			Initialize(obj);
		}
		public DataAbility(PropTable obj)
		{
			Initialize(obj);
		}
		public bool IsDefault
		{
			get
			{
				if (str != 0) return false;
				if (dex != 0) return false;
				if (vit != 0) return false;
				return true;
			}
		}
		public void Initialize(IBaseObejct from)
		{
			DataAbility obj = from as DataAbility;
			if (obj == null)
			{
				Log.Error("Cannot Initialize [name]:DataAbility");
				return;
			}
			str                 = obj.str;
			dex                 = obj.dex;
			vit                 = obj.vit;
		}
		public void Initialize(PropTable obj)
		{
			str                 = obj.GetInt32("str");
			dex                 = obj.GetInt32("dex");
			vit                 = obj.GetInt32("vit");
		}
		public void Initialize(JsonData obj)
		{
			if (obj.Keys.Contains("str")) int.TryParse(obj["str"].ToString(), out str); else str = default(int);
			if (obj.Keys.Contains("dex")) int.TryParse(obj["dex"].ToString(), out dex); else dex = default(int);
			if (obj.Keys.Contains("vit")) int.TryParse(obj["vit"].ToString(), out vit); else vit = default(int);
		}
		public void Initialize(SQLite_Reader obj)
		{
			str                 = obj.GetInt32(0);
			dex                 = obj.GetInt32(1);
			vit                 = obj.GetInt32(2);
		}
		public override string ToString()
		{
		    StringBuilder sb = new StringBuilder();
		    sb.Append("{"); sb.Append(" \"str\":"); sb.Append("\""); sb.Append(str.ToString()); sb.Append("\"");
		    sb.Append(","); sb.Append(" \"dex\":"); sb.Append("\""); sb.Append(dex.ToString()); sb.Append("\"");
		    sb.Append(","); sb.Append(" \"vit\":"); sb.Append("\""); sb.Append(vit.ToString()); sb.Append("\"");
		    sb.Append("}");
		    return sb.ToString();
		}
		public string ToJson()
		{
		    if (IsDefault) { return "{}"; }
		    StringBuilder sb = new StringBuilder();
		    sb.Append("{"); sb.Append("\"str\":"); sb.Append("\""); sb.Append(str.ToString()); sb.Append("\"");
			if (default(int) != dex) { sb.Append(","); sb.Append("\"dex\":"); sb.Append("\""); sb.Append(dex.ToString()); sb.Append("\""); }
			if (default(int) != vit) { sb.Append(","); sb.Append("\"vit\":"); sb.Append("\""); sb.Append(vit.ToString()); sb.Append("\""); }
		    sb.Append("}");
		    return sb.ToString();
		}
		public PropTable ToTable()
		{
			PropTable obj = new PropTable("DataAbility");
			obj.Attach("str", "int", CLASS_TYPE.VALUE, KEY_TYPE.NONE, str.ToString());
			obj.Attach("dex", "int", CLASS_TYPE.VALUE, KEY_TYPE.NONE, dex.ToString());
			obj.Attach("vit", "int", CLASS_TYPE.VALUE, KEY_TYPE.NONE, vit.ToString());
			return obj;
		}
	}
	public static partial class Marshaler
	{
	    public static bool Read(NetBuffer msg, DataAbility obj)
	    {
	        bool success = true;
			success = success ? Marshaler.Read(msg, ref obj.str) : false;
			success = success ? Marshaler.Read(msg, ref obj.dex) : false;
			success = success ? Marshaler.Read(msg, ref obj.vit) : false;
	        return success;
	    }
	    public static bool Write(NetBuffer msg, DataAbility obj)
	    {
			Marshaler.Write(msg, obj.str);
			Marshaler.Write(msg, obj.dex);
			Marshaler.Write(msg, obj.vit);
	        return msg.IsError;
	    }
	    public static bool Read(NetBuffer msg, List<DataAbility> list)
	    {
	        bool success = true;
	        int cnt = msg.ReadInt16();
	        for (int i = 0; i < cnt; i++)
	        {
				DataAbility obj = new DataAbility();
				success = success ? Marshaler.Read(msg, ref obj.str) : false;
				success = success ? Marshaler.Read(msg, ref obj.dex) : false;
				success = success ? Marshaler.Read(msg, ref obj.vit) : false;
				list.Add(obj);
	        }
	        return success;
	    }
	    public static bool Write(NetBuffer msg, List<DataAbility> list)
	    {
	        msg.Write((Int16)list.Count);
	        foreach (DataAbility obj in list)
	        {
				Marshaler.Write(msg, obj.str);
				Marshaler.Write(msg, obj.dex);
				Marshaler.Write(msg, obj.vit);
	        }
	        return msg.IsError;
	    }
	}
	[System.Serializable]
	public class DataPlayer : IBaseObejct
	{
		public HostID              id;
		public VECTOR3             pos = new VECTOR3();

		public DataPlayer()
		{
		}
		public DataPlayer(DataPlayer obj)
		{
			Initialize(obj);
		}
		public DataPlayer(PropTable obj)
		{
			Initialize(obj);
		}
		public bool IsDefault
		{
			get
			{
				if (id != 0) return false;
				if (pos.IsDefault == false) return false;
				return true;
			}
		}
		public void Initialize(IBaseObejct from)
		{
			DataPlayer obj = from as DataPlayer;
			if (obj == null)
			{
				Log.Error("Cannot Initialize [name]:DataPlayer");
				return;
			}
			id                  = obj.id;
			pos.Initialize(obj.pos);
		}
		public void Initialize(PropTable obj)
		{
			id                  = (HostID)obj.GetInt16("id");
			pos.Initialize(obj.GetTable("pos"));
		}
		public void Initialize(JsonData obj)
		{
			if (obj.Keys.Contains("id")) HostID.TryParse(obj["id"].ToString(), out id); else id = default(short);
			if (obj.Keys.Contains("pos")) pos.Initialize(obj["pos"]);
		}
		public void Initialize(SQLite_Reader obj)
		{
			id                  = (HostID)obj.GetInt16(0);
			pos.Initialize(JsonMapper.ToObject(obj.GetString(1)));
		}
		public override string ToString()
		{
		    StringBuilder sb = new StringBuilder();
		    sb.Append("{"); sb.Append(" \"id\":"); sb.Append("\""); sb.Append(id.ToString()); sb.Append("\"");
		    sb.Append(","); sb.Append(" \"pos\":"); sb.Append(pos != null ? pos.ToString() : "{}");
		    sb.Append("}");
		    return sb.ToString();
		}
		public string ToJson()
		{
		    if (IsDefault) { return "{}"; }
		    StringBuilder sb = new StringBuilder();
		    sb.Append("{"); sb.Append("\"id\":"); sb.Append("\""); sb.Append(id.ToString()); sb.Append("\"");
		    sb.Append(","); sb.Append("\"pos\":"); sb.Append(pos != null ? pos.ToJson() : "{}");
		    sb.Append("}");
		    return sb.ToString();
		}
		public PropTable ToTable()
		{
			PropTable obj = new PropTable("DataPlayer");
			obj.Attach("id", "HostID", CLASS_TYPE.VALUE, KEY_TYPE.NONE, id.ToString());
			obj.Attach_Class("pos", "VECTOR3", pos.ToTable());
			return obj;
		}
	}
	public static partial class Marshaler
	{
	    public static bool Read(NetBuffer msg, DataPlayer obj)
	    {
	        bool success = true;
			success = success ? Marshaler.Read(msg, ref obj.id) : false;
			success = success ? Marshaler.Read(msg, obj.pos) : false;
	        return success;
	    }
	    public static bool Write(NetBuffer msg, DataPlayer obj)
	    {
			Marshaler.Write(msg, obj.id);
			Marshaler.Write(msg, obj.pos);
	        return msg.IsError;
	    }
	    public static bool Read(NetBuffer msg, List<DataPlayer> list)
	    {
	        bool success = true;
	        int cnt = msg.ReadInt16();
	        for (int i = 0; i < cnt; i++)
	        {
				DataPlayer obj = new DataPlayer();
				success = success ? Marshaler.Read(msg, ref obj.id) : false;
				success = success ? Marshaler.Read(msg, obj.pos) : false;
				list.Add(obj);
	        }
	        return success;
	    }
	    public static bool Write(NetBuffer msg, List<DataPlayer> list)
	    {
	        msg.Write((Int16)list.Count);
	        foreach (DataPlayer obj in list)
	        {
				Marshaler.Write(msg, obj.id);
				Marshaler.Write(msg, obj.pos);
	        }
	        return msg.IsError;
	    }
	}
	[System.Serializable]
	public class VECTOR3 : IBaseObejct
	{
		public float               x;
		public float               y;
		public float               z;

		public VECTOR3()
		{
		}
		public VECTOR3(VECTOR3 obj)
		{
			Initialize(obj);
		}
		public VECTOR3(PropTable obj)
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
			VECTOR3 obj = from as VECTOR3;
			if (obj == null)
			{
				Log.Error("Cannot Initialize [name]:VECTOR3");
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
		public void Initialize(SQLite_Reader obj)
		{
			x                   = obj.GetFloat(0);
			y                   = obj.GetFloat(1);
			z                   = obj.GetFloat(2);
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
			PropTable obj = new PropTable("VECTOR3");
			obj.Attach("x", "float", CLASS_TYPE.VALUE, KEY_TYPE.NONE, x.ToString());
			obj.Attach("y", "float", CLASS_TYPE.VALUE, KEY_TYPE.NONE, y.ToString());
			obj.Attach("z", "float", CLASS_TYPE.VALUE, KEY_TYPE.NONE, z.ToString());
			return obj;
		}
	}
	public static partial class Marshaler
	{
	    public static bool Read(NetBuffer msg, VECTOR3 obj)
	    {
	        bool success = true;
			success = success ? Marshaler.Read(msg, ref obj.x) : false;
			success = success ? Marshaler.Read(msg, ref obj.y) : false;
			success = success ? Marshaler.Read(msg, ref obj.z) : false;
	        return success;
	    }
	    public static bool Write(NetBuffer msg, VECTOR3 obj)
	    {
			Marshaler.Write(msg, obj.x);
			Marshaler.Write(msg, obj.y);
			Marshaler.Write(msg, obj.z);
	        return msg.IsError;
	    }
	    public static bool Read(NetBuffer msg, List<VECTOR3> list)
	    {
	        bool success = true;
	        int cnt = msg.ReadInt16();
	        for (int i = 0; i < cnt; i++)
	        {
				VECTOR3 obj = new VECTOR3();
				success = success ? Marshaler.Read(msg, ref obj.x) : false;
				success = success ? Marshaler.Read(msg, ref obj.y) : false;
				success = success ? Marshaler.Read(msg, ref obj.z) : false;
				list.Add(obj);
	        }
	        return success;
	    }
	    public static bool Write(NetBuffer msg, List<VECTOR3> list)
	    {
	        msg.Write((Int16)list.Count);
	        foreach (VECTOR3 obj in list)
	        {
				Marshaler.Write(msg, obj.x);
				Marshaler.Write(msg, obj.y);
				Marshaler.Write(msg, obj.z);
	        }
	        return msg.IsError;
	    }
	}
	[System.Serializable]
	public partial class _DIRECTION : IBaseObejct<DIRECTION>
	{
		public static DIRECTION Parse(string name)
		{
			return FrameworkUtil.Parse<DIRECTION>(name);
		}
		public string              Name = "";
		public DIRECTION           ID;

		public _DIRECTION()
		{
		}
		public _DIRECTION(_DIRECTION obj)
		{
			Initialize(obj);
		}
		public _DIRECTION(PropTable obj)
		{
			Initialize(obj);
		}
		public DIRECTION GetKey() { return ID; }
		public string GetSelectQuery(DIRECTION _key) { return string.Format("select Name, ID from DIRECTION where ID='{0}';", _key); }
		public bool IsDefault
		{
			get
			{
				if (string.IsNullOrEmpty(Name) == false) return false;
				if ((int)ID != 0) return false;
				return true;
			}
		}
		public void Initialize(IBaseObejct from)
		{
			_DIRECTION obj = from as _DIRECTION;
			if (obj == null)
			{
				Log.Error("Cannot Initialize [name]:_DIRECTION");
				return;
			}
			Name                = obj.Name;
			ID                  = obj.ID;
		}
		public void Initialize(PropTable obj)
		{
			Name                = obj.GetStr("Name");
			ID                  = _DIRECTION.Parse(obj.GetStr("ID"));
		}
		public void Initialize(JsonData obj)
		{
			if (obj.Keys.Contains("Name")) Name = obj["Name"].ToString(); else Name = default(string);
			if (obj.Keys.Contains("ID")) ID = _DIRECTION.Parse(obj["ID"].ToString()); else ID = default(DIRECTION);
		}
		public void Initialize(SQLite_Reader obj)
		{
			Name                = obj.GetString(0);
			ID                  = _DIRECTION.Parse(obj.GetString(1));
		}
		public override string ToString()
		{
		    StringBuilder sb = new StringBuilder();
		    sb.Append("{"); sb.Append(" \"Name\":"); sb.Append("\""); sb.Append(Name); sb.Append("\"");
		    sb.Append(","); sb.Append(" \"ID\":"); sb.Append("\""); sb.Append(ID.ToString()); sb.Append("\"");
		    sb.Append("}");
		    return sb.ToString();
		}
		public string ToJson()
		{
		    StringBuilder sb = new StringBuilder();
		    sb.Append("{"); sb.Append("\"Name\":"); sb.Append("\""); sb.Append(Name); sb.Append("\"");
			if (default(DIRECTION) != ID) { sb.Append(","); sb.Append("\"ID\":"); sb.Append("\""); sb.Append(ID.ToString()); sb.Append("\""); }
		    sb.Append("}");
		    return sb.ToString();
		}
		public PropTable ToTable()
		{
			PropTable obj = new PropTable("_DIRECTION");
			obj.Attach("Name", "string", CLASS_TYPE.VALUE, KEY_TYPE.NONE, Name);
			obj.Attach("ID", "DIRECTION", CLASS_TYPE.VALUE, KEY_TYPE.MAP, ((int)ID).ToString());
			return obj;
		}
	}
	public static partial class Marshaler
	{
	    public static bool Read(NetBuffer msg, _DIRECTION obj)
	    {
	        bool success = true;
			success = success ? Marshaler.Read(msg, ref obj.Name) : false;
			success = success ? Marshaler.Read(msg, ref obj.ID) : false;
	        return success;
	    }
	    public static bool Write(NetBuffer msg, _DIRECTION obj)
	    {
			Marshaler.Write(msg, obj.Name);
			Marshaler.Write(msg, obj.ID);
	        return msg.IsError;
	    }
	    public static bool Read(NetBuffer msg, List<_DIRECTION> list)
	    {
	        bool success = true;
	        int cnt = msg.ReadInt16();
	        for (int i = 0; i < cnt; i++)
	        {
				_DIRECTION obj = new _DIRECTION();
				success = success ? Marshaler.Read(msg, ref obj.Name) : false;
				success = success ? Marshaler.Read(msg, ref obj.ID) : false;
				list.Add(obj);
	        }
	        return success;
	    }
	    public static bool Write(NetBuffer msg, List<_DIRECTION> list)
	    {
	        msg.Write((Int16)list.Count);
	        foreach (_DIRECTION obj in list)
	        {
				Marshaler.Write(msg, obj.Name);
				Marshaler.Write(msg, obj.ID);
	        }
	        return msg.IsError;
	    }
	}
	[System.Serializable]
	public partial class _MESSAGE : IBaseObejct<MESSAGE>
	{
		public static MESSAGE Parse(string name)
		{
			return FrameworkUtil.Parse<MESSAGE>(name);
		}
		public string              Name = "";
		public MESSAGE             ID;
		public string              TEXT { get { return Table.T_LString.GetAt(_TEXT.Key); } }
		LString             _TEXT = new LString();

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
		public MESSAGE GetKey() { return ID; }
		public string GetSelectQuery(MESSAGE _key) { return string.Format("select Name, ID, TEXT from MESSAGE where ID='{0}';", _key); }
		public bool IsDefault
		{
			get
			{
				if (string.IsNullOrEmpty(Name) == false) return false;
				if ((int)ID != 0) return false;
				return true;
			}
		}
		public void Initialize(IBaseObejct from)
		{
			_MESSAGE obj = from as _MESSAGE;
			if (obj == null)
			{
				Log.Error("Cannot Initialize [name]:_MESSAGE");
				return;
			}
			Name                = obj.Name;
			ID                  = obj.ID;
			_TEXT.Initialize(obj._TEXT);
		}
		public void Initialize(PropTable obj)
		{
			Name                = obj.GetStr("Name");
			ID                  = _MESSAGE.Parse(obj.GetStr("ID"));
			_TEXT.Key = FrameworkUtil.MakeLStringKey("_MESSAGE", "TEXT", ID.ToString());
			_TEXT.Value = obj.GetStr("TEXT");
		}
		public void Initialize(JsonData obj)
		{
			if (obj.Keys.Contains("Name")) Name = obj["Name"].ToString(); else Name = default(string);
			if (obj.Keys.Contains("ID")) ID = _MESSAGE.Parse(obj["ID"].ToString()); else ID = default(MESSAGE);
			_TEXT.Key = FrameworkUtil.MakeLStringKey("_MESSAGE", "TEXT", ID.ToString());
		}
		public void Initialize(SQLite_Reader obj)
		{
			Name                = obj.GetString(0);
			ID                  = _MESSAGE.Parse(obj.GetString(1));
			_TEXT.Key = FrameworkUtil.MakeLStringKey("_MESSAGE", "TEXT", ID.ToString());
		}
		public override string ToString()
		{
		    StringBuilder sb = new StringBuilder();
		    sb.Append("{"); sb.Append(" \"Name\":"); sb.Append("\""); sb.Append(Name); sb.Append("\"");
		    sb.Append(","); sb.Append(" \"ID\":"); sb.Append("\""); sb.Append(ID.ToString()); sb.Append("\"");
		    sb.Append(","); sb.Append(" \"TEXT\":"); sb.Append("\""); sb.Append(TEXT); sb.Append("\"");
		    sb.Append("}");
		    return sb.ToString();
		}
		public string ToJson()
		{
		    StringBuilder sb = new StringBuilder();
		    sb.Append("{"); sb.Append("\"Name\":"); sb.Append("\""); sb.Append(Name); sb.Append("\"");
			if (default(MESSAGE) != ID) { sb.Append(","); sb.Append("\"ID\":"); sb.Append("\""); sb.Append(ID.ToString()); sb.Append("\""); }
		    sb.Append("}");
		    return sb.ToString();
		}
		public PropTable ToTable()
		{
			PropTable obj = new PropTable("_MESSAGE");
			obj.Attach("Name", "string", CLASS_TYPE.VALUE, KEY_TYPE.NONE, Name);
			obj.Attach("ID", "MESSAGE", CLASS_TYPE.VALUE, KEY_TYPE.MAP, ((int)ID).ToString());
			obj.Attach("TEXT", "LString", CLASS_TYPE.VALUE, KEY_TYPE.NONE, _TEXT.Value);
			return obj;
		}
	}
	public static partial class Marshaler
	{
	    public static bool Read(NetBuffer msg, _MESSAGE obj)
	    {
	        bool success = true;
			success = success ? Marshaler.Read(msg, ref obj.Name) : false;
			success = success ? Marshaler.Read(msg, ref obj.ID) : false;
	        return success;
	    }
	    public static bool Write(NetBuffer msg, _MESSAGE obj)
	    {
			Marshaler.Write(msg, obj.Name);
			Marshaler.Write(msg, obj.ID);
			Marshaler.Write(msg, obj.TEXT);
	        return msg.IsError;
	    }
	    public static bool Read(NetBuffer msg, List<_MESSAGE> list)
	    {
	        bool success = true;
	        int cnt = msg.ReadInt16();
	        for (int i = 0; i < cnt; i++)
	        {
				_MESSAGE obj = new _MESSAGE();
				success = success ? Marshaler.Read(msg, ref obj.Name) : false;
				success = success ? Marshaler.Read(msg, ref obj.ID) : false;
				list.Add(obj);
	        }
	        return success;
	    }
	    public static bool Write(NetBuffer msg, List<_MESSAGE> list)
	    {
	        msg.Write((Int16)list.Count);
	        foreach (_MESSAGE obj in list)
	        {
				Marshaler.Write(msg, obj.Name);
				Marshaler.Write(msg, obj.ID);
				Marshaler.Write(msg, obj.TEXT);
	        }
	        return msg.IsError;
	    }
	}
	[System.Serializable]
	public partial class _UNIT : IBaseObejct<UNIT>
	{
		public static UNIT Parse(string name)
		{
			return FrameworkUtil.Parse<UNIT>(name);
		}
		public string              Name = "";
		public UNIT                ID;

		public _UNIT()
		{
		}
		public _UNIT(_UNIT obj)
		{
			Initialize(obj);
		}
		public _UNIT(PropTable obj)
		{
			Initialize(obj);
		}
		public UNIT GetKey() { return ID; }
		public string GetSelectQuery(UNIT _key) { return string.Format("select Name, ID from UNIT where ID='{0}';", _key); }
		public bool IsDefault
		{
			get
			{
				if (string.IsNullOrEmpty(Name) == false) return false;
				if ((int)ID != 0) return false;
				return true;
			}
		}
		public void Initialize(IBaseObejct from)
		{
			_UNIT obj = from as _UNIT;
			if (obj == null)
			{
				Log.Error("Cannot Initialize [name]:_UNIT");
				return;
			}
			Name                = obj.Name;
			ID                  = obj.ID;
		}
		public void Initialize(PropTable obj)
		{
			Name                = obj.GetStr("Name");
			ID                  = _UNIT.Parse(obj.GetStr("ID"));
		}
		public void Initialize(JsonData obj)
		{
			if (obj.Keys.Contains("Name")) Name = obj["Name"].ToString(); else Name = default(string);
			if (obj.Keys.Contains("ID")) ID = _UNIT.Parse(obj["ID"].ToString()); else ID = default(UNIT);
		}
		public void Initialize(SQLite_Reader obj)
		{
			Name                = obj.GetString(0);
			ID                  = _UNIT.Parse(obj.GetString(1));
		}
		public override string ToString()
		{
		    StringBuilder sb = new StringBuilder();
		    sb.Append("{"); sb.Append(" \"Name\":"); sb.Append("\""); sb.Append(Name); sb.Append("\"");
		    sb.Append(","); sb.Append(" \"ID\":"); sb.Append("\""); sb.Append(ID.ToString()); sb.Append("\"");
		    sb.Append("}");
		    return sb.ToString();
		}
		public string ToJson()
		{
		    StringBuilder sb = new StringBuilder();
		    sb.Append("{"); sb.Append("\"Name\":"); sb.Append("\""); sb.Append(Name); sb.Append("\"");
			if (default(UNIT) != ID) { sb.Append(","); sb.Append("\"ID\":"); sb.Append("\""); sb.Append(ID.ToString()); sb.Append("\""); }
		    sb.Append("}");
		    return sb.ToString();
		}
		public PropTable ToTable()
		{
			PropTable obj = new PropTable("_UNIT");
			obj.Attach("Name", "string", CLASS_TYPE.VALUE, KEY_TYPE.NONE, Name);
			obj.Attach("ID", "UNIT", CLASS_TYPE.VALUE, KEY_TYPE.MAP, ((int)ID).ToString());
			return obj;
		}
	}
	public static partial class Marshaler
	{
	    public static bool Read(NetBuffer msg, _UNIT obj)
	    {
	        bool success = true;
			success = success ? Marshaler.Read(msg, ref obj.Name) : false;
			success = success ? Marshaler.Read(msg, ref obj.ID) : false;
	        return success;
	    }
	    public static bool Write(NetBuffer msg, _UNIT obj)
	    {
			Marshaler.Write(msg, obj.Name);
			Marshaler.Write(msg, obj.ID);
	        return msg.IsError;
	    }
	    public static bool Read(NetBuffer msg, List<_UNIT> list)
	    {
	        bool success = true;
	        int cnt = msg.ReadInt16();
	        for (int i = 0; i < cnt; i++)
	        {
				_UNIT obj = new _UNIT();
				success = success ? Marshaler.Read(msg, ref obj.Name) : false;
				success = success ? Marshaler.Read(msg, ref obj.ID) : false;
				list.Add(obj);
	        }
	        return success;
	    }
	    public static bool Write(NetBuffer msg, List<_UNIT> list)
	    {
	        msg.Write((Int16)list.Count);
	        foreach (_UNIT obj in list)
	        {
				Marshaler.Write(msg, obj.Name);
				Marshaler.Write(msg, obj.ID);
	        }
	        return msg.IsError;
	    }
	}
	public enum DIRECTION
	{
		STOP                = 0,
		BACK                = 1,
		FORWARD             = 2,
		L                   = 3,
		R                   = 4,
		FL                  = 5,
		FR                  = 6,
		BL                  = 7,
		BR                  = 8,
	}
	public static partial class Marshaler
	{
	    public static bool Read(NetBuffer msg, ref DIRECTION obj)
	    {
	        obj = (DIRECTION)msg.ReadInt32();
	        return !msg.IsError;
	    }
	    public static bool Write(NetBuffer msg, DIRECTION obj)
	    {
	        msg.Write((Int32)obj);
	        return !msg.IsError;
	    }
	    public static bool Read(NetBuffer msg, out DIRECTION[] obj)
	    {
	        int cnt = msg.ReadInt16();
	        obj = new DIRECTION[cnt];
	        for (int i = 0; i < cnt; i++)
	        {
	            obj[i] = (DIRECTION)msg.ReadInt32();
	        }
	        return !msg.IsError;
	    }
	    public static bool Read(NetBuffer msg, List<DIRECTION> obj)
	    {
	        int cnt = msg.ReadInt16();
	        obj = new List<DIRECTION>();
	        for (int i = 0; i < cnt; i++)
	        {
	            obj[i] = (DIRECTION)msg.ReadInt32();
	        }
	        return !msg.IsError;
	    }
	    public static bool Write(NetBuffer msg, DIRECTION[] list)
	    {
	        msg.Write((Int16)list.Length);
	        foreach (DIRECTION obj in list)
	        {
	            msg.Write((Int32)obj);
	        }
	        return !msg.IsError;
	    }
	    public static bool Write(NetBuffer msg, List<DIRECTION> list)
	    {
	        msg.Write((Int16)list.Count);
	        foreach (DIRECTION obj in list)
	        {
	            msg.Write((Int32)obj);
	        }
	        return !msg.IsError;
	    }
	}
	public enum UNIT
	{
		NONE                = 0,
		HUMAN_FEMALE        = 1,
		HUMAN_MALE          = 2,
		ELF_FEMALE          = 3,
		ELF_MALE            = 4,
		DARKELF_FEMALE      = 5,
		DARKELF_MALE        = 6,
		DWARF_FEMALE        = 7,
		DWARF_MALE          = 8,
		GOBLIN              = 9,
		IMP                 = 10,
		MUMMY               = 11,
		ORC                 = 12,
		TROLL               = 13,
		UNDEAD              = 14,
		DRAGON_BLACK        = 15,
		DRAGON_RED          = 16,
		DRAGON_UNDEAD       = 17,
	}
	public static partial class Marshaler
	{
	    public static bool Read(NetBuffer msg, ref UNIT obj)
	    {
	        obj = (UNIT)msg.ReadInt32();
	        return !msg.IsError;
	    }
	    public static bool Write(NetBuffer msg, UNIT obj)
	    {
	        msg.Write((Int32)obj);
	        return !msg.IsError;
	    }
	    public static bool Read(NetBuffer msg, out UNIT[] obj)
	    {
	        int cnt = msg.ReadInt16();
	        obj = new UNIT[cnt];
	        for (int i = 0; i < cnt; i++)
	        {
	            obj[i] = (UNIT)msg.ReadInt32();
	        }
	        return !msg.IsError;
	    }
	    public static bool Read(NetBuffer msg, List<UNIT> obj)
	    {
	        int cnt = msg.ReadInt16();
	        obj = new List<UNIT>();
	        for (int i = 0; i < cnt; i++)
	        {
	            obj[i] = (UNIT)msg.ReadInt32();
	        }
	        return !msg.IsError;
	    }
	    public static bool Write(NetBuffer msg, UNIT[] list)
	    {
	        msg.Write((Int16)list.Length);
	        foreach (UNIT obj in list)
	        {
	            msg.Write((Int32)obj);
	        }
	        return !msg.IsError;
	    }
	    public static bool Write(NetBuffer msg, List<UNIT> list)
	    {
	        msg.Write((Int16)list.Count);
	        foreach (UNIT obj in list)
	        {
	            msg.Write((Int32)obj);
	        }
	        return !msg.IsError;
	    }
	}
	public enum MESSAGE
	{
		SUCCESS             = 0,
		ERROR_UNKNOWN       = 1,
	}
	public static partial class Marshaler
	{
	    public static bool Read(NetBuffer msg, ref MESSAGE obj)
	    {
	        obj = (MESSAGE)msg.ReadInt32();
	        return !msg.IsError;
	    }
	    public static bool Write(NetBuffer msg, MESSAGE obj)
	    {
	        msg.Write((Int32)obj);
	        return !msg.IsError;
	    }
	    public static bool Read(NetBuffer msg, out MESSAGE[] obj)
	    {
	        int cnt = msg.ReadInt16();
	        obj = new MESSAGE[cnt];
	        for (int i = 0; i < cnt; i++)
	        {
	            obj[i] = (MESSAGE)msg.ReadInt32();
	        }
	        return !msg.IsError;
	    }
	    public static bool Read(NetBuffer msg, List<MESSAGE> obj)
	    {
	        int cnt = msg.ReadInt16();
	        obj = new List<MESSAGE>();
	        for (int i = 0; i < cnt; i++)
	        {
	            obj[i] = (MESSAGE)msg.ReadInt32();
	        }
	        return !msg.IsError;
	    }
	    public static bool Write(NetBuffer msg, MESSAGE[] list)
	    {
	        msg.Write((Int16)list.Length);
	        foreach (MESSAGE obj in list)
	        {
	            msg.Write((Int32)obj);
	        }
	        return !msg.IsError;
	    }
	    public static bool Write(NetBuffer msg, List<MESSAGE> list)
	    {
	        msg.Write((Int16)list.Count);
	        foreach (MESSAGE obj in list)
	        {
	            msg.Write((Int32)obj);
	        }
	        return !msg.IsError;
	    }
	}
} // end of namespace
