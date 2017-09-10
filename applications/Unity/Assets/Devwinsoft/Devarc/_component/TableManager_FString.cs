using System;
using System.IO;
using LitJson;
namespace Devarc
{
	public partial class TableManager
	{
		public static bool isLoad_FString { get { return m_isLoad_FString;} set { m_isLoad_FString = value; } }
		private static bool m_isLoad_FString = false;
		static void Callback_FString_XML(string sheet_name, PropTable tb)
		{
			 m_isLoad_FString = true;
			using(T_FString obj = T_FString.LIST.Alloc(tb.GetStr("Key")))
			{
				obj.Initialize(tb);
			}
		}
		static void Callback_FString_JSON(string sheet_name, JsonData node)
		{
			if (node.Keys.Contains("unit_type") == false) return;
			m_isLoad_FString = true;
			using(T_FString obj = T_FString.LIST.Alloc(node["Key"].ToString()))
			{
				obj.Initialize(node);
			}
		}
		public static void UnLoad_FString()
		{
			T_FString.LIST.Clear();
		}
		public static bool Load_FString_XmlFile(string file_path)
		{
			using (XmlReader reader = new XmlReader())
			{
				reader.RegisterCallback_Line("FString", Callback_FString_XML);
				return reader.ReadFile(file_path);
			}
		}
		public static bool Load_FString_XmlData(string file_path)
		{
			using (XmlReader reader = new XmlReader())
			{
				reader.RegisterCallback_Line("FString", Callback_FString_XML);
				return reader.ReadData(file_path);
			}
		}
		public static bool Load_FString_JsonFile(string file_path)
		{
			using (JsonReader reader = new JsonReader())
			{
				reader.RegisterCallback("FString", Callback_FString_JSON);
				return reader.ReadFile(file_path);
			}
		}
		public static void Save_FString_XmlFile(string file_path)
		{
			using (XmlWriter writer = new XmlWriter())
			{
				{
				    FString temp = new FString();
				    PropTable tb_header = temp.ToTable();
				    System.Xml.XmlNode node = writer.Write_Header(tb_header, T_FString.LIST.Count, false);
				    for (int i = 0; i < T_FString.LIST.Count; i++)
				    {
				        T_FString obj = T_FString.LIST.ElementAt(i);
				        PropTable tb = obj.ToTable();
				        writer.Write_Contents(node, tb);
				    }
				}
			    writer.Write_End(file_path);
			}
		}
		public static void Save_FString_JsonFile(string file_path)
		{
			TextWriter sw = new StreamWriter(file_path, false);
			sw.WriteLine("{");
			sw.WriteLine("\"FString\":[");
			for (int i = 0; i < T_FString.LIST.Count; i++)
			{
			    if (i > 0) sw.WriteLine(",");
			    sw.Write(T_FString.LIST.ElementAt(i).ToJson());
			}
			sw.WriteLine("]");
			sw.WriteLine("}");
			sw.Close();
		}
	}
} // end of namespace
