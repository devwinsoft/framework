using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using LitJson;
namespace Devarc
{
	[System.Serializable]
	public partial class SOUND : IBaseObejct<string>
	{
		public string              SEQ = "";
		public string              SOUND_ID = "";
		public float               VOLUME;
		public string              PATH = "";
		public bool                LOOP;

		public SOUND()
		{
		}
		public SOUND(SOUND obj)
		{
			Initialize(obj);
		}
		public SOUND(PropTable obj)
		{
			Initialize(obj);
		}
		public string GetKey() { return SEQ; }
		public string GetQuery_Select(string _key)
		{
			return string.Format("select * from `SOUND` where SEQ='{0}';", _key);
		}
		public string GetQuery_SelectWhere(string _where)
		{
			return string.Format("select * from `SOUND` where {0};", _where);
		}
		public string GetQuery_InsertOrUpdate()
		{
			PropTable obj = ToTable();
			return string.Format("insert into SOUND (`SEQ`, `SOUND_ID`, `VOLUME`, `PATH`, `LOOP`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}') on duplicate key update `SEQ`='{0}', `SOUND_ID`='{1}', `VOLUME`='{2}', `PATH`='{3}', `LOOP`='{4}';", FrameworkUtil.InnerString(obj.GetStr("SEQ")), FrameworkUtil.InnerString(obj.GetStr("SOUND_ID")), obj.GetStr("VOLUME"), FrameworkUtil.InnerString(obj.GetStr("PATH")), obj.GetStr("LOOP"));
		}
		public bool IsDefault
		{
			get
			{
				if (string.IsNullOrEmpty(SEQ) == false) return false;
				if (string.IsNullOrEmpty(SOUND_ID) == false) return false;
				if (VOLUME != 0) return false;
				if (string.IsNullOrEmpty(PATH) == false) return false;
				if (LOOP) return false;
				return true;
			}
		}
		public void Initialize(IBaseObejct from)
		{
			SOUND obj = from as SOUND;
			if (obj == null)
			{
				Log.Error("Cannot Initialize [name]:SOUND");
				return;
			}
			SEQ                 = obj.SEQ;
			SOUND_ID            = obj.SOUND_ID;
			VOLUME              = obj.VOLUME;
			PATH                = obj.PATH;
			LOOP                = obj.LOOP;
		}
		public void Initialize(PropTable obj)
		{
			SEQ                 = obj.GetStr("SEQ");
			SOUND_ID            = obj.GetStr("SOUND_ID");
			VOLUME              = obj.GetFloat("VOLUME");
			PATH                = obj.GetStr("PATH");
			LOOP                = obj.GetBool("LOOP");
		}
		public void Initialize(JsonData obj)
		{
			if (obj.Keys.Contains("SEQ")) SEQ = obj["SEQ"].ToString(); else SEQ = default(string);
			if (obj.Keys.Contains("SOUND_ID")) SOUND_ID = obj["SOUND_ID"].ToString(); else SOUND_ID = default(string);
			if (obj.Keys.Contains("VOLUME")) float.TryParse(obj["VOLUME"].ToString(), out VOLUME); else VOLUME = default(float);
			if (obj.Keys.Contains("PATH")) PATH = obj["PATH"].ToString(); else PATH = default(string);
			if (obj.Keys.Contains("LOOP")) bool.TryParse(obj["LOOP"].ToString(), out LOOP); else LOOP = default(bool);
		}
		public void Initialize(IBaseReader obj)
		{
			SEQ                 = obj.GetString("SEQ");
			SOUND_ID            = obj.GetString("SOUND_ID");
			VOLUME              = obj.GetFloat("VOLUME");
			PATH                = obj.GetString("PATH");
			LOOP                = obj.GetBoolean("LOOP");
		}
		public override string ToString()
		{
		    StringBuilder sb = new StringBuilder();
		    sb.Append("{"); sb.Append(" \"SEQ\":"); sb.Append("\""); sb.Append(SEQ); sb.Append("\"");
		    sb.Append(","); sb.Append(" \"SOUND_ID\":"); sb.Append("\""); sb.Append(SOUND_ID); sb.Append("\"");
		    sb.Append(","); sb.Append(" \"VOLUME\":"); sb.Append("\""); sb.Append(VOLUME.ToString()); sb.Append("\"");
		    sb.Append(","); sb.Append(" \"PATH\":"); sb.Append("\""); sb.Append(PATH); sb.Append("\"");
		    sb.Append(","); sb.Append(" \"LOOP\":"); sb.Append("\""); sb.Append(LOOP.ToString()); sb.Append("\"");
		    sb.Append("}");
		    return sb.ToString();
		}
		public string ToJson()
		{
		    if (IsDefault) { return "{}"; }
		    StringBuilder sb = new StringBuilder();
		    int j = 0;
			sb.Append("{");
			if (string.IsNullOrEmpty(SEQ) == false) { if (j > 0) { sb.Append(", "); } j++;
			 sb.Append("\"SEQ\":"); sb.Append("\""); sb.Append(FrameworkUtil.JsonString(SEQ)); sb.Append("\""); }
			if (string.IsNullOrEmpty(SOUND_ID) == false) { if (j > 0) { sb.Append(", "); } j++;
			 sb.Append("\"SOUND_ID\":"); sb.Append("\""); sb.Append(FrameworkUtil.JsonString(SOUND_ID)); sb.Append("\""); }
			if (default(float) != VOLUME) { if (j > 0) { sb.Append(", "); } j++;
			 sb.Append("\"VOLUME\":"); sb.Append(string.Format("\"{0}\"", VOLUME.ToString())); }
			if (string.IsNullOrEmpty(PATH) == false) { if (j > 0) { sb.Append(", "); } j++;
			 sb.Append("\"PATH\":"); sb.Append("\""); sb.Append(FrameworkUtil.JsonString(PATH)); sb.Append("\""); }
			if (default(bool) != LOOP) { if (j > 0) { sb.Append(", "); } j++;
			 sb.Append("\"LOOP\":"); sb.Append(string.Format("\"{0}\"", LOOP.ToString())); }
		    sb.Append("}");
		    return sb.ToString();
		}
		public PropTable ToTable()
		{
			PropTable obj = new PropTable("SOUND");
			obj.Attach("SEQ", "string", CLASS_TYPE.VALUE, true, SEQ);
			obj.Attach("SOUND_ID", "string", CLASS_TYPE.VALUE, false, SOUND_ID);
			obj.Attach("VOLUME", "float", CLASS_TYPE.VALUE, false, VOLUME.ToString());
			obj.Attach("PATH", "string", CLASS_TYPE.VALUE, false, PATH);
			obj.Attach("LOOP", "bool", CLASS_TYPE.VALUE, false, LOOP.ToString());
			return obj;
		}
	}
	public static partial class Marshaler
	{
	    public static void Read(NetBuffer msg, SOUND obj)
	    {
			Marshaler.Read(msg, ref obj.SEQ);
			Marshaler.Read(msg, ref obj.SOUND_ID);
			Marshaler.Read(msg, ref obj.VOLUME);
			Marshaler.Read(msg, ref obj.PATH);
			Marshaler.Read(msg, ref obj.LOOP);
	    }
	    public static bool Write(NetBuffer msg, SOUND obj)
	    {
			Marshaler.Write(msg, obj.SEQ);
			Marshaler.Write(msg, obj.SOUND_ID);
			Marshaler.Write(msg, obj.VOLUME);
			Marshaler.Write(msg, obj.PATH);
			Marshaler.Write(msg, obj.LOOP);
	        return msg.IsError;
	    }
	    public static void Read(NetBuffer msg, List<SOUND> list)
	    {
	        int cnt = msg.ReadInt16();
	        for (int i = 0; i < cnt; i++)
	        {
				SOUND obj = new SOUND();
				Marshaler.Read(msg, ref obj.SEQ);
				Marshaler.Read(msg, ref obj.SOUND_ID);
				Marshaler.Read(msg, ref obj.VOLUME);
				Marshaler.Read(msg, ref obj.PATH);
				Marshaler.Read(msg, ref obj.LOOP);
				list.Add(obj);
	        }
	    }
	    public static bool Write(NetBuffer msg, List<SOUND> list)
	    {
	        msg.Write((Int16)list.Count);
	        foreach (SOUND obj in list)
	        {
				Marshaler.Write(msg, obj.SEQ);
				Marshaler.Write(msg, obj.SOUND_ID);
				Marshaler.Write(msg, obj.VOLUME);
				Marshaler.Write(msg, obj.PATH);
				Marshaler.Write(msg, obj.LOOP);
	        }
	        return msg.IsError;
	    }
	}
} // end of namespace
