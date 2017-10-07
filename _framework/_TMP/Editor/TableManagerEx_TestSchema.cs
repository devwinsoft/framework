using System;
using System.IO;
using LitJson;
namespace Devarc
{
	public partial class TableManagerEx
	{
		static void Callback_DataCharacter_Sheet(string sheet_name, PropTable tb)
		{
			DataCharacter obj = TableManager.T_DataCharacter.Alloc(_UNIT.Parse(tb.GetStr("unit_type")));
			if (obj == null)
			{
				Log.Error("[TableManager]Cannot create 'DataCharacter'. (id={0})", tb.GetStr("unit_type"));
				return;
			}
			obj.Initialize(tb);
		}
		static void Callback_DataCharacter_JSON(string sheet_name, JsonData node)
		{
			if (node.Keys.Contains("unit_type") == false) return;
			DataCharacter obj = TableManager.T_DataCharacter.Alloc(_UNIT.Parse(node["unit_type"].ToString()));
			if (obj == null)
			{
				Log.Error("[TableManager]Cannot create 'DataCharacter'. (id={0})", _UNIT.Parse(node["unit_type"].ToString()));
				return;
			}
			obj.Initialize(node);
		}
		static void Callback_DIRECTION_Sheet(string sheet_name, PropTable tb)
		{
			_DIRECTION obj = TableManager.T_DIRECTION.Alloc(_DIRECTION.Parse(tb.GetStr("ID")));
			if (obj == null)
			{
				Log.Error("[TableManager]Cannot create 'DIRECTION'. (id={0})", tb.GetStr("ID"));
				return;
			}
			obj.Initialize(tb);
		}
		static void Callback_DIRECTION_JSON(string sheet_name, JsonData node)
		{
			if (node.Keys.Contains("unit_type") == false) return;
			_DIRECTION obj = TableManager.T_DIRECTION.Alloc(_DIRECTION.Parse(node["ID"].ToString()));
			if (obj == null)
			{
				Log.Error("[TableManager]Cannot create 'DIRECTION'. (id={0})", _DIRECTION.Parse(node["ID"].ToString()));
				return;
			}
			obj.Initialize(node);
		}
		static void Callback_MESSAGE_Sheet(string sheet_name, PropTable tb)
		{
			_MESSAGE obj = TableManager.T_MESSAGE.Alloc(_MESSAGE.Parse(tb.GetStr("ID")));
			if (obj == null)
			{
				Log.Error("[TableManager]Cannot create 'MESSAGE'. (id={0})", tb.GetStr("ID"));
				return;
			}
			obj.Initialize(tb);
		}
		static void Callback_MESSAGE_JSON(string sheet_name, JsonData node)
		{
			if (node.Keys.Contains("unit_type") == false) return;
			_MESSAGE obj = TableManager.T_MESSAGE.Alloc(_MESSAGE.Parse(node["ID"].ToString()));
			if (obj == null)
			{
				Log.Error("[TableManager]Cannot create 'MESSAGE'. (id={0})", _MESSAGE.Parse(node["ID"].ToString()));
				return;
			}
			obj.Initialize(node);
		}
		static void Callback_UNIT_Sheet(string sheet_name, PropTable tb)
		{
			_UNIT obj = TableManager.T_UNIT.Alloc(_UNIT.Parse(tb.GetStr("ID")));
			if (obj == null)
			{
				Log.Error("[TableManager]Cannot create 'UNIT'. (id={0})", tb.GetStr("ID"));
				return;
			}
			obj.Initialize(tb);
		}
		static void Callback_UNIT_JSON(string sheet_name, JsonData node)
		{
			if (node.Keys.Contains("unit_type") == false) return;
			_UNIT obj = TableManager.T_UNIT.Alloc(_UNIT.Parse(node["ID"].ToString()));
			if (obj == null)
			{
				Log.Error("[TableManager]Cannot create 'UNIT'. (id={0})", _UNIT.Parse(node["ID"].ToString()));
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
				if (TableManager.T_DataCharacter.Count > 0) return true;
				if (TableManager.T_DIRECTION.Count > 0) return true;
				if (TableManager.T_MESSAGE.Count > 0) return true;
				if (TableManager.T_UNIT.Count > 0) return true;
				return false;
			}
		}
		public static void UnLoad_TestSchema()
		{
			TableManager.T_DataCharacter.Clear();
			TableManager.T_DIRECTION.Clear();
			TableManager.T_MESSAGE.Clear();
			TableManager.T_UNIT.Clear();
		}
		public static bool Load_TestSchema_ExcelFile(string file_path)
		{
			using (ExcelReader reader = new ExcelReader())
			{
				reader.RegisterCallback_DataLine("DataCharacter", Callback_DataCharacter_Sheet);
				reader.RegisterCallback_DataLine("DIRECTION", Callback_DIRECTION_Sheet);
				reader.RegisterCallback_DataLine("MESSAGE", Callback_MESSAGE_Sheet);
				reader.RegisterCallback_DataLine("UNIT", Callback_UNIT_Sheet);
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
				    for (int i = 0; i < TableManager.T_DataCharacter.Count; i++)
				    {
				        DataCharacter obj = TableManager.T_DataCharacter.ElementAt(i);
				        PropTable tb = obj.ToTable();
				        writer.Write_Contents(tb);
				    }
				}
				{
				    _DIRECTION temp = new _DIRECTION();
				    PropTable tb_header = temp.ToTable();
				    writer.Write_Header(tb_header, true);
				    for (int i = 0; i < TableManager.T_DIRECTION.Count; i++)
				    {
				        _DIRECTION obj = TableManager.T_DIRECTION.ElementAt(i);
				        PropTable tb = obj.ToTable();
				        writer.Write_Contents(tb);
				    }
				}
				{
				    _MESSAGE temp = new _MESSAGE();
				    PropTable tb_header = temp.ToTable();
				    writer.Write_Header(tb_header, true);
				    for (int i = 0; i < TableManager.T_MESSAGE.Count; i++)
				    {
				        _MESSAGE obj = TableManager.T_MESSAGE.ElementAt(i);
				        PropTable tb = obj.ToTable();
				        writer.Write_Contents(tb);
				    }
				}
				{
				    _UNIT temp = new _UNIT();
				    PropTable tb_header = temp.ToTable();
				    writer.Write_Header(tb_header, true);
				    for (int i = 0; i < TableManager.T_UNIT.Count; i++)
				    {
				        _UNIT obj = TableManager.T_UNIT.ElementAt(i);
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
				reader.RegisterCallback_DataLine("DataCharacter", Callback_DataCharacter_Sheet);
				reader.RegisterCallback_DataLine("DIRECTION", Callback_DIRECTION_Sheet);
				reader.RegisterCallback_DataLine("MESSAGE", Callback_MESSAGE_Sheet);
				reader.RegisterCallback_DataLine("UNIT", Callback_UNIT_Sheet);
				return reader.ReadFile(file_path);
			}
		}
		public static bool Load_TestSchema_SheetData(string _data)
		{
			using (XmlSheetReader reader = new XmlSheetReader())
			{
				reader.RegisterCallback_DataLine("DataCharacter", Callback_DataCharacter_Sheet);
				reader.RegisterCallback_DataLine("DIRECTION", Callback_DIRECTION_Sheet);
				reader.RegisterCallback_DataLine("MESSAGE", Callback_MESSAGE_Sheet);
				reader.RegisterCallback_DataLine("UNIT", Callback_UNIT_Sheet);
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
		public static void Save_TestSchema_SheetFile(string file_path)
		{
			using (XmlSheetWriter writer = new XmlSheetWriter())
			{
				{
				    DataCharacter temp = new DataCharacter();
				    PropTable tb_header = temp.ToTable();
				    System.Xml.XmlNode node = writer.Write_Header(tb_header, TableManager.T_DataCharacter.Count, false);
				    for (int i = 0; i < TableManager.T_DataCharacter.Count; i++)
				    {
				        DataCharacter obj = TableManager.T_DataCharacter.ElementAt(i);
				        PropTable tb = obj.ToTable();
				        writer.Write_Contents(node, tb);
				    }
				}
				{
				    _DIRECTION temp = new _DIRECTION();
				    PropTable tb_header = temp.ToTable();
				    System.Xml.XmlNode node = writer.Write_Header(tb_header, TableManager.T_DIRECTION.Count, true);
				    for (int i = 0; i < TableManager.T_DIRECTION.Count; i++)
				    {
				        _DIRECTION obj = TableManager.T_DIRECTION.ElementAt(i);
				        PropTable tb = obj.ToTable();
				        writer.Write_Contents(node, tb);
				    }
				}
				{
				    _MESSAGE temp = new _MESSAGE();
				    PropTable tb_header = temp.ToTable();
				    System.Xml.XmlNode node = writer.Write_Header(tb_header, TableManager.T_MESSAGE.Count, true);
				    for (int i = 0; i < TableManager.T_MESSAGE.Count; i++)
				    {
				        _MESSAGE obj = TableManager.T_MESSAGE.ElementAt(i);
				        PropTable tb = obj.ToTable();
				        writer.Write_Contents(node, tb);
				    }
				}
				{
				    _UNIT temp = new _UNIT();
				    PropTable tb_header = temp.ToTable();
				    System.Xml.XmlNode node = writer.Write_Header(tb_header, TableManager.T_UNIT.Count, true);
				    for (int i = 0; i < TableManager.T_UNIT.Count; i++)
				    {
				        _UNIT obj = TableManager.T_UNIT.ElementAt(i);
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
			for (int i = 0; i < TableManager.T_DataCharacter.Count; i++)
			{
			    if (i > 0) sw.WriteLine(",");
			    sw.Write(TableManager.T_DataCharacter.ElementAt(i).ToJson());
			}
			sw.WriteLine("]");
			sw.WriteLine(",\"DIRECTION\":[");
			for (int i = 0; i < TableManager.T_DIRECTION.Count; i++)
			{
			    if (i > 0) sw.WriteLine(",");
			    sw.Write(TableManager.T_DIRECTION.ElementAt(i).ToJson());
			}
			sw.WriteLine("]");
			sw.WriteLine(",\"MESSAGE\":[");
			for (int i = 0; i < TableManager.T_MESSAGE.Count; i++)
			{
			    if (i > 0) sw.WriteLine(",");
			    sw.Write(TableManager.T_MESSAGE.ElementAt(i).ToJson());
			}
			sw.WriteLine("]");
			sw.WriteLine(",\"UNIT\":[");
			for (int i = 0; i < TableManager.T_UNIT.Count; i++)
			{
			    if (i > 0) sw.WriteLine(",");
			    sw.Write(TableManager.T_UNIT.ElementAt(i).ToJson());
			}
			sw.WriteLine("]");
			sw.WriteLine("}");
			sw.Close();
		}
	}
} // end of namespace
