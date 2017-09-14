using System;
using System.IO;
using LitJson;
namespace Devarc
{
	public partial class TableManager
	{
		static void Callback_DataCharacter_XML(string sheet_name, PropTable tb)
		{
			using(T_DataCharacter obj = T_DataCharacter.MAP.Alloc(_UNIT.Parse(tb.GetStr("unit_type"))))
			{
				if (obj == null)
				{
					Log.Error("[TableManager]Cannot create 'DataCharacter'. (id={0})", tb.GetStr("unit_type"));
				}
				obj.Initialize(tb);
			}
		}
		static void Callback_DataCharacter_JSON(string sheet_name, JsonData node)
		{
			if (node.Keys.Contains("unit_type") == false) return;
			using(T_DataCharacter obj = T_DataCharacter.MAP.Alloc(_UNIT.Parse(node["unit_type"].ToString())))
			{
				obj.Initialize(node);
			}
		}
		static void Callback_UNIT_XML(string sheet_name, PropTable tb)
		{
			using(T_UNIT obj = T_UNIT.MAP.Alloc(_UNIT.Parse(tb.GetStr("ID"))))
			{
				if (obj == null)
				{
					Log.Error("[TableManager]Cannot create 'UNIT'. (id={0})", tb.GetStr("ID"));
				}
				obj.Initialize(tb);
			}
		}
		static void Callback_UNIT_JSON(string sheet_name, JsonData node)
		{
			if (node.Keys.Contains("unit_type") == false) return;
			using(T_UNIT obj = T_UNIT.MAP.Alloc(_UNIT.Parse(node["ID"].ToString())))
			{
				obj.Initialize(node);
			}
		}
		static void Callback_DIRECTION_XML(string sheet_name, PropTable tb)
		{
			using(T_DIRECTION obj = T_DIRECTION.MAP.Alloc(_DIRECTION.Parse(tb.GetStr("ID"))))
			{
				if (obj == null)
				{
					Log.Error("[TableManager]Cannot create 'DIRECTION'. (id={0})", tb.GetStr("ID"));
				}
				obj.Initialize(tb);
			}
		}
		static void Callback_DIRECTION_JSON(string sheet_name, JsonData node)
		{
			if (node.Keys.Contains("unit_type") == false) return;
			using(T_DIRECTION obj = T_DIRECTION.MAP.Alloc(_DIRECTION.Parse(node["ID"].ToString())))
			{
				obj.Initialize(node);
			}
		}
		public static bool isLoad_TestSchema
		{
			get
			{
				if (T_DataCharacter.MAP.Count > 0) return true;
				if (T_UNIT.MAP.Count > 0) return true;
				if (T_DIRECTION.MAP.Count > 0) return true;
				return false;
			}
		}
		public static void UnLoad_TestSchema()
		{
			T_DataCharacter.MAP.Clear();
			T_UNIT.MAP.Clear();
			T_DIRECTION.MAP.Clear();
		}
		public static bool Load_TestSchema_XmlFile(string file_path)
		{
			using (XmlSheetReader reader = new XmlSheetReader())
			{
				reader.RegisterCallback_Line("DataCharacter", Callback_DataCharacter_XML);
				reader.RegisterCallback_Line("UNIT", Callback_UNIT_XML);
				reader.RegisterCallback_Line("DIRECTION", Callback_DIRECTION_XML);
				return reader.ReadFile(file_path);
			}
		}
		public static bool Load_TestSchema_XmlData(string _data)
		{
			using (XmlSheetReader reader = new XmlSheetReader())
			{
				reader.RegisterCallback_Line("DataCharacter", Callback_DataCharacter_XML);
				reader.RegisterCallback_Line("UNIT", Callback_UNIT_XML);
				reader.RegisterCallback_Line("DIRECTION", Callback_DIRECTION_XML);
				return reader.ReadData(_data);
			}
		}
		public static bool Load_TestSchema_JsonFile(string file_path)
		{
			using (JsonReader reader = new JsonReader())
			{
				reader.RegisterCallback("DataCharacter", Callback_DataCharacter_JSON);
				reader.RegisterCallback("UNIT", Callback_UNIT_JSON);
				reader.RegisterCallback("DIRECTION", Callback_DIRECTION_JSON);
				return reader.ReadFile(file_path);
			}
		}
		public static void Save_TestSchema_XmlFile(string file_path)
		{
			using (XmlSheetWriter writer = new XmlSheetWriter())
			{
				{
				    DataCharacter temp = new DataCharacter();
				    PropTable tb_header = temp.ToTable();
				    System.Xml.XmlNode node = writer.Write_Header(tb_header, T_DataCharacter.MAP.Count, false);
				    for (int i = 0; i < T_DataCharacter.MAP.Count; i++)
				    {
				        T_DataCharacter obj = T_DataCharacter.MAP.ElementAt(i);
				        PropTable tb = obj.ToTable();
				        writer.Write_Contents(node, tb);
				    }
				}
				{
				    _UNIT temp = new _UNIT();
				    PropTable tb_header = temp.ToTable();
				    System.Xml.XmlNode node = writer.Write_Header(tb_header, T_UNIT.MAP.Count, true);
				    for (int i = 0; i < T_UNIT.MAP.Count; i++)
				    {
				        T_UNIT obj = T_UNIT.MAP.ElementAt(i);
				        PropTable tb = obj.ToTable();
				        writer.Write_Contents(node, tb);
				    }
				}
				{
				    _DIRECTION temp = new _DIRECTION();
				    PropTable tb_header = temp.ToTable();
				    System.Xml.XmlNode node = writer.Write_Header(tb_header, T_DIRECTION.MAP.Count, true);
				    for (int i = 0; i < T_DIRECTION.MAP.Count; i++)
				    {
				        T_DIRECTION obj = T_DIRECTION.MAP.ElementAt(i);
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
			for (int i = 0; i < T_DataCharacter.MAP.Count; i++)
			{
			    if (i > 0) sw.WriteLine(",");
			    sw.Write(T_DataCharacter.MAP.ElementAt(i).ToJson());
			}
			sw.WriteLine("]");
			sw.WriteLine(",\"UNIT\":[");
			for (int i = 0; i < T_UNIT.MAP.Count; i++)
			{
			    if (i > 0) sw.WriteLine(",");
			    sw.Write(T_UNIT.MAP.ElementAt(i).ToJson());
			}
			sw.WriteLine("]");
			sw.WriteLine(",\"DIRECTION\":[");
			for (int i = 0; i < T_DIRECTION.MAP.Count; i++)
			{
			    if (i > 0) sw.WriteLine(",");
			    sw.Write(T_DIRECTION.MAP.ElementAt(i).ToJson());
			}
			sw.WriteLine("]");
			sw.WriteLine("}");
			sw.Close();
		}
	}
} // end of namespace
