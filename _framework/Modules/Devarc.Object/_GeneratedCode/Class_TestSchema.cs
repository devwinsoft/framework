using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using LitJson;
namespace Devarc
{
	public class DataCharacter : IBaseObejct
	{
		public UNIT                unit_type;
		public string              name = "";
		public List<UNIT> items = new List<UNIT>();
		public List<DataAbility> stats = new List<DataAbility>();
		private DataAbility        _ability = new DataAbility();
		public DataAbility         ability { get { return _ability; } set { _ability.Initialize(value); } }
		public List<string> nodes = new List<string>();

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
		public void Initialize(IBaseObejct from)
		{
			DataCharacter obj = from as DataCharacter;
			if (obj == null)
			{
				Log.Error("Cannot Initialize [name]:DataCharacter");
				return;
			}
			unit_type           = obj.unit_type;
			name                = obj.name;
			items.Clear();
			items.AddRange(obj.items);
			stats.Clear();
			foreach(DataAbility _obj in stats) { DataAbility _new = new DataAbility(_obj); stats.Add(_new); }
			ability.Initialize(obj.ability);
			nodes.Clear();
			nodes.AddRange(obj.nodes);
		}
		public void Initialize(PropTable obj)
		{
			unit_type           = _UNIT.Parse(obj.GetStr("unit_type"));
			name                = obj.GetStr("name");
			items.Clear();
			JsonData __items = JsonMapper.ToObject(obj.GetStr("items"));
			if (__items != null && __items.IsArray) { foreach (var node in __items as IList) { items.Add(_UNIT.Parse(node.ToString())); } }
			stats.Clear();
			JsonData __stats = JsonMapper.ToObject(obj.GetStr("stats"));
			if (__stats != null && __stats.IsArray) { foreach (var node in __stats as IList) { DataAbility _v = new DataAbility(); _v.Initialize(node as JsonData); stats.Add(_v); } }
			_ability.Initialize(obj.GetTable("ability"));
			obj.GetList<string>("nodes", nodes);
		}
		public void Initialize(JsonData obj)
		{
			if (obj.Keys.Contains("unit_type")) unit_type = _UNIT.Parse(obj["unit_type"].ToString()); else unit_type = default(UNIT);
			if (obj.Keys.Contains("name")) name = obj["name"].ToString(); else name = default(string);
			if (obj.Keys.Contains("items")) foreach (JsonData node in obj["items"]) { items.Add(_UNIT.Parse(node.ToString())); }
			if (obj.Keys.Contains("stats")) foreach (JsonData node in obj["stats"]) { DataAbility _v = new DataAbility(); _v.Initialize(node); stats.Add(_v); }
			if (obj.Keys.Contains("ability")) _ability.Initialize(obj["ability"]);
			if (obj.Keys.Contains("nodes")) foreach (JsonData node in obj["nodes"]) { nodes.Add(DES.Decrypt(node.ToString())); }
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
		    sb.Append("}");
		    return sb.ToString();
		}
		public string ToJson()
		{
		    StringBuilder sb = new StringBuilder();
		    sb.Append("{"); sb.Append("\"unit_type\":"); sb.Append("\""); sb.Append(unit_type.ToString()); sb.Append("\"");
			if (string.IsNullOrEmpty(name) == false) { sb.Append(","); sb.Append("\"name\":"); sb.Append("\""); sb.Append(name); sb.Append("\""); }
			if (items.Count > 0) { sb.Append(","); sb.Append("\"items\":"); sb.Append("["); for (int i = 0; i < items.Count; i++) { UNIT _obj = items[i]; if (i > 0) sb.Append(","); sb.Append(string.Format("\"{0}\"", _obj)); } sb.Append("]"); }
			if (stats.Count > 0) { sb.Append(","); sb.Append("\"stats\":"); sb.Append("["); for (int i = 0; i < stats.Count; i++) { sb.Append(stats[i].ToJson()); } sb.Append("]"); }
		    sb.Append(","); sb.Append("\"ability\":"); sb.Append(ability != null ? ability.ToJson() : "{}");
			if (nodes.Count > 0) { sb.Append(","); sb.Append("\"nodes\":"); sb.Append("["); for (int i = 0; i < nodes.Count; i++) { string _obj = nodes[i]; if (i > 0) sb.Append(","); sb.Append("\""); sb.Append(DES.Encrypt(_obj)); sb.Append("\""); } sb.Append("]"); }
		    sb.Append("}");
		    return sb.ToString();
		}
		public PropTable ToTable()
		{
			PropTable obj = new PropTable("DataCharacter");
			obj.Attach("unit_type", "UNIT", CLASS_TYPE.VALUE, true, unit_type.ToString());
			obj.Attach("name", "string", CLASS_TYPE.VALUE, false, name);
			obj.Attach_List<UNIT>("items", "UNIT", VAR_TYPE.ENUM, items);
			obj.Attach_List<DataAbility>("stats", "DataAbility", VAR_TYPE.CLASS, stats);
			obj.Attach_Class("ability", "DataAbility", ability.ToTable());
			obj.Attach_List<string>("nodes", "string", VAR_TYPE.CSTRING, nodes);
			return obj;
		}
	}
	public static partial class Marshaler
	{
	    public static bool Read(NetBuffer msg, DataCharacter obj)
	    {
	        bool success = true;
			success = success ? Marshaler.Read(msg, ref obj.unit_type) : false;
			success = success ? Marshaler.Read(msg, ref obj.name) : false;
			success = success ? Marshaler.Read(msg, obj.items) : false;
			success = success ? Marshaler.Read(msg, obj.stats) : false;
			success = success ? Marshaler.Read(msg, obj.ability) : false;
	        return success;
	    }
	    public static void Write(NetBuffer msg, DataCharacter obj)
	    {
			Marshaler.Write(msg, obj.unit_type);
			Marshaler.Write(msg, obj.name);
			Marshaler.Write(msg, obj.items);
			Marshaler.Write(msg, obj.stats);
			Marshaler.Write(msg, obj.ability);
			Marshaler.Write(msg, obj.nodes);
	    }
	    public static bool Read(NetBuffer msg, List<DataCharacter> list)
	    {
	        bool success = true;
	        int cnt = msg.ReadInt16();
	        for (int i = 0; i < cnt; i++)
	        {
				DataCharacter obj = new DataCharacter();
				success = success ? Marshaler.Read(msg, ref obj.unit_type) : false;
				success = success ? Marshaler.Read(msg, ref obj.name) : false;
				success = success ? Marshaler.Read(msg, obj.items) : false;
				success = success ? Marshaler.Read(msg, obj.stats) : false;
				success = success ? Marshaler.Read(msg, obj.ability) : false;
				list.Add(obj);
	        }
	        return success;
	    }
	    public static void Write(NetBuffer msg, List<DataCharacter> list)
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
	        }
	    }
	}
	public class T_DataCharacter : DataCharacter, IContents<UNIT>, IDisposable
	{
	    public static Container_C1<T_DataCharacter, UNIT> LIST = new Container_C1<T_DataCharacter, UNIT>();
	    public UNIT GetKey1()
	    {
	        return base.unit_type;
	    }
	    public void OnAlloc(UNIT key)
	    {
	        base.unit_type = key;
	    }
	    public void OnFree()
	    {
	    }
	    public void Dispose()
	    {
	    }
	}
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
			obj.Attach("str", "int", CLASS_TYPE.VALUE, false, str.ToString());
			obj.Attach("dex", "int", CLASS_TYPE.VALUE, false, dex.ToString());
			obj.Attach("vit", "int", CLASS_TYPE.VALUE, false, vit.ToString());
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
	    public static void Write(NetBuffer msg, DataAbility obj)
	    {
			Marshaler.Write(msg, obj.str);
			Marshaler.Write(msg, obj.dex);
			Marshaler.Write(msg, obj.vit);
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
	    public static void Write(NetBuffer msg, List<DataAbility> list)
	    {
	        msg.Write((Int16)list.Count);
	        foreach (DataAbility obj in list)
	        {
				Marshaler.Write(msg, obj.str);
				Marshaler.Write(msg, obj.dex);
				Marshaler.Write(msg, obj.vit);
	        }
	    }
	}
	public class DataPlayer : IBaseObejct
	{
		public HostID              id;
		private VECTOR3            _pos = new VECTOR3();
		public VECTOR3             pos { get { return _pos; } set { _pos.Initialize(value); } }

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
			_pos.Initialize(obj.GetTable("pos"));
		}
		public void Initialize(JsonData obj)
		{
			if (obj.Keys.Contains("id")) HostID.TryParse(obj["id"].ToString(), out id); else id = default(short);
			if (obj.Keys.Contains("pos")) _pos.Initialize(obj["pos"]);
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
		    StringBuilder sb = new StringBuilder();
		    sb.Append("{"); sb.Append("\"id\":"); sb.Append("\""); sb.Append(id.ToString()); sb.Append("\"");
		    sb.Append(","); sb.Append("\"pos\":"); sb.Append(pos != null ? pos.ToJson() : "{}");
		    sb.Append("}");
		    return sb.ToString();
		}
		public PropTable ToTable()
		{
			PropTable obj = new PropTable("DataPlayer");
			obj.Attach("id", "HostID", CLASS_TYPE.VALUE, false, id.ToString());
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
	    public static void Write(NetBuffer msg, DataPlayer obj)
	    {
			Marshaler.Write(msg, obj.id);
			Marshaler.Write(msg, obj.pos);
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
	    public static void Write(NetBuffer msg, List<DataPlayer> list)
	    {
	        msg.Write((Int16)list.Count);
	        foreach (DataPlayer obj in list)
	        {
				Marshaler.Write(msg, obj.id);
				Marshaler.Write(msg, obj.pos);
	        }
	    }
	}
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
			obj.Attach("x", "float", CLASS_TYPE.VALUE, false, x.ToString());
			obj.Attach("y", "float", CLASS_TYPE.VALUE, false, y.ToString());
			obj.Attach("z", "float", CLASS_TYPE.VALUE, false, z.ToString());
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
	    public static void Write(NetBuffer msg, VECTOR3 obj)
	    {
			Marshaler.Write(msg, obj.x);
			Marshaler.Write(msg, obj.y);
			Marshaler.Write(msg, obj.z);
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
	    public static void Write(NetBuffer msg, List<VECTOR3> list)
	    {
	        msg.Write((Int16)list.Count);
	        foreach (VECTOR3 obj in list)
	        {
				Marshaler.Write(msg, obj.x);
				Marshaler.Write(msg, obj.y);
				Marshaler.Write(msg, obj.z);
	        }
	    }
	}
	public class _UNIT : IBaseObejct
	{
		public static UNIT Parse(string name)
		{
			int result;
			if (Int32.TryParse(name, out result))
				return (UNIT)result;
			if (name == "NONE")
				return UNIT.NONE;
			if (name == "HUMAN_FEMALE")
				return UNIT.HUMAN_FEMALE;
			if (name == "HUMAN_MALE")
				return UNIT.HUMAN_MALE;
			if (name == "ELF_FEMALE")
				return UNIT.ELF_FEMALE;
			if (name == "ELF_MALE")
				return UNIT.ELF_MALE;
			if (name == "DARKELF_FEMALE")
				return UNIT.DARKELF_FEMALE;
			if (name == "DARKELF_MALE")
				return UNIT.DARKELF_MALE;
			if (name == "DWARF_FEMALE")
				return UNIT.DWARF_FEMALE;
			if (name == "DWARF_MALE")
				return UNIT.DWARF_MALE;
			if (name == "GOBLIN")
				return UNIT.GOBLIN;
			if (name == "IMP")
				return UNIT.IMP;
			if (name == "MUMMY")
				return UNIT.MUMMY;
			if (name == "ORC")
				return UNIT.ORC;
			if (name == "TROLL")
				return UNIT.TROLL;
			if (name == "UNDEAD")
				return UNIT.UNDEAD;
			if (name == "DRAGON_BLACK")
				return UNIT.DRAGON_BLACK;
			if (name == "DRAGON_RED")
				return UNIT.DRAGON_RED;
			if (name == "DRAGON_UNDEAD")
				return UNIT.DRAGON_UNDEAD;
			return (UNIT)0;
		}
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
		public void Initialize(IBaseObejct from)
		{
			_UNIT obj = from as _UNIT;
			if (obj == null)
			{
				Log.Error("Cannot Initialize [name]:_UNIT");
				return;
			}
			ID                  = obj.ID;
		}
		public void Initialize(PropTable obj)
		{
			ID                  = _UNIT.Parse(obj.GetStr("ID"));
		}
		public void Initialize(JsonData obj)
		{
			if (obj.Keys.Contains("ID")) ID = _UNIT.Parse(obj["ID"].ToString()); else ID = default(UNIT);
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
			PropTable obj = new PropTable("_UNIT");
			obj.Attach("Name", "", CLASS_TYPE.VALUE, false, ID.ToString());
			obj.Attach("ID", "UNIT", CLASS_TYPE.VALUE, true, ((int)ID).ToString());
			return obj;
		}
	}
	public static partial class Marshaler
	{
	    public static bool Read(NetBuffer msg, _UNIT obj)
	    {
	        bool success = true;
			success = success ? Marshaler.Read(msg, ref obj.ID) : false;
	        return success;
	    }
	    public static void Write(NetBuffer msg, _UNIT obj)
	    {
			Marshaler.Write(msg, obj.ID);
	    }
	    public static bool Read(NetBuffer msg, List<_UNIT> list)
	    {
	        bool success = true;
	        int cnt = msg.ReadInt16();
	        for (int i = 0; i < cnt; i++)
	        {
				_UNIT obj = new _UNIT();
				success = success ? Marshaler.Read(msg, ref obj.ID) : false;
				list.Add(obj);
	        }
	        return success;
	    }
	    public static void Write(NetBuffer msg, List<_UNIT> list)
	    {
	        msg.Write((Int16)list.Count);
	        foreach (_UNIT obj in list)
	        {
				Marshaler.Write(msg, obj.ID);
	        }
	    }
	}
	public class T_UNIT : _UNIT, IContents<UNIT>, IDisposable
	{
	    public static Container_C1<T_UNIT, UNIT> LIST = new Container_C1<T_UNIT, UNIT>();
	    public UNIT GetKey1()
	    {
	        return base.ID;
	    }
	    public void OnAlloc(UNIT key)
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
	public class _DIRECTION : IBaseObejct
	{
		public static DIRECTION Parse(string name)
		{
			int result;
			if (Int32.TryParse(name, out result))
				return (DIRECTION)result;
			if (name == "STOP")
				return DIRECTION.STOP;
			if (name == "BACK")
				return DIRECTION.BACK;
			if (name == "FORWARD")
				return DIRECTION.FORWARD;
			if (name == "L")
				return DIRECTION.L;
			if (name == "R")
				return DIRECTION.R;
			if (name == "FL")
				return DIRECTION.FL;
			if (name == "FR")
				return DIRECTION.FR;
			if (name == "BL")
				return DIRECTION.BL;
			if (name == "BR")
				return DIRECTION.BR;
			return (DIRECTION)0;
		}
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
		public void Initialize(IBaseObejct from)
		{
			_DIRECTION obj = from as _DIRECTION;
			if (obj == null)
			{
				Log.Error("Cannot Initialize [name]:_DIRECTION");
				return;
			}
			ID                  = obj.ID;
		}
		public void Initialize(PropTable obj)
		{
			ID                  = _DIRECTION.Parse(obj.GetStr("ID"));
		}
		public void Initialize(JsonData obj)
		{
			if (obj.Keys.Contains("ID")) ID = _DIRECTION.Parse(obj["ID"].ToString()); else ID = default(DIRECTION);
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
			PropTable obj = new PropTable("_DIRECTION");
			obj.Attach("Name", "", CLASS_TYPE.VALUE, false, ID.ToString());
			obj.Attach("ID", "DIRECTION", CLASS_TYPE.VALUE, true, ((int)ID).ToString());
			return obj;
		}
	}
	public static partial class Marshaler
	{
	    public static bool Read(NetBuffer msg, _DIRECTION obj)
	    {
	        bool success = true;
			success = success ? Marshaler.Read(msg, ref obj.ID) : false;
	        return success;
	    }
	    public static void Write(NetBuffer msg, _DIRECTION obj)
	    {
			Marshaler.Write(msg, obj.ID);
	    }
	    public static bool Read(NetBuffer msg, List<_DIRECTION> list)
	    {
	        bool success = true;
	        int cnt = msg.ReadInt16();
	        for (int i = 0; i < cnt; i++)
	        {
				_DIRECTION obj = new _DIRECTION();
				success = success ? Marshaler.Read(msg, ref obj.ID) : false;
				list.Add(obj);
	        }
	        return success;
	    }
	    public static void Write(NetBuffer msg, List<_DIRECTION> list)
	    {
	        msg.Write((Int16)list.Count);
	        foreach (_DIRECTION obj in list)
	        {
				Marshaler.Write(msg, obj.ID);
	        }
	    }
	}
	public class T_DIRECTION : _DIRECTION, IContents<DIRECTION>, IDisposable
	{
	    public static Container_C1<T_DIRECTION, DIRECTION> LIST = new Container_C1<T_DIRECTION, DIRECTION>();
	    public DIRECTION GetKey1()
	    {
	        return base.ID;
	    }
	    public void OnAlloc(DIRECTION key)
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
	        try
	        {
	            obj = (DIRECTION)msg.ReadInt32();
	            return true;
	        }
	        catch (System.Exception)
	        {
	            return false;
	        }
	    }
	    public static void Write(NetBuffer msg, DIRECTION obj)
	    {
	        msg.Write((Int32)obj);
	    }
	    public static bool Read(NetBuffer msg, out DIRECTION[] obj)
	    {
	        try
	        {
	            int cnt = msg.ReadInt16();
	            obj = new DIRECTION[cnt];
	            for (int i = 0; i < cnt; i++)
	            {
	                obj[i] = (DIRECTION)msg.ReadInt32();
	            }
	            return true;
	        }
	        catch (System.Exception)
	        {
	            obj = null;;
	            return false;
	        }
	    }
	    public static bool Read(NetBuffer msg, List<DIRECTION> obj)
	    {
	        try
	        {
	            int cnt = msg.ReadInt16();
	            obj = new List<DIRECTION>();
	            for (int i = 0; i < cnt; i++)
	            {
	                obj[i] = (DIRECTION)msg.ReadInt32();
	            }
	            return true;
	        }
	        catch (System.Exception)
	        {
	            obj = null;;
	            return false;
	        }
	    }
	    public static void Write(NetBuffer msg, DIRECTION[] list)
	    {
	        msg.Write((Int16)list.Length);
	        foreach (DIRECTION obj in list)
	        {
	            msg.Write((Int32)obj);
	        }
	    }
	    public static void Write(NetBuffer msg, List<DIRECTION> list)
	    {
	        msg.Write((Int16)list.Count);
	        foreach (DIRECTION obj in list)
	        {
	            msg.Write((Int32)obj);
	        }
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
	        try
	        {
	            obj = (UNIT)msg.ReadInt32();
	            return true;
	        }
	        catch (System.Exception)
	        {
	            return false;
	        }
	    }
	    public static void Write(NetBuffer msg, UNIT obj)
	    {
	        msg.Write((Int32)obj);
	    }
	    public static bool Read(NetBuffer msg, out UNIT[] obj)
	    {
	        try
	        {
	            int cnt = msg.ReadInt16();
	            obj = new UNIT[cnt];
	            for (int i = 0; i < cnt; i++)
	            {
	                obj[i] = (UNIT)msg.ReadInt32();
	            }
	            return true;
	        }
	        catch (System.Exception)
	        {
	            obj = null;;
	            return false;
	        }
	    }
	    public static bool Read(NetBuffer msg, List<UNIT> obj)
	    {
	        try
	        {
	            int cnt = msg.ReadInt16();
	            obj = new List<UNIT>();
	            for (int i = 0; i < cnt; i++)
	            {
	                obj[i] = (UNIT)msg.ReadInt32();
	            }
	            return true;
	        }
	        catch (System.Exception)
	        {
	            obj = null;;
	            return false;
	        }
	    }
	    public static void Write(NetBuffer msg, UNIT[] list)
	    {
	        msg.Write((Int16)list.Length);
	        foreach (UNIT obj in list)
	        {
	            msg.Write((Int32)obj);
	        }
	    }
	    public static void Write(NetBuffer msg, List<UNIT> list)
	    {
	        msg.Write((Int16)list.Count);
	        foreach (UNIT obj in list)
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
