using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Devarc
{
    public class XmlWriter : IDisposable
    {
        TextWriter mWriter;
        string mTableName;

        public XmlWriter(string _tableName)
        {
            mTableName = _tableName;
        }

        public void Dispose()
        {
            mWriter.Close();
            mWriter.Dispose();
            mWriter = null;
        }

        public void Write_Begin(string _fileName, PropTable _prop)
        {
            mWriter = new StreamWriter(_fileName, false);
            mWriter.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            mWriter.WriteLine("<{0} xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">", mTableName);
            mWriter.WriteLine("  <DataTypes>");
            for (int i = 0; i < _prop.Length; i++)
            {
                mWriter.WriteLine("    <DataType NAME=\"{0}\" TYPE_NAME=\"{1}\" CLASS_TYPE=\"{2}\" KEY=\"{3}\"/>"
                    , _prop.GetVarName(i), _prop.GetTypeName(i), _prop.GetClassType(i), _prop.KeyIndex == i);
            }
            mWriter.WriteLine("  </DataTypes>");
            mWriter.WriteLine("  <Datas>");
        }

        public void Write_Data(PropTable _prop)
        {
            mWriter.Write("    <Data");
            for (int i = 0; i < _prop.Length; i++)
            {
                mWriter.Write("    <Data {0}=\"{1}\"/>", _prop.GetVarName(i), FrameworkUtil.InnerString(_prop.GetStr(i)));
            }
            mWriter.WriteLine("/>");
        }

        public void Write_End()
        {
            mWriter.WriteLine("  </Datas>");
            mWriter.WriteLine("</{0}>", mTableName);
            mWriter.Close();
        }
    }
}
