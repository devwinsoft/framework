using System;
using System.IO;
using LitJson;
namespace Devarc
{
	public partial class TableManager
	{
		public static bool isLoad_LString { get { return m_isLoad_LString;} set { m_isLoad_LString = value; } }
		private static bool m_isLoad_LString = false;
		static void Callback_LString_XML(string sheet_name, PropTable tb)
		{
			 m_isLoad_LString = true;
			using(T_LString obj = T_LString.MAP.Alloc(tb.GetStr("Key")))
			{
				obj.Initialize(tb);
			}
		}
		static void Callback_LString_JSON(string sheet_name, JsonData node)
		{
			if (node.Keys.Contains("unit_type") == false) return;
			m_isLoad_LString = true;
			using(T_LString obj = T_LString.MAP.Alloc(node["Key"].ToString()))
			{
				obj.Initialize(node);
			}
		}
		public static void UnLoad_LString()
		{
			T_LString.MAP.Clear();
		}
		public static bool Load_LString_XmlFile(string file_path)
		{
			using (XmlReader reader = new XmlReader())
			{
				reader.RegisterCallback_Line("LString", Callback_LString_XML);
				return reader.ReadFile(file_path);
			}
		}
		public static bool Load_LString_XmlData(string file_path)
		{
			using (XmlReader reader = new XmlReader())
			{
				reader.RegisterCallback_Line("LString", Callback_LString_XML);
				return reader.ReadData(file_path);
			}
		}
		public static bool Load_LString_JsonFile(string file_path)
		{
			using (JsonReader reader = new JsonReader())
			{
				reader.RegisterCallback("LString", Callback_LString_JSON);
				return reader.ReadFile(file_path);
			}
		}
		public static void Save_LString_XmlFile(string file_path)
		{
			using (XmlWriter writer = new XmlWriter())
			{
				{
				    LString temp = new LString();
				    PropTable tb_header = temp.ToTable();
				    System.Xml.XmlNode node = writer.Write_Header(tb_header, T_LString.MAP.Count, false);
				    for (int i = 0; i < T_LString.MAP.Count; i++)
				    {
				        T_LString obj = T_LString.MAP.ElementAt(i);
				        PropTable tb = obj.ToTable();
				        writer.Write_Contents(node, tb);
				    }
				}
			    writer.Write_End(file_path);
			}
		}
		public static void Save_LString_JsonFile(string file_path)
		{
			TextWriter sw = new StreamWriter(file_path, false);
			sw.WriteLine("{");
			sw.WriteLine("\"LString\":[");
			for (int i = 0; i < T_LString.MAP.Count; i++)
			{
			    if (i > 0) sw.WriteLine(",");
			    sw.Write(T_LString.MAP.ElementAt(i).ToJson());
			}
			sw.WriteLine("]");
			sw.WriteLine("}");
			sw.Close();
		}
	}
} // end of namespace
