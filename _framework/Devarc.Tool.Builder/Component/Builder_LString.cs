using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Devarc
{
    class Builder_LString : Builder_Base
    {
        string TableName = "LString";
        string FileName = "";
        string OutFilePath = "";
        TextWriter mWriter;

        List<string> mKeyList = new List<string>();
        List<KeyValuePair<string, string>> mDataList = new List<KeyValuePair<string, string>>();

        public void Dispose()
        {
            Clear();
        }

        void Clear()
        {
            mKeyList.Clear();
            mDataList.Clear();

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
            FileName = Path.GetFileNameWithoutExtension(_inFilePath);
            OutFilePath = Path.Combine(_outDir, string.Format("LString_{0}.xml", this.FileName));

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
                reader.ReadFile(_inFilePath);
            }
            write();
        }

        void write()
        {
            mWriter = new StreamWriter(OutFilePath, false);
            mWriter.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            mWriter.WriteLine("<{0} xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">", TableName);

            mWriter.WriteLine("  <DataTypes>");
            mWriter.WriteLine("    <DataType NAME=\"Key\" TYPE_NAME=\"string\" KEY=\"True\"/>");
            mWriter.WriteLine("    <DataType NAME=\"Value\" TYPE_NAME=\"string\"/>");
            mWriter.WriteLine("  </DataTypes>");

            mWriter.WriteLine("  <DATAS>");
            for (int i = 0; i < mDataList.Count; i++)
            {
                KeyValuePair<string, string> data = mDataList[i];
                mWriter.WriteLine("    <DATA Key=\"{0}\" Value=\"{1}\"/>", data.Key, data.Value);
            }
            mWriter.WriteLine("  </DATAS>");
            mWriter.WriteLine("</{0}>", TableName);
            Clear();
        }

        void Callback_Table(string _sheetName, PropTable _prop)
        {
        }

        void Callback_Data(string _sheetName, PropTable _prop)
        {
            string className = base.GetClassName(_sheetName);
            for (int i = 0; i < _prop.Length; i++)
            {
                string varName = _prop.GetVarName(i);
                string keyValue = _prop.GetStr(_prop.KeyIndex);
                if (string.IsNullOrWhiteSpace(varName))
                    continue;
                if (_prop.GetVarType(i) == VAR_TYPE.LSTRING)
                {
                    string keyName = string.Format("{0}_{1}_{2}", className, varName, keyValue);
                    string value = FrameworkUtil.InnerString_XML(_prop.GetStr(i));
                    if (mKeyList.Contains(keyName))
                        continue;

                    mKeyList.Add(keyName);
                    mDataList.Add(new KeyValuePair<string, string>(keyName, value));
                }
            }
        }

    }
}
