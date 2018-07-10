using System;
using System.IO;
using LitJson;
namespace Devarc
{
	public partial class Table
	{
		static void Callback_DataCharacter_Sheet(string sheet_name, PropTable tb)
		{
			DataCharacter obj = Table.T_DataCharacter.Alloc(FrameworkUtil.Parse<UNIT>(tb.GetStr("unit_type")));
			if (obj == null)
			{
				Log.Error("[Table]Cannot create 'DataCharacter'. (id={0})", tb.GetStr("unit_type"));
				return;
			}
			obj.Initialize(tb);
		}
		static void Callback_DataCharacter_JSON(string sheet_name, JsonData node)
		{
			if (node.Keys.Contains("unit_type") == false) return;
			DataCharacter obj = Table.T_DataCharacter.Alloc(FrameworkUtil.Parse<UNIT>(node["unit_type"].ToString()));
			if (obj == null)
			{
				Log.Error("[Table]Cannot create 'DataCharacter'. (id={0})", FrameworkUtil.Parse<UNIT>(node["unit_type"].ToString()));
				return;
			}
			obj.Initialize(node);
		}
		static void Callback_DIRECTION_Sheet(string sheet_name, PropTable tb)
		{
			_DIRECTION obj = Table.T_DIRECTION.Alloc(FrameworkUtil.Parse<DIRECTION>(tb.GetStr("ID")));
			if (obj == null)
			{
				Log.Error("[Table]Cannot create 'DIRECTION'. (id={0})", tb.GetStr("ID"));
				return;
			}
			obj.Initialize(tb);
		}
		static void Callback_DIRECTION_JSON(string sheet_name, JsonData node)
		{
			if (node.Keys.Contains("ID") == false) return;
			_DIRECTION obj = Table.T_DIRECTION.Alloc(FrameworkUtil.Parse<DIRECTION>(node["ID"].ToString()));
			if (obj == null)
			{
				Log.Error("[Table]Cannot create 'DIRECTION'. (id={0})", FrameworkUtil.Parse<DIRECTION>(node["ID"].ToString()));
				return;
			}
			obj.Initialize(node);
		}
		static void Callback_MESSAGE_Sheet(string sheet_name, PropTable tb)
		{
			_MESSAGE obj = Table.T_MESSAGE.Alloc(FrameworkUtil.Parse<MESSAGE>(tb.GetStr("ID")));
			if (obj == null)
			{
				Log.Error("[Table]Cannot create 'MESSAGE'. (id={0})", tb.GetStr("ID"));
				return;
			}
			obj.Initialize(tb);
		}
		static void Callback_MESSAGE_JSON(string sheet_name, JsonData node)
		{
			if (node.Keys.Contains("ID") == false) return;
			_MESSAGE obj = Table.T_MESSAGE.Alloc(FrameworkUtil.Parse<MESSAGE>(node["ID"].ToString()));
			if (obj == null)
			{
				Log.Error("[Table]Cannot create 'MESSAGE'. (id={0})", FrameworkUtil.Parse<MESSAGE>(node["ID"].ToString()));
				return;
			}
			obj.Initialize(node);
		}
		static void Callback_UNIT_Sheet(string sheet_name, PropTable tb)
		{
			_UNIT obj = Table.T_UNIT.Alloc(FrameworkUtil.Parse<UNIT>(tb.GetStr("ID")));
			if (obj == null)
			{
				Log.Error("[Table]Cannot create 'UNIT'. (id={0})", tb.GetStr("ID"));
				return;
			}
			obj.Initialize(tb);
		}
		static void Callback_UNIT_JSON(string sheet_name, JsonData node)
		{
			if (node.Keys.Contains("ID") == false) return;
			_UNIT obj = Table.T_UNIT.Alloc(FrameworkUtil.Parse<UNIT>(node["ID"].ToString()));
			if (obj == null)
			{
				Log.Error("[Table]Cannot create 'UNIT'. (id={0})", FrameworkUtil.Parse<UNIT>(node["ID"].ToString()));
				return;
			}
			obj.Initialize(node);
		}
	    public static Container<DataCharacter, UNIT> T_DataCharacter = new Container<DataCharacter, UNIT>();
	    public static Container<_DIRECTION, DIRECTION> T_DIRECTION = new Container<_DIRECTION, DIRECTION>();
	    public static Container<_MESSAGE, MESSAGE> T_MESSAGE = new Container<_MESSAGE, MESSAGE>();
	    public static Container<_UNIT, UNIT> T_UNIT = new Container<_UNIT, UNIT>();
		public static bool isLoad_TestSchema
		{
			get
			{
				if (Table.T_DataCharacter.Count > 0) return true;
				if (Table.T_DIRECTION.Count > 0) return true;
				if (Table.T_MESSAGE.Count > 0) return true;
				if (Table.T_UNIT.Count > 0) return true;
				return false;
			}
		}
		public static void UnLoad_TestSchema()
		{
			Table.T_DataCharacter.Clear();
			Table.T_DIRECTION.Clear();
			Table.T_MESSAGE.Clear();
			Table.T_UNIT.Clear();
		}
		public static bool Load_TestSchema_ExcelFile(string file_path)
		{
			using (ExcelReader reader = new ExcelReader())
			{
				reader.RegisterCallback_Data("DataCharacter", Callback_DataCharacter_Sheet);
				reader.RegisterCallback_Data("DIRECTION", Callback_DIRECTION_Sheet);
				reader.RegisterCallback_Data("MESSAGE", Callback_MESSAGE_Sheet);
				reader.RegisterCallback_Data("UNIT", Callback_UNIT_Sheet);
				return reader.ReadFile(file_path);
			}
		}
		public static void Save_TestSchema_ExcelFile(string file_path)
		{
			using (ExcelWriter writer = new ExcelWriter())
			{
				{
				    DataCharacter temp = new DataCharacter();
				    PropTable tb_header = temp.ToTable();
				    writer.Write_Header(tb_header, false);
				    for (int i = 0; i < Table.T_DataCharacter.Count; i++)
				    {
				        DataCharacter obj = Table.T_DataCharacter.ElementAt(i);
				        PropTable tb = obj.ToTable();
				        writer.Write_Contents(tb);
				    }
				}
				{
				    _DIRECTION temp = new _DIRECTION();
				    PropTable tb_header = temp.ToTable();
				    writer.Write_Header(tb_header, true);
				    for (int i = 0; i < Table.T_DIRECTION.Count; i++)
				    {
				        _DIRECTION obj = Table.T_DIRECTION.ElementAt(i);
				        PropTable tb = obj.ToTable();
				        writer.Write_Contents(tb);
				    }
				}
				{
				    _MESSAGE temp = new _MESSAGE();
				    PropTable tb_header = temp.ToTable();
				    writer.Write_Header(tb_header, true);
				    for (int i = 0; i < Table.T_MESSAGE.Count; i++)
				    {
				        _MESSAGE obj = Table.T_MESSAGE.ElementAt(i);
				        PropTable tb = obj.ToTable();
				        writer.Write_Contents(tb);
				    }
				}
				{
				    _UNIT temp = new _UNIT();
				    PropTable tb_header = temp.ToTable();
				    writer.Write_Header(tb_header, true);
				    for (int i = 0; i < Table.T_UNIT.Count; i++)
				    {
				        _UNIT obj = Table.T_UNIT.ElementAt(i);
				        PropTable tb = obj.ToTable();
				        writer.Write_Contents(tb);
				    }
				}
			    writer.Write_End(file_path);
			}
		}
		public static bool Load_TestSchema_SheetFile(string file_path)
		{
			using (XmlSheetReader reader = new XmlSheetReader())
			{
				reader.RegisterCallback_Data("DataCharacter", Callback_DataCharacter_Sheet);
				reader.RegisterCallback_Data("DIRECTION", Callback_DIRECTION_Sheet);
				reader.RegisterCallback_Data("MESSAGE", Callback_MESSAGE_Sheet);
				reader.RegisterCallback_Data("UNIT", Callback_UNIT_Sheet);
				return reader.ReadFile(file_path);
			}
		}
		public static bool Load_TestSchema_SheetData(string _data)
		{
			using (XmlSheetReader reader = new XmlSheetReader())
			{
				reader.RegisterCallback_Data("DataCharacter", Callback_DataCharacter_Sheet);
				reader.RegisterCallback_Data("DIRECTION", Callback_DIRECTION_Sheet);
				reader.RegisterCallback_Data("MESSAGE", Callback_MESSAGE_Sheet);
				reader.RegisterCallback_Data("UNIT", Callback_UNIT_Sheet);
				return reader.ReadData(_data);
			}
		}
		public static bool Load_TestSchema_JsonFile(string file_path)
		{
			using (JsonReader reader = new JsonReader())
			{
				reader.RegisterCallback("DataCharacter", Callback_DataCharacter_JSON);
				reader.RegisterCallback("DIRECTION", Callback_DIRECTION_JSON);
				reader.RegisterCallback("MESSAGE", Callback_MESSAGE_JSON);
				reader.RegisterCallback("UNIT", Callback_UNIT_JSON);
				return reader.ReadFile(file_path);
			}
		}
		public static bool Load_TestSchema_JsonData(string _data)
		{
			using (JsonReader reader = new JsonReader())
			{
				reader.RegisterCallback("DataCharacter", Callback_DataCharacter_JSON);
				reader.RegisterCallback("DIRECTION", Callback_DIRECTION_JSON);
				reader.RegisterCallback("MESSAGE", Callback_MESSAGE_JSON);
				reader.RegisterCallback("UNIT", Callback_UNIT_JSON);
				return reader.ReadData(_data);
			}
		}
		public static void Save_TestSchema_SheetFile(string file_path)
		{
			using (XmlSheetWriter writer = new XmlSheetWriter())
			{
				{
				    DataCharacter temp = new DataCharacter();
				    PropTable tb_header = temp.ToTable();
				    System.Xml.XmlNode node = writer.Write_Header(tb_header, Table.T_DataCharacter.Count, false);
				    for (int i = 0; i < Table.T_DataCharacter.Count; i++)
				    {
				        DataCharacter obj = Table.T_DataCharacter.ElementAt(i);
				        PropTable tb = obj.ToTable();
				        writer.Write_Contents(node, tb);
				    }
				}
				{
				    _DIRECTION temp = new _DIRECTION();
				    PropTable tb_header = temp.ToTable();
				    System.Xml.XmlNode node = writer.Write_Header(tb_header, Table.T_DIRECTION.Count, true);
				    for (int i = 0; i < Table.T_DIRECTION.Count; i++)
				    {
				        _DIRECTION obj = Table.T_DIRECTION.ElementAt(i);
				        PropTable tb = obj.ToTable();
				        writer.Write_Contents(node, tb);
				    }
				}
				{
				    _MESSAGE temp = new _MESSAGE();
				    PropTable tb_header = temp.ToTable();
				    System.Xml.XmlNode node = writer.Write_Header(tb_header, Table.T_MESSAGE.Count, true);
				    for (int i = 0; i < Table.T_MESSAGE.Count; i++)
				    {
				        _MESSAGE obj = Table.T_MESSAGE.ElementAt(i);
				        PropTable tb = obj.ToTable();
				        writer.Write_Contents(node, tb);
				    }
				}
				{
				    _UNIT temp = new _UNIT();
				    PropTable tb_header = temp.ToTable();
				    System.Xml.XmlNode node = writer.Write_Header(tb_header, Table.T_UNIT.Count, true);
				    for (int i = 0; i < Table.T_UNIT.Count; i++)
				    {
				        _UNIT obj = Table.T_UNIT.ElementAt(i);
				        PropTable tb = obj.ToTable();
				        writer.Write_Contents(node, tb);
				    }
				}
			    writer.Write_End(file_path);
			}
		}
		public static void Save_TestSchema_JsonFile(string file_path)
		{
			TextWriter sw = new StreamWriter(file_path, false);
			sw.WriteLine("{");
			sw.WriteLine("\"DataCharacter\":[");
			for (int i = 0; i < Table.T_DataCharacter.Count; i++)
			{
			    if (i > 0) sw.WriteLine(",");
			    sw.Write(Table.T_DataCharacter.ElementAt(i).ToJson());
			}
			sw.WriteLine("]");
			sw.WriteLine(",\"DIRECTION\":[");
			for (int i = 0; i < Table.T_DIRECTION.Count; i++)
			{
			    if (i > 0) sw.WriteLine(",");
			    sw.Write(Table.T_DIRECTION.ElementAt(i).ToJson());
			}
			sw.WriteLine("]");
			sw.WriteLine(",\"MESSAGE\":[");
			for (int i = 0; i < Table.T_MESSAGE.Count; i++)
			{
			    if (i > 0) sw.WriteLine(",");
			    sw.Write(Table.T_MESSAGE.ElementAt(i).ToJson());
			}
			sw.WriteLine("]");
			sw.WriteLine(",\"UNIT\":[");
			for (int i = 0; i < Table.T_UNIT.Count; i++)
			{
			    if (i > 0) sw.WriteLine(",");
			    sw.Write(Table.T_UNIT.ElementAt(i).ToJson());
			}
			sw.WriteLine("]");
			sw.WriteLine("}");
			sw.Close();
		}
	}
} // end of namespace
