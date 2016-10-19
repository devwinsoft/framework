using System;
using System.IO;
using LitJson;
namespace Devarc
{
	public partial class DataManager
	{
		public static bool isLoad_TestSchema { get { return m_isLoad_TestSchema;} set { m_isLoad_TestSchema = value; } }
		private static bool m_isLoad_TestSchema = false;
		static void Callback_DataCharacter_XML(string sheet_name, PropTable tb)
		{
			 m_isLoad_TestSchema = true;
			using(T_DataCharacter obj = T_DataCharacter.LIST.Alloc(_UNIT.Parse(tb.ToStr("unit_type"))))
			{
				obj.Initialize(tb);
			}
		}
		static void Callback_DataCharacter_JSON(string sheet_name, JsonData node)
		{
			if (node.Keys.Contains("unit_type") == false) return;
			m_isLoad_TestSchema = true;
			using(T_DataCharacter obj = T_DataCharacter.LIST.Alloc(_UNIT.Parse(node["unit_type"].ToString())))
			{
				obj.Initialize(node);
			}
		}
		static void Callback_UNIT_XML(string sheet_name, PropTable tb)
		{
			 m_isLoad_TestSchema = true;
			using(T_UNIT obj = T_UNIT.LIST.Alloc(_UNIT.Parse(tb.ToStr("ID"))))
			{
				obj.Initialize(tb);
			}
		}
		static void Callback_UNIT_JSON(string sheet_name, JsonData node)
		{
			if (node.Keys.Contains("unit_type") == false) return;
			m_isLoad_TestSchema = true;
			using(T_UNIT obj = T_UNIT.LIST.Alloc(_UNIT.Parse(node["ID"].ToString())))
			{
				obj.Initialize(node);
			}
		}
		static void Callback_DIRECTION_XML(string sheet_name, PropTable tb)
		{
			 m_isLoad_TestSchema = true;
			using(T_DIRECTION obj = T_DIRECTION.LIST.Alloc(_DIRECTION.Parse(tb.ToStr("ID"))))
			{
				obj.Initialize(tb);
			}
		}
		static void Callback_DIRECTION_JSON(string sheet_name, JsonData node)
		{
			if (node.Keys.Contains("unit_type") == false) return;
			m_isLoad_TestSchema = true;
			using(T_DIRECTION obj = T_DIRECTION.LIST.Alloc(_DIRECTION.Parse(node["ID"].ToString())))
			{
				obj.Initialize(node);
			}
		}
		public static void UnLoad_TestSchema()
		{
			T_DataCharacter.LIST.Clear();
			T_UNIT.LIST.Clear();
			T_DIRECTION.LIST.Clear();
		}
		public static bool Load_TestSchema_XmlFile(string file_path)
		{
			using (XmlReader reader = new XmlReader())
			{
				reader.RegisterCallback_Line("DataCharacter", Callback_DataCharacter_XML);
				reader.RegisterCallback_Line("UNIT", Callback_UNIT_XML);
				reader.RegisterCallback_Line("DIRECTION", Callback_DIRECTION_XML);
				return reader.ReadFile(file_path);
			}
		}
		public static bool Load_TestSchema_XmlData(string file_path)
		{
			using (XmlReader reader = new XmlReader())
			{
				reader.RegisterCallback_Line("DataCharacter", Callback_DataCharacter_XML);
				reader.RegisterCallback_Line("UNIT", Callback_UNIT_XML);
				reader.RegisterCallback_Line("DIRECTION", Callback_DIRECTION_XML);
				return reader.ReadData(file_path);
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
			using (XmlWriter writer = new XmlWriter())
			{
				{
				    DataCharacter temp = new DataCharacter();
				    PropTable tb_header = temp.ToTable();
				    System.Xml.XmlNode node = writer.Write_Header(tb_header, T_DataCharacter.LIST.Count, false);
				    for (int i = 0; i < T_DataCharacter.LIST.Count; i++)
				    {
				        T_DataCharacter obj = T_DataCharacter.LIST.ElementAt(i);
				        PropTable tb = obj.ToTable();
				        writer.Write_Contents(node, tb);
				    }
				}
				{
				    _UNIT temp = new _UNIT();
				    PropTable tb_header = temp.ToTable();
				    System.Xml.XmlNode node = writer.Write_Header(tb_header, T_UNIT.LIST.Count, true);
				    for (int i = 0; i < T_UNIT.LIST.Count; i++)
				    {
				        T_UNIT obj = T_UNIT.LIST.ElementAt(i);
				        PropTable tb = obj.ToTable();
				        writer.Write_Contents(node, tb);
				    }
				}
				{
				    _DIRECTION temp = new _DIRECTION();
				    PropTable tb_header = temp.ToTable();
				    System.Xml.XmlNode node = writer.Write_Header(tb_header, T_DIRECTION.LIST.Count, true);
				    for (int i = 0; i < T_DIRECTION.LIST.Count; i++)
				    {
				        T_DIRECTION obj = T_DIRECTION.LIST.ElementAt(i);
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
			for (int i = 0; i < T_DataCharacter.LIST.Count; i++)
			{
			    if (i > 0) sw.WriteLine(",");
			    sw.Write(T_DataCharacter.LIST.ElementAt(i).ToJson());
			}
			sw.WriteLine("]");
			sw.WriteLine(",\"UNIT\":[");
			for (int i = 0; i < T_UNIT.LIST.Count; i++)
			{
			    if (i > 0) sw.WriteLine(",");
			    sw.Write(T_UNIT.LIST.ElementAt(i).ToJson());
			}
			sw.WriteLine("]");
			sw.WriteLine(",\"DIRECTION\":[");
			for (int i = 0; i < T_DIRECTION.LIST.Count; i++)
			{
			    if (i > 0) sw.WriteLine(",");
			    sw.Write(T_DIRECTION.LIST.ElementAt(i).ToJson());
			}
			sw.WriteLine("]");
			sw.WriteLine("}");
			sw.Close();
		}
	}
} // end of namespace
