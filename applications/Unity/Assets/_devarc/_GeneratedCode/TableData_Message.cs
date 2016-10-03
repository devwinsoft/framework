using System;
using System.IO;
using LitJson;
namespace Devarc
{
	public partial class TableData
	{
		public static bool isLoad_Message { get { return m_isLoad_Message;} set { m_isLoad_Message = value; } }
		private static bool m_isLoad_Message = false;
		static void Callback_MESSAGE_XML(string sheet_name, PropTable tb)
		{
			 m_isLoad_Message = true;
			using(T_MESSAGE obj = T_MESSAGE.LIST.Alloc(_MESSAGE.Parse(tb.ToStr("ID"))))
			{
				obj.Initialize(tb);
			}
		}
		static void Callback_MESSAGE_JSON(string sheet_name, JsonData node)
		{
			if (node.Keys.Contains("unit_type") == false) return;
			m_isLoad_Message = true;
			using(T_MESSAGE obj = T_MESSAGE.LIST.Alloc(_MESSAGE.Parse(node["ID"].ToString())))
			{
				obj.Initialize(node);
			}
		}
		public static void UnLoad_Message()
		{
			T_MESSAGE.LIST.Clear();
		}
		public static bool Load_Message_XML(string file_path)
		{
			using (XmlReader reader = new XmlReader())
			{
				reader.RegisterCallback_Line("MESSAGE", Callback_MESSAGE_XML);
				return reader.ReadFile(file_path);
			}
		}
		public static bool Load_Message_JSON(string file_path)
		{
			using (JsonReader reader = new JsonReader())
			{
				reader.RegisterCallback("MESSAGE", Callback_MESSAGE_JSON);
				return reader.ReadFile(file_path);
			}
		}
		public static void Save_Message_XML(string file_path)
		{
			using (XmlWriter writer = new XmlWriter())
			{
				{
				    _MESSAGE temp = new _MESSAGE();
				    PropTable tb_header = temp.ToTable();
				    System.Xml.XmlNode node = writer.Write_Header(tb_header, T_MESSAGE.LIST.Count, true);
				    foreach (T_MESSAGE obj in T_MESSAGE.LIST.ToArray())
				    {
				        PropTable tb = obj.ToTable();
				        writer.Write_Contents(node, tb);
				    }
				}
			    writer.Write_End(file_path);
			}
		}
		public static void Save_Message_JSON(string file_path)
		{
			TextWriter sw = new StreamWriter(file_path, false);
			sw.WriteLine("{");
			sw.WriteLine("\"MESSAGE\":[");
			for (int i = 0; i < T_MESSAGE.LIST.Count; i++)
			{
			    if (i > 0) sw.WriteLine(",");
			    sw.Write(T_MESSAGE.LIST.ElementAt(i).ToJson());
			}
			sw.WriteLine("]");
			sw.WriteLine("}");
			sw.Close();
		}
	}
} // end of namespace
