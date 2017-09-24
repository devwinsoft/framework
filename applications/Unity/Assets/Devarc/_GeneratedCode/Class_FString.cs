using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Data;
#if UNITY_5
using Mono.Data.Sqlite;
#else
using System.Data.SQLite;
using SqliteDataReader = System.Data.SQLite.SQLiteDataReader;
using SqliteConnection = System.Data.SQLite.SQLiteConnection;
using SqliteCommand = System.Data.SQLite.SQLiteCommand;
#endif
using LitJson;
namespace Devarc
{
	[System.Serializable]
	public partial class FString : IBaseObejct<string>
	{
		public string              Key = "";
		public string              Value = "";

		public FString()
		{
		}
		public FString(FString obj)
		{
			Initialize(obj);
		}
		public FString(PropTable obj)
		{
			Initialize(obj);
		}
		public string GetSelectQuery(string _key) { return string.Format("select Key, Value from Key where Key='{0}';", _key); }
		public bool IsDefault
		{
			get
			{
				if (string.IsNullOrEmpty(Key) == false) return false;
				if (string.IsNullOrEmpty(Value) == false) return false;
				return true;
			}
		}
		public void Initialize(IBaseObejct from)
		{
			FString obj = from as FString;
			if (obj == null)
			{
				Log.Error("Cannot Initialize [name]:FString");
				return;
			}
			Key                 = obj.Key;
			Value               = obj.Value;
		}
		public void Initialize(PropTable obj)
		{
			Key                 = obj.GetStr("Key");
			Value               = obj.GetStr("Value");
		}
		public void Initialize(JsonData obj)
		{
			if (obj.Keys.Contains("Key")) Key = obj["Key"].ToString(); else Key = default(string);
			if (obj.Keys.Contains("Value")) Value = obj["Value"].ToString(); else Value = default(string);
		}
		public void Initialize(SqliteDataReader obj)
		{
			Key                 = obj.GetString(0);
			Value               = obj.GetString(1);
		}
		public override string ToString()
		{
		    StringBuilder sb = new StringBuilder();
		    sb.Append("{"); sb.Append(" \"Key\":"); sb.Append("\""); sb.Append(Key); sb.Append("\"");
		    sb.Append(","); sb.Append(" \"Value\":"); sb.Append("\""); sb.Append(Value); sb.Append("\"");
		    sb.Append("}");
		    return sb.ToString();
		}
		public string ToJson()
		{
		    if (IsDefault) { return "{}"; }
		    StringBuilder sb = new StringBuilder();
		    sb.Append("{"); sb.Append("\"Key\":"); sb.Append("\""); sb.Append(Key); sb.Append("\"");
			if (string.IsNullOrEmpty(Value) == false) { sb.Append(","); sb.Append("\"Value\":"); sb.Append("\""); sb.Append(Value); sb.Append("\""); }
		    sb.Append("}");
		    return sb.ToString();
		}
		public PropTable ToTable()
		{
			PropTable obj = new PropTable("FString");
			obj.Attach("Key", "string", CLASS_TYPE.VALUE, KEY_TYPE.MAP, Key);
			obj.Attach("Value", "string", CLASS_TYPE.VALUE, KEY_TYPE.NONE, Value);
			return obj;
		}
	}
	public static partial class Marshaler
	{
	    public static bool Read(NetBuffer msg, FString obj)
	    {
	        bool success = true;
			success = success ? Marshaler.Read(msg, ref obj.Key) : false;
			success = success ? Marshaler.Read(msg, ref obj.Value) : false;
	        return success;
	    }
	    public static void Write(NetBuffer msg, FString obj)
	    {
			Marshaler.Write(msg, obj.Key);
			Marshaler.Write(msg, obj.Value);
	    }
	    public static bool Read(NetBuffer msg, List<FString> list)
	    {
	        bool success = true;
	        int cnt = msg.ReadInt16();
	        for (int i = 0; i < cnt; i++)
	        {
				FString obj = new FString();
				success = success ? Marshaler.Read(msg, ref obj.Key) : false;
				success = success ? Marshaler.Read(msg, ref obj.Value) : false;
				list.Add(obj);
	        }
	        return success;
	    }
	    public static void Write(NetBuffer msg, List<FString> list)
	    {
	        msg.Write((Int16)list.Count);
	        foreach (FString obj in list)
	        {
				Marshaler.Write(msg, obj.Key);
				Marshaler.Write(msg, obj.Value);
	        }
	    }
	}
} // end of namespace
