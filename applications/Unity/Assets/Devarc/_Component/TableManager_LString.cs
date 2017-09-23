using System;
using System.IO;
using LitJson;
namespace Devarc
{
	public partial class TableManager
	{
		public static bool isLoad_LString { get { return m_isLoad_LString;} set { m_isLoad_LString = value; } }
		private static bool m_isLoad_LString = false;
        static void Callback_LString_Sheet(string sheet_name, PropTable tb)
        {
            using (T_LString obj = T_LString.MAP.Alloc(tb.GetStr("Key")))
            {
                if (obj == null)
                {
                    Log.Error("[TableManager]Cannot create 'LString'. (id={0})", tb.GetStr("Key"));
                    return;
                }
                obj.Initialize(tb);
            }
        }
        static void Callback_LString_JSON(string sheet_name, JsonData node)
        {
            if (node.Keys.Contains("unit_type") == false) return;
            using (T_LString obj = T_LString.MAP.Alloc(node["Key"].ToString()))
            {
                if (obj == null)
                {
                    Log.Error("[TableManager]Cannot create 'LString'. (id={0})", node["Key"].ToString());
                    return;
                }
                obj.Initialize(node);
            }
        }
        public static void UnLoad_LString()
		{
			T_LString.MAP.Clear();
		}
		public static bool Load_LString_SheetFile(string _filePath)
		{
			using (XmlSheetReader reader = new XmlSheetReader())
			{
				reader.RegisterCallback_DataLine("LString", Callback_LString_Sheet);
				return reader.ReadFile(_filePath);
			}
		}
		public static bool Load_LString_SheetData(string _data)
		{
			using (XmlSheetReader reader = new XmlSheetReader())
			{
				reader.RegisterCallback_DataLine("LString", Callback_LString_Sheet);
				return reader.ReadData(_data);
			}
		}
		public static bool Load_LString_JsonFile(string _filePath)
		{
			using (JsonReader reader = new JsonReader())
			{
				reader.RegisterCallback("LString", Callback_LString_JSON);
				return reader.ReadFile(_filePath);
			}
		}
		public static void Save_LString_SheetFile(string _filePath)
		{
			using (XmlSheetWriter writer = new XmlSheetWriter())
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
			    writer.Write_End(_filePath);
			}
		}
		public static void Save_LString_JsonFile(string _filePath)
		{
			TextWriter sw = new StreamWriter(_filePath, false);
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
