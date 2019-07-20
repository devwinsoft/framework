using System;
using System.IO;
using LitJson;
namespace Devarc
{
	public partial class Table
	{
		static void Callback_XML_DataCharacter(string sheet_name, PropTable tb)
		{
			DataCharacter obj = Table.T_DataCharacter.Alloc(FrameworkUtil.Parse<UNIT>(tb.GetStr("unit_type")));
			if (obj == null)
			{
				Log.Error("[Table]Cannot create 'DataCharacter'. (id={0})", tb.GetStr("unit_type"));
				return;
			}
			obj.Initialize(tb);
		}
		static void Callback_Sheet_DataCharacter(string sheet_name, PropTable tb)
		{
			DataCharacter obj = Table.T_DataCharacter.Alloc(FrameworkUtil.Parse<UNIT>(tb.GetStr("unit_type")));
			if (obj == null)
			{
				Log.Error("[Table]Cannot create 'DataCharacter'. (id={0})", tb.GetStr("unit_type"));
				return;
			}
			obj.Initialize(tb);
		}
		static void Callback_JSON_DataCharacter(string sheet_name, JsonData node)
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
		static void Callback_XML_DIRECTION(string sheet_name, PropTable tb)
		{
			_DIRECTION obj = Table.T_DIRECTION.Alloc(FrameworkUtil.Parse<DIRECTION>(tb.GetStr("ID")));
			if (obj == null)
			{
				Log.Error("[Table]Cannot create 'DIRECTION'. (id={0})", tb.GetStr("ID"));
				return;
			}
			obj.Initialize(tb);
		}
		static void Callback_Sheet_DIRECTION(string sheet_name, PropTable tb)
		{
			_DIRECTION obj = Table.T_DIRECTION.Alloc(FrameworkUtil.Parse<DIRECTION>(tb.GetStr("ID")));
			if (obj == null)
			{
				Log.Error("[Table]Cannot create 'DIRECTION'. (id={0})", tb.GetStr("ID"));
				return;
			}
			obj.Initialize(tb);
		}
		static void Callback_JSON_DIRECTION(string sheet_name, JsonData node)
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
		static void Callback_XML_MESSAGE(string sheet_name, PropTable tb)
		{
			_MESSAGE obj = Table.T_MESSAGE.Alloc(FrameworkUtil.Parse<MESSAGE>(tb.GetStr("ID")));
			if (obj == null)
			{
				Log.Error("[Table]Cannot create 'MESSAGE'. (id={0})", tb.GetStr("ID"));
				return;
			}
			obj.Initialize(tb);
		}
		static void Callback_Sheet_MESSAGE(string sheet_name, PropTable tb)
		{
			_MESSAGE obj = Table.T_MESSAGE.Alloc(FrameworkUtil.Parse<MESSAGE>(tb.GetStr("ID")));
			if (obj == null)
			{
				Log.Error("[Table]Cannot create 'MESSAGE'. (id={0})", tb.GetStr("ID"));
				return;
			}
			obj.Initialize(tb);
		}
		static void Callback_JSON_MESSAGE(string sheet_name, JsonData node)
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
		static void Callback_XML_UNIT(string sheet_name, PropTable tb)
		{
			_UNIT obj = Table.T_UNIT.Alloc(FrameworkUtil.Parse<UNIT>(tb.GetStr("ID")));
			if (obj == null)
			{
				Log.Error("[Table]Cannot create 'UNIT'. (id={0})", tb.GetStr("ID"));
				return;
			}
			obj.Initialize(tb);
		}
		static void Callback_Sheet_UNIT(string sheet_name, PropTable tb)
		{
			_UNIT obj = Table.T_UNIT.Alloc(FrameworkUtil.Parse<UNIT>(tb.GetStr("ID")));
			if (obj == null)
			{
				Log.Error("[Table]Cannot create 'UNIT'. (id={0})", tb.GetStr("ID"));
				return;
			}
			obj.Initialize(tb);
		}
		static void Callback_JSON_UNIT(string sheet_name, JsonData node)
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
		public static bool Load_XmlData_DataCharacter(string file_path)
		{
			using (XmlReader reader = new XmlReader())
			{
				reader.RegisterCallback_Data(Callback_XML_DataCharacter);
				return reader.ReadData(file_path);
			}
		}
		public static bool Load_XmlData_DIRECTION(string file_path)
		{
			using (XmlReader reader = new XmlReader())
			{
				reader.RegisterCallback_Data(Callback_XML_DIRECTION);
				return reader.ReadData(file_path);
			}
		}
		public static bool Load_XmlData_MESSAGE(string file_path)
		{
			using (XmlReader reader = new XmlReader())
			{
				reader.RegisterCallback_Data(Callback_XML_MESSAGE);
				return reader.ReadData(file_path);
			}
		}
		public static bool Load_XmlData_UNIT(string file_path)
		{
			using (XmlReader reader = new XmlReader())
			{
				reader.RegisterCallback_Data(Callback_XML_UNIT);
				return reader.ReadData(file_path);
			}
		}
		public static bool Load_XmlFile_DataCharacter(string file_path)
		{
			using (XmlReader reader = new XmlReader())
			{
				reader.RegisterCallback_Data(Callback_XML_DataCharacter);
				return reader.ReadFile(file_path);
			}
		}
		public static bool Load_XmlFile_DIRECTION(string file_path)
		{
			using (XmlReader reader = new XmlReader())
			{
				reader.RegisterCallback_Data(Callback_XML_DIRECTION);
				return reader.ReadFile(file_path);
			}
		}
		public static bool Load_XmlFile_MESSAGE(string file_path)
		{
			using (XmlReader reader = new XmlReader())
			{
				reader.RegisterCallback_Data(Callback_XML_MESSAGE);
				return reader.ReadFile(file_path);
			}
		}
		public static bool Load_XmlFile_UNIT(string file_path)
		{
			using (XmlReader reader = new XmlReader())
			{
				reader.RegisterCallback_Data(Callback_XML_UNIT);
				return reader.ReadFile(file_path);
			}
		}
		public static bool Load_SheetFile_TestSchema(string file_path)
		{
			using (XmlSheetReader reader = new XmlSheetReader())
			{
				reader.RegisterCallback_Data("DataCharacter", Callback_Sheet_DataCharacter);
				reader.RegisterCallback_Data("DIRECTION", Callback_Sheet_DIRECTION);
				reader.RegisterCallback_Data("MESSAGE", Callback_Sheet_MESSAGE);
				reader.RegisterCallback_Data("UNIT", Callback_Sheet_UNIT);
				return reader.ReadFile(file_path);
			}
		}
		public static bool Load_SheetData_TestSchema(string _data)
		{
			using (XmlSheetReader reader = new XmlSheetReader())
			{
				reader.RegisterCallback_Data("DataCharacter", Callback_Sheet_DataCharacter);
				reader.RegisterCallback_Data("DIRECTION", Callback_Sheet_DIRECTION);
				reader.RegisterCallback_Data("MESSAGE", Callback_Sheet_MESSAGE);
				reader.RegisterCallback_Data("UNIT", Callback_Sheet_UNIT);
				return reader.ReadData(_data);
			}
		}
		public static bool Load_JsonFile_TestSchema(string file_path)
		{
			using (JsonReader reader = new JsonReader())
			{
				reader.RegisterCallback("DataCharacter", Callback_JSON_DataCharacter);
				reader.RegisterCallback("DIRECTION", Callback_JSON_DIRECTION);
				reader.RegisterCallback("MESSAGE", Callback_JSON_MESSAGE);
				reader.RegisterCallback("UNIT", Callback_JSON_UNIT);
				return reader.ReadFile(file_path);
			}
		}
		public static bool Load_JsonData_TestSchema(string _data)
		{
			using (JsonReader reader = new JsonReader())
			{
				reader.RegisterCallback("DataCharacter", Callback_JSON_DataCharacter);
				reader.RegisterCallback("DIRECTION", Callback_JSON_DIRECTION);
				reader.RegisterCallback("MESSAGE", Callback_JSON_MESSAGE);
				reader.RegisterCallback("UNIT", Callback_JSON_UNIT);
				return reader.ReadData(_data);
			}
		}
		public static void Save_XmlFile_DataCharacter(string file_path)
		{
			using (XmlWriter writer = new XmlWriter("DataCharacter"))
			{
				DataCharacter temp = new DataCharacter();
				writer.Write_Begin(file_path, temp.ToTable());
				for (int i = 0; i < Table.T_DataCharacter.Count; i++)
				{
				    DataCharacter obj = Table.T_DataCharacter.ElementAt(i);
				    PropTable tb = obj.ToTable();
				    writer.Write_Data(tb);
				}
				writer.Write_End();
			}
		}
		public static void Save_XmlFile_DIRECTION(string file_path)
		{
			using (XmlWriter writer = new XmlWriter("DIRECTION"))
			{
				_DIRECTION temp = new _DIRECTION();
				writer.Write_Begin(file_path, temp.ToTable());
				for (int i = 0; i < Table.T_DIRECTION.Count; i++)
				{
				    _DIRECTION obj = Table.T_DIRECTION.ElementAt(i);
				    PropTable tb = obj.ToTable();
				    writer.Write_Data(tb);
				}
				writer.Write_End();
			}
		}
		public static void Save_XmlFile_MESSAGE(string file_path)
		{
			using (XmlWriter writer = new XmlWriter("MESSAGE"))
			{
				_MESSAGE temp = new _MESSAGE();
				writer.Write_Begin(file_path, temp.ToTable());
				for (int i = 0; i < Table.T_MESSAGE.Count; i++)
				{
				    _MESSAGE obj = Table.T_MESSAGE.ElementAt(i);
				    PropTable tb = obj.ToTable();
				    writer.Write_Data(tb);
				}
				writer.Write_End();
			}
		}
		public static void Save_XmlFile_UNIT(string file_path)
		{
			using (XmlWriter writer = new XmlWriter("UNIT"))
			{
				_UNIT temp = new _UNIT();
				writer.Write_Begin(file_path, temp.ToTable());
				for (int i = 0; i < Table.T_UNIT.Count; i++)
				{
				    _UNIT obj = Table.T_UNIT.ElementAt(i);
				    PropTable tb = obj.ToTable();
				    writer.Write_Data(tb);
				}
				writer.Write_End();
			}
		}
		public static void Save_SheetFile_TestSchema(string file_path)
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
		public static void Save_JsonFile_TestSchema(string file_path)
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
