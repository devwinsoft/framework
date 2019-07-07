using System;
using System.IO;
using LitJson;
namespace Devarc
{
	public partial class Table
	{
		static void Callback_XML_SOUND(string sheet_name, PropTable tb)
		{
			SOUND obj = Table.T_SOUND.Alloc(tb.GetStr("SEQ"));
			if (obj == null)
			{
				Log.Error("[Table]Cannot create 'SOUND'. (id={0})", tb.GetStr("SEQ"));
				return;
			}
			obj.Initialize(tb);
		}
		static void Callback_Sheet_SOUND(string sheet_name, PropTable tb)
		{
			SOUND obj = Table.T_SOUND.Alloc(tb.GetStr("SEQ"));
			if (obj == null)
			{
				Log.Error("[Table]Cannot create 'SOUND'. (id={0})", tb.GetStr("SEQ"));
				return;
			}
			obj.Initialize(tb);
		}
		static void Callback_JSON_SOUND(string sheet_name, JsonData node)
		{
			if (node.Keys.Contains("SEQ") == false) return;
			SOUND obj = Table.T_SOUND.Alloc(node["SEQ"].ToString());
			if (obj == null)
			{
				Log.Error("[Table]Cannot create 'SOUND'. (id={0})", node["SEQ"].ToString());
				return;
			}
			obj.Initialize(node);
		}
	    public static Container<SOUND, string> T_SOUND = new Container<SOUND, string>();
		public static bool isLoad_Sound
		{
			get
			{
				if (Table.T_SOUND.Count > 0) return true;
				return false;
			}
		}
		public static void UnLoad_Sound()
		{
			Table.T_SOUND.Clear();
		}
		public static bool Load_XmlData_SOUND(string file_path)
		{
			using (XmlReader reader = new XmlReader())
			{
				reader.RegisterCallback_Data(Callback_XML_SOUND);
				return reader.ReadData(file_path);
			}
		}
		public static bool Load_XmlFile_SOUND(string file_path)
		{
			using (XmlReader reader = new XmlReader())
			{
				reader.RegisterCallback_Data(Callback_XML_SOUND);
				return reader.ReadFile(file_path);
			}
		}
		public static bool Load_SheetFile_Sound(string file_path)
		{
			using (XmlSheetReader reader = new XmlSheetReader())
			{
				reader.RegisterCallback_Data("SOUND", Callback_Sheet_SOUND);
				return reader.ReadFile(file_path);
			}
		}
		public static bool Load_SheetData_Sound(string _data)
		{
			using (XmlSheetReader reader = new XmlSheetReader())
			{
				reader.RegisterCallback_Data("SOUND", Callback_Sheet_SOUND);
				return reader.ReadData(_data);
			}
		}
		public static bool Load_JsonFile_Sound(string file_path)
		{
			using (JsonReader reader = new JsonReader())
			{
				reader.RegisterCallback("SOUND", Callback_JSON_SOUND);
				return reader.ReadFile(file_path);
			}
		}
		public static bool Load_JsonData_Sound(string _data)
		{
			using (JsonReader reader = new JsonReader())
			{
				reader.RegisterCallback("SOUND", Callback_JSON_SOUND);
				return reader.ReadData(_data);
			}
		}
		public static void Save_XmlFile_SOUND(string file_path)
		{
			using (XmlWriter writer = new XmlWriter("SOUND"))
			{
				SOUND temp = new SOUND();
				writer.Write_Begin(file_path, temp.ToTable());
				for (int i = 0; i < Table.T_SOUND.Count; i++)
				{
				    SOUND obj = Table.T_SOUND.ElementAt(i);
				    PropTable tb = obj.ToTable();
				    writer.Write_Data(tb);
				}
				writer.Write_End();
			}
		}
		public static void Save_SheetFile_Sound(string file_path)
		{
			using (XmlSheetWriter writer = new XmlSheetWriter())
			{
				{
				    SOUND temp = new SOUND();
				    PropTable tb_header = temp.ToTable();
				    System.Xml.XmlNode node = writer.Write_Header(tb_header, Table.T_SOUND.Count, false);
				    for (int i = 0; i < Table.T_SOUND.Count; i++)
				    {
				        SOUND obj = Table.T_SOUND.ElementAt(i);
				        PropTable tb = obj.ToTable();
				        writer.Write_Contents(node, tb);
				    }
				}
			    writer.Write_End(file_path);
			}
		}
		public static void Save_JsonFile_Sound(string file_path)
		{
			TextWriter sw = new StreamWriter(file_path, false);
			sw.WriteLine("{");
			sw.WriteLine("\"SOUND\":[");
			for (int i = 0; i < Table.T_SOUND.Count; i++)
			{
			    if (i > 0) sw.WriteLine(",");
			    sw.Write(Table.T_SOUND.ElementAt(i).ToJson());
			}
			sw.WriteLine("]");
			sw.WriteLine("}");
			sw.Close();
		}
	}
} // end of namespace
