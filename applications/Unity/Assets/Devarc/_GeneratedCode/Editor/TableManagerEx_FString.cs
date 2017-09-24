using System;
using System.IO;
using LitJson;
namespace Devarc
{
	public partial class TableManagerEx
	{
		static void Callback_FString_Sheet(string sheet_name, PropTable tb)
		{
			FString obj = TableManager.T_FString.Alloc(tb.GetStr("Key"));
			if (obj == null)
			{
				Log.Error("[TableManager]Cannot create 'FString'. (id={0})", tb.GetStr("Key"));
				return;
			}
			obj.Initialize(tb);
		}
		static void Callback_FString_JSON(string sheet_name, JsonData node)
		{
			if (node.Keys.Contains("unit_type") == false) return;
			FString obj = TableManager.T_FString.Alloc(node["Key"].ToString());
			if (obj == null)
			{
				Log.Error("[TableManager]Cannot create 'FString'. (id={0})", node["Key"].ToString());
				return;
			}
			obj.Initialize(node);
		}
	    public static Container<FString, string> T_FString = new Container<FString, string>();
		public static bool isLoad_FString
		{
			get
			{
				if (TableManager.T_FString.Count > 0) return true;
				return false;
			}
		}
		public static void UnLoad_FString()
		{
			TableManager.T_FString.Clear();
		}
		public static bool Load_FString_ExcelFile(string file_path)
		{
			using (ExcelReader reader = new ExcelReader())
			{
				reader.RegisterCallback_DataLine("FString", Callback_FString_Sheet);
				return reader.ReadFile(file_path);
			}
		}
		public static void Save_FString_ExcelFile(string file_path)
		{
			using (ExcelWriter writer = new ExcelWriter())
			{
				{
				    FString temp = new FString();
				    PropTable tb_header = temp.ToTable();
				    writer.Write_Header(tb_header, false);
				    for (int i = 0; i < TableManager.T_FString.Count; i++)
				    {
				        FString obj = TableManager.T_FString.ElementAt(i);
				        PropTable tb = obj.ToTable();
				        writer.Write_Contents(tb);
				    }
				}
			    writer.Write_End(file_path);
			}
		}
		public static bool Load_FString_SheetFile(string file_path)
		{
			using (XmlSheetReader reader = new XmlSheetReader())
			{
				reader.RegisterCallback_DataLine("FString", Callback_FString_Sheet);
				return reader.ReadFile(file_path);
			}
		}
		public static bool Load_FString_SheetData(string _data)
		{
			using (XmlSheetReader reader = new XmlSheetReader())
			{
				reader.RegisterCallback_DataLine("FString", Callback_FString_Sheet);
				return reader.ReadData(_data);
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
		public static void Save_FString_SheetFile(string file_path)
		{
			using (XmlSheetWriter writer = new XmlSheetWriter())
			{
				{
				    FString temp = new FString();
				    PropTable tb_header = temp.ToTable();
				    System.Xml.XmlNode node = writer.Write_Header(tb_header, TableManager.T_FString.Count, false);
				    for (int i = 0; i < TableManager.T_FString.Count; i++)
				    {
				        FString obj = TableManager.T_FString.ElementAt(i);
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
			for (int i = 0; i < TableManager.T_FString.Count; i++)
			{
			    if (i > 0) sw.WriteLine(",");
			    sw.Write(TableManager.T_FString.ElementAt(i).ToJson());
			}
			sw.WriteLine("]");
			sw.WriteLine("}");
			sw.Close();
		}
	}
} // end of namespace
