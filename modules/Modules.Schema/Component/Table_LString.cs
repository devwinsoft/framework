using System;
using System.IO;
using LitJson;
namespace Devarc
{
	public partial class Table
	{
		static void Callback_LString_Sheet(string sheet_name, PropTable tb)
		{
			LString obj = Table.T_LString.Alloc(tb.GetStr("Key"));
			if (obj == null)
			{
				Log.Error("[TableManager]Cannot create 'LString'. (id={0})", tb.GetStr("Key"));
				return;
			}
			obj.Initialize(tb);
		}
		static void Callback_LString_JSON(string sheet_name, JsonData node)
		{
			if (node.Keys.Contains("unit_type") == false) return;
			LString obj = Table.T_LString.Alloc(node["Key"].ToString());
			if (obj == null)
			{
				Log.Error("[TableManager]Cannot create 'LString'. (id={0})", node["Key"].ToString());
				return;
			}
			obj.Initialize(node);
		}
	    public static Container<LString, string> T_LString = new Container<LString, string>();
		public static bool isLoad_LString
		{
			get
			{
				if (Table.T_LString.Count > 0) return true;
				return false;
			}
		}
		public static void UnLoad_LString()
		{
			Table.T_LString.Clear();
		}
		public static bool Load_LString_SheetFile(string file_path)
		{
			using (XmlSheetReader reader = new XmlSheetReader())
			{
				reader.RegisterCallback_Data("LString", Callback_LString_Sheet);
				return reader.ReadFile(file_path);
			}
		}
		public static bool Load_LString_SheetData(string _data)
		{
			using (XmlSheetReader reader = new XmlSheetReader())
			{
				reader.RegisterCallback_Data("LString", Callback_LString_Sheet);
				return reader.ReadData(_data);
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
		public static void Save_LString_SheetFile(string file_path)
		{
			using (XmlSheetWriter writer = new XmlSheetWriter())
			{
				{
				    LString temp = new LString();
				    PropTable tb_header = temp.ToTable();
				    System.Xml.XmlNode node = writer.Write_Header(tb_header, Table.T_LString.Count, false);
				    for (int i = 0; i < Table.T_LString.Count; i++)
				    {
				        LString obj = Table.T_LString.ElementAt(i);
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
			for (int i = 0; i < Table.T_LString.Count; i++)
			{
			    if (i > 0) sw.WriteLine(",");
			    sw.Write(Table.T_LString.ElementAt(i).ToJson());
			}
			sw.WriteLine("]");
			sw.WriteLine("}");
			sw.Close();
		}
	}
} // end of namespace
