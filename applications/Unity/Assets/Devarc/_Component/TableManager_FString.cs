using System;
using System.IO;
using LitJson;
namespace Devarc
{
	public partial class TableManager
	{
		public static bool isLoad_FString { get { return m_isLoad_FString;} set { m_isLoad_FString = value; } }
		private static bool m_isLoad_FString = false;
        static void Callback_FString_Sheet(string sheet_name, PropTable tb)
        {
            using (T_FString obj = T_FString.MAP.Alloc(tb.GetStr("Key")))
            {
                if (obj == null)
                {
                    Log.Error("[TableManager]Cannot create 'FString'. (id={0})", tb.GetStr("Key"));
                    return;
                }
                obj.Initialize(tb);
            }
        }
        static void Callback_FString_JSON(string sheet_name, JsonData node)
        {
            if (node.Keys.Contains("unit_type") == false) return;
            using (T_FString obj = T_FString.MAP.Alloc(node["Key"].ToString()))
            {
                if (obj == null)
                {
                    Log.Error("[TableManager]Cannot create 'FString'. (id={0})", node["Key"].ToString());
                    return;
                }
                obj.Initialize(node);
            }
        }
        public static void UnLoad_FString()
		{
			T_FString.MAP.Clear();
		}
		public static bool Load_FString_SheetFile(string _filePath)
		{
			using (XmlSheetReader reader = new XmlSheetReader())
			{
				reader.RegisterCallback_DataLine("FString", Callback_FString_Sheet);
				return reader.ReadFile(_filePath);
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
		public static bool Load_FString_JsonFile(string _filePath)
		{
			using (JsonReader reader = new JsonReader())
			{
				reader.RegisterCallback("FString", Callback_FString_JSON);
				return reader.ReadFile(_filePath);
			}
		}
		public static void Save_FString_SheetFile(string _filePAth)
		{
			using (XmlSheetWriter writer = new XmlSheetWriter())
			{
				{
				    FString temp = new FString();
				    PropTable tb_header = temp.ToTable();
				    System.Xml.XmlNode node = writer.Write_Header(tb_header, T_FString.MAP.Count, false);
				    for (int i = 0; i < T_FString.MAP.Count; i++)
				    {
				        T_FString obj = T_FString.MAP.ElementAt(i);
				        PropTable tb = obj.ToTable();
				        writer.Write_Contents(node, tb);
				    }
				}
			    writer.Write_End(_filePAth);
			}
		}
		public static void Save_FString_JsonFile(string _filePath)
		{
			TextWriter sw = new StreamWriter(_filePath, false);
			sw.WriteLine("{");
			sw.WriteLine("\"FString\":[");
			for (int i = 0; i < T_FString.MAP.Count; i++)
			{
			    if (i > 0) sw.WriteLine(",");
			    sw.Write(T_FString.MAP.ElementAt(i).ToJson());
			}
			sw.WriteLine("]");
			sw.WriteLine("}");
			sw.Close();
		}
	}
} // end of namespace
