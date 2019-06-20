using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Devarc
{
    public class Builder_Xml : Builder_Base, IDisposable
    {
        string FileName;
        string OutDir;
        TextWriter mWriter;

        public void Dispose()
        {
            Clear();
        }

        void Clear()
        {
            if (mWriter != null)
            {
                mWriter.Close();
                mWriter.Dispose();
                mWriter = null;
            }
        }

        public void Build_ExcelFile(string _inFilePath, string _outDir)
        {
            dataFileType = SCHEMA_TYPE.EXCEL;
            this.FileName = GetClassNameEx(_inFilePath);
            this.OutDir = _outDir;

            if (File.Exists(_inFilePath) == false)
            {
                Log.Info("Cannot find file: " + _inFilePath);
                return;
            }
            if (File.Exists(_inFilePath) == false)
            {
                Log.Info("Cannot find file: " + _inFilePath);
                return;
            }
            _build(_inFilePath);
        }

        void _build(string _inFilePath)
        {
            using (BaseSchemaReader reader = _createReader())
            {
                reader.RegisterCallback_Table(Callback_Table);
                reader.RegisterCallback_Data(Callback_Data);
                reader.RegisterCallback_Complete(Callback_Complete);
                reader.ReadFile(_inFilePath);
            }
        }

        void Callback_Table(string _sheetName, PropTable _prop)
        {
            if (_prop.KeyIndex < 0)
                return;
            string tableName = GetClassName(GetClassName(_sheetName));
            string filePath = Path.Combine(OutDir, tableName + ".xml");
            mWriter = new StreamWriter(filePath, false);
            mWriter.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            mWriter.WriteLine("<{0} xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">", tableName);
            mWriter.WriteLine("  <DataTypes>");
            for (int i = 0; i < _prop.Length; i++)
            {
                mWriter.WriteLine("    <DataType NAME=\"{0}\" TYPE_NAME=\"{1}\" CLASS_TYPE=\"{2}\" KEY=\"{3}\"/>"
                    , _prop.GetVarName(i), _prop.GetTypeName(i), _prop.GetClassType(i), _prop.KeyIndex == i);
            }
            mWriter.WriteLine("  </DataTypes>");
            mWriter.WriteLine("  <DATAS>");
        }

        void Callback_Data(string _sheetName, PropTable _prop)
        {
            mWriter.Write("    <DATA");
            for (int i = 0; i < _prop.Length; i++)
            {
                string varName = _prop.GetVarName(i);
                if (string.IsNullOrWhiteSpace(varName))
                    continue;
                mWriter.Write(" {0}=\"{1}\"", _prop.GetVarName(i), FrameworkUtil.InnerString_XML(_prop.GetStr(i)));
            }
            mWriter.WriteLine("/>");
        }

        void Callback_Complete(string _sheetName, PropTable _prop)
        {
            string tableName = GetClassName(GetClassName(_sheetName));
            mWriter.WriteLine("  </DATAS>");
            mWriter.WriteLine("</{0}>", tableName);
            Clear();
        }
    }
}
