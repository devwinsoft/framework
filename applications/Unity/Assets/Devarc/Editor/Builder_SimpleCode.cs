using System;
using System.IO;
using System.Linq;
using System.Text;
using LitJson;
using Devarc;

namespace Devarc
{
    class Builder_SimpleCode : Builder_Base
    {
        StringBuilder mStrBuilder = new StringBuilder();

        public bool Read(string _filePath)
        {
            string tmpPath = _filePath;
            string tmpFullPath;
            if (tmpPath.Length > 2 && tmpPath[1] == ':')
                tmpFullPath = tmpPath;
            else
                tmpFullPath = Path.Combine(Directory.GetCurrentDirectory(), tmpPath);

            if (_filePath.ToLower().EndsWith("xml"))
                dataFileType = DATA_FILE_TYPE.SHEET;
            else
                dataFileType = DATA_FILE_TYPE.EXCEL;

            using (BaseDataReader reader = _createReader())
            {
                reader.RegisterCallback_EveryTable(Callback_LoadSheet);
                reader.ReadFile(tmpFullPath);
            }

            return true;
        }

        public override string ToString()
        {
            return mStrBuilder.ToString();
        }

        void Callback_LoadSheet(string sheet_name, PropTable tb)
        {
            bool is_enum = sheet_name.StartsWith("!");
            string class_name = is_enum ? sheet_name.Substring(1) : sheet_name;
            if (is_enum)
                mStrBuilder.Append(string.Format("public enum {0} {{ }}\r\n", class_name ));
            else
                mStrBuilder.Append(string.Format("public class {0} {{ }}\r\n", class_name));
        }
    }
}
