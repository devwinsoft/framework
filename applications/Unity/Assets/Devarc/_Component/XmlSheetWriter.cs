//
// Copyright (c) 2012 Hyoung Joon, Kim
// http://www.devwinsoft.com/
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

//
// @author Hyoung Joon, Kim (maoshy@nate.com)
// @version $Rev: 1, $Date: 2012-02-20
//

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Devarc
{
    public class XmlSheetWriter : IDisposable
    {
        XmlDocument m_XmlDoc = new XmlDocument();

        public XmlSheetWriter()
        {
            m_XmlDoc.LoadXml(xml_source);
        }
        public void Dispose()
        {
        }

        // returns table node
        public XmlNode Write_Header(PropTable tb, int cnt, bool is_enum)
        {
            XmlElement root = m_XmlDoc.DocumentElement;

            XmlNode sheet = m_XmlDoc.CreateNode(XmlNodeType.Element, "Worksheet", "urn:schemas-microsoft-com:office:spreadsheet");
            root.AppendChild(sheet);

            XmlAttribute sheet_name = m_XmlDoc.CreateAttribute("ss:Name", "urn:schemas-microsoft-com:office:spreadsheet");
            if(is_enum)
                sheet_name.Value = "!" + tb.TableName;
            else
                sheet_name.Value = tb.TableName;
            sheet.Attributes.Append(sheet_name);


            XmlNode table = m_XmlDoc.CreateNode("element", "Table", "urn:schemas-microsoft-com:office:spreadsheet");
            sheet.AppendChild(table);


            XmlAttribute attr_col = m_XmlDoc.CreateAttribute("ss:ExpandedColumnCount", "urn:schemas-microsoft-com:office:spreadsheet");
            XmlAttribute attr_row = m_XmlDoc.CreateAttribute("ss:ExpandedRowCount", "urn:schemas-microsoft-com:office:spreadsheet");
            attr_col.Value = tb.Length.ToString();
            attr_row.Value = (cnt + (int)ROW_TYPE.DATA_FIELD).ToString();
            table.Attributes.Append(attr_col);
            table.Attributes.Append(attr_row);

            for (int line = 0; line < (int)ROW_TYPE.DATA_FIELD - 1; line++)
            {
                XmlNode row = m_XmlDoc.CreateNode("element", "Row", "urn:schemas-microsoft-com:office:spreadsheet");
                table.AppendChild(row);
                for (int i = 0; i < tb.Length; i++)
                {
                    XmlNode cell = m_XmlDoc.CreateNode("element", "Cell", "urn:schemas-microsoft-com:office:spreadsheet");
                    row.AppendChild(cell);
                    XmlNode data = m_XmlDoc.CreateNode("element", "Data", "urn:schemas-microsoft-com:office:spreadsheet");
                    cell.AppendChild(data);
                    XmlAttribute data_type = m_XmlDoc.CreateAttribute("ss:Type", "urn:schemas-microsoft-com:office:spreadsheet");
                    data_type.Value = "String";
                    data.Attributes.Append(data_type);

                    switch ((ROW_TYPE)(line+1))
                    {
                        case ROW_TYPE.VAR_NAME:
                            data.InnerText = tb.GetVarName(i);
                            break;
                        case ROW_TYPE.TYPE_NAME:
                            data.InnerText = tb.GetTypeName(i);
                            break;
                        case ROW_TYPE.CLASS_TYPE:
                            data.InnerText = tb.GetClassType(i).ToString();
                            break;
                        case ROW_TYPE.KEY_TYPE:
                            data.InnerText = (tb.GetKeyType(i)).ToString();
                            break;
                        default:
                            break;
                    }
                }
            }
            return table;
        }

        public void Write_Contents(XmlNode table, PropTable tb)
        {
            XmlNode row = m_XmlDoc.CreateNode("element", "Row", "urn:schemas-microsoft-com:office:spreadsheet");
            table.AppendChild(row);

            for (int i = 0; i < tb.Length; i++)
            {
                XmlNode cell = m_XmlDoc.CreateNode("element", "Cell", "urn:schemas-microsoft-com:office:spreadsheet");
                row.AppendChild(cell);
                XmlNode data = m_XmlDoc.CreateNode("element", "Data", "urn:schemas-microsoft-com:office:spreadsheet");
                cell.AppendChild(data);
                XmlAttribute data_type = m_XmlDoc.CreateAttribute("ss:Type", "urn:schemas-microsoft-com:office:spreadsheet");
                data_type.Value = "String";
                data.Attributes.Append(data_type);

                data.InnerXml = tb.GetStr(i).Replace("\r", "").Replace("\n", "@#34;").Replace("\n", "@#n;");
            }
        }

        public void Write_End(string file_path)
        {
            m_XmlDoc.Save(file_path);
            TextReader sr = new StreamReader(file_path, true);
            TextWriter sw = new StreamWriter(file_path + ".work", false);
            while(true)
            {
                string data = sr.ReadLine();
                if (data == null)
                    break;
                sw.Write(data.Replace("@#34;", "\"").Replace("@#n;", "&#10;"));
            }
            sr.Close();
            sw.Close();
            File.Delete(file_path);
            File.Move(file_path + ".work", file_path);
        }

        const string xml_source =
"<?xml version=\"1.0\"?>"+
"<?mso-application progid=\"Excel.Sheet\"?>"+
"<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\""+
" xmlns:o=\"urn:schemas-microsoft-com:office:office\""+
" xmlns:x=\"urn:schemas-microsoft-com:office:excel\""+
" xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\""+
" xmlns:html=\"http://www.w3.org/TR/REC-html40\">"+
" <DocumentProperties xmlns=\"urn:schemas-microsoft-com:office:office\">"+
" </DocumentProperties>"+
" <ExcelWorkbook xmlns=\"urn:schemas-microsoft-com:office:excel\">"+
"  <WindowHeight>11175</WindowHeight>"+
"  <WindowWidth>19815</WindowWidth>"+
"  <WindowTopX>1350</WindowTopX>"+
"  <WindowTopY>345</WindowTopY>"+
"  <TabRatio>571</TabRatio>"+
"  <ProtectStructure>False</ProtectStructure>"+
"  <ProtectWindows>False</ProtectWindows>"+
" </ExcelWorkbook>"+
"</Workbook>";
    }

}

